using NextHave.Gateway.Services;
using NextHave.Gateway.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace NextHave.Gateway.Middleware
{
    public class ChallengeMiddleware(RequestDelegate next, IChallengeService challengeService, IMemoryCache cache, ILogger<ChallengeMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.GetUserIp();

            if (context.Request.Path == "/" && context.Request.Method == "GET")
            {
                if (!IsAuthorized(ip))
                {
                    await SendChallenge(context, ip);
                    return;
                }
            }

            if (context.Request.Path == "/verify-challenge" && context.Request.Method == "POST")
            {
                await HandleChallenge(context, ip);
                return;
            }

            if (!IsAuthorized(ip))
            {
                context.Response.StatusCode = 401;
                return;
            }

            await next(context);
        }

        bool IsAuthorized(string ip)
            => challengeService.IsWhitelisted(ip) || cache.TryGetValue($"verified_{ip}", out _);

        async Task SendChallenge(HttpContext context, string ip)
        {
            var challenge = challengeService.GenerateChallenge(ip);

            var random = new Random();
            var num1 = random.Next(1, 10);
            var num2 = random.Next(1, 10);
            var captchaAnswer = num1 + num2;

            cache.Set($"captcha_{ip}", captchaAnswer, TimeSpan.FromMinutes(5));

            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/html";

            var html = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <title>Security Challenge</title>
        <style>
            body {{
                font-family: Arial, sans-serif;
                display: flex;
                justify-content: center;
                align-items: center;
                height: 100vh;
                margin: 0;
                background-color: #f0f0f0;
            }}
            .challenge-box {{
                background: white;
                padding: 30px;
                border-radius: 10px;
                box-shadow: 0 2px 10px rgba(0,0,0,0.1);
                text-align: center;
                max-width: 400px;
            }}
            h2 {{
                color: #333;
                margin-bottom: 20px;
            }}
            .captcha {{
                font-size: 24px;
                margin: 20px 0;
                padding: 15px;
                background: #f9f9f9;
                border-radius: 5px;
            }}
            input[type='text'] {{
                padding: 10px;
                font-size: 16px;
                border: 2px solid #ddd;
                border-radius: 5px;
                width: 100px;
                margin: 10px;
            }}
            button {{
                background-color: #4CAF50;
                color: white;
                padding: 12px 30px;
                border: none;
                border-radius: 5px;
                font-size: 16px;
                cursor: pointer;
                margin-top: 10px;
            }}
            button:hover {{
                background-color: #45a049;
            }}
            .error {{
                color: red;
                margin-top: 10px;
                display: none;
            }}
        </style>
    </head>
    <body>
        <div class='challenge-box'>
            <h2>Security Verification</h2>
            <p>Please solve this simple math problem to continue:</p>
            <div class='captcha'>
                {num1} + {num2} = ?
            </div>
            <form id='challengeForm'>
                <input type='hidden' name='challenge' value='{challenge}' />
                <input type='text' name='captcha' id='captchaAnswer' placeholder='Answer' required autofocus />
                <br/>
                <button type='submit'>Verify</button>
                <div class='error' id='errorMsg'>Incorrect answer. Please try again.</div>
            </form>
        </div>
        
        <script>
            document.getElementById('challengeForm').addEventListener('submit', async (e) => {{
                e.preventDefault();
                const formData = new FormData();
                formData.append('response', document.querySelector('input[name=""challenge""]').value);
                formData.append('captcha', document.getElementById('captchaAnswer').value);
                
                try {{
                    const response = await fetch('/verify-challenge', {{
                        method: 'POST',
                        body: formData
                    }});
                    
                    const data = await response.json();
                    
                    if (data.success) {{
                        // Se successo, il server farà redirect
                        window.location.href = data.redirectUrl || '/';
                    }} else {{
                        document.getElementById('errorMsg').style.display = 'block';
                        document.getElementById('captchaAnswer').value = '';
                        document.getElementById('captchaAnswer').focus();
                    }}
                }} catch (error) {{
                    console.error('Error:', error);
                    document.getElementById('errorMsg').textContent = 'An error occurred. Please try again.';
                    document.getElementById('errorMsg').style.display = 'block';
                }}
            }});
        </script>
    </body>
    </html>";

            await context.Response.WriteAsync(html);
        }

        async Task HandleChallenge(HttpContext context, string ip)
        {
            var form = await context.Request.ReadFormAsync();
            var challengeResponse = form["response"].ToString();
            var captchaResponse = form["captcha"].ToString();

            if (cache.TryGetValue($"captcha_{ip}", out int expectedAnswer))
            {
                if (!int.TryParse(captchaResponse, out int userAnswer) || userAnswer != expectedAnswer)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { success = false, message = "Invalid CAPTCHA answer" });
                    logger.LogWarning("IP {ip} failed CAPTCHA", ip);
                    return;
                }

                cache.Remove($"captcha_{ip}");
            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { success = false, message = "CAPTCHA expired" });
                return;
            }

            if (challengeService.ValidateChallenge(ip, challengeResponse))
            {
                cache.Set($"verified_{ip}", true, TimeSpan.FromHours(1));

                context.Response.StatusCode = 200;
                await context.Response.WriteAsJsonAsync(new
                {
                    success = true,
                    redirectUrl = "/"
                });
                logger.LogInformation("IP {ip} passed challenge, redirecting to Blazor", ip);
            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { success = false, message = "Invalid challenge response" });
                logger.LogWarning("IP {ip} failed challenge", ip);
            }
        }
    }
}