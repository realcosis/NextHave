using Dolphin.Core.Configurations;
using Dolphin.Core.Injection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using NextHave.Gateway.Extensions;
using NextHave.Gateway.Middleware;
using NextHave.Gateway.Models.Configurations;
using NextHave.Gateway.Services;
using System.Security.Cryptography.X509Certificates;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ApplicationName = "NextHave.Gateway"
});

builder.Host.ConfigureDolphinApplication<NextHaveConfiguration>("NextHave", "1");

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(System.Net.IPAddress.Any, 80);
    serverOptions.Listen(System.Net.IPAddress.Any, 443, listenOptions =>
    {
        var cert = X509Certificate2.CreateFromPemFile("/etc/ssl/certs/server/cert.pem", "/etc/ssl/certs/server/key.pem");
        listenOptions.UseHttps(cert);
    });
    serverOptions.Limits.MaxConcurrentConnections = 1050;
    serverOptions.Limits.MaxConcurrentUpgradedConnections = 100;
    serverOptions.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
    serverOptions.Limits.MinRequestBodyDataRate = new MinDataRate(100, TimeSpan.FromSeconds(10));
    serverOptions.Limits.MinResponseDataRate = new MinDataRate(100, TimeSpan.FromSeconds(10));
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(120);
});

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IConnectionThrottler, ConnectionThrottler>();
builder.Services.AddSingleton<IChallengeService, ChallengeService>();
builder.Services.AddSingleton<IStartableService, ConnectionsService>();

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("PerIpRateLimit", context => RateLimitPartition.GetFixedWindowLimiter(context.GetUserIp(), partition => new FixedWindowRateLimiterOptions
    {
        AutoReplenishment = true,
        PermitLimit = 100,
        Window = TimeSpan.FromMinutes(1),
        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        QueueLimit = 0
    }));

    options.AddPolicy("StrictRateLimit", context => RateLimitPartition.GetFixedWindowLimiter(context.GetUserIp(), partition => new FixedWindowRateLimiterOptions
    {
        AutoReplenishment = true,
        PermitLimit = 10,
        Window = TimeSpan.FromMinutes(1),
        QueueLimit = 0
    }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCMS", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Origin:Hosts").Get<string[]>() ?? [];

        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddHttpClient();

builder.Services.AddControllers();

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    KnownNetworks = { new IPNetwork(System.Net.IPAddress.Parse("10.0.0.0"), 8) },
    ForwardLimit = 2
});

app.UseWebSockets();

app.UseMiddleware<ConnectionThrottlingMiddleware>();

app.UseMiddleware<ChallengeMiddleware>();

app.MapReverseProxy();

app.UseRateLimiter();

app.UseCors("AllowCMS");

app.UseWebSockets();

app.UseRouting();

app.UseAuthorization();

app.MapControllers().RequireRateLimiting("PerIpRateLimit").RequireRateLimiting("StrictRateLimit");

app.MapGet("/health", () => Results.Ok("Healthy")).RequireRateLimiting("PerIpRateLimit").RequireRateLimiting("StrictRateLimit");

var services = app.Services.GetRequiredService<IEnumerable<IStartableService>>();
foreach (var service in services)
    await service.StartAsync();

await app.RunAsync();