using NextHave.Gateway.Services;
using NextHave.Gateway.Extensions;

namespace NextHave.Gateway.Middleware
{
    public class ConnectionThrottlingMiddleware(RequestDelegate next, IConnectionThrottler throttler, ILogger<ConnectionThrottlingMiddleware> logger)
    {
        readonly HashSet<string> StaticExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".css", ".js", ".json", ".map",
            ".png", ".jpg", ".jpeg", ".gif", ".svg", ".webp", ".ico",
            ".woff", ".woff2", ".ttf", ".eot", ".otf", ".br", ".gz"
        };

        public async Task InvokeAsync(HttpContext context)
        {
            if (IsStaticFileRequest(context.Request) || IsSocketHubRequet(context.Request))
            {
                await next(context);
                return;
            }

            var sessionId = context.Request.Cookies["SessionId"];

            if (string.IsNullOrWhiteSpace(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                context.Response.Cookies.Append("SessionId", sessionId, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    MaxAge = TimeSpan.FromHours(24)
                });
            }

            var connection = context.Connection;
            var ipAddress = context.GetUserIp();
            var remotePort = connection.RemotePort;
            var localIpAddress = connection.LocalIpAddress?.ToString();
            var localPort = connection.LocalPort;
            var connectionId = connection.Id;

            logger.LogDebug("Connection from {ipAddress}:{remotePort} to {localIpAddress}:{localPort} (ID: {connectionId})", ipAddress, remotePort, localIpAddress, localPort, connectionId);

            if (!throttler.ShouldAllowConnection(ipAddress, sessionId))
            {
                logger.LogWarning("Connection throttled for IP: {ipAddress}:{remotePort} (Connection ID: {connectionId})", ipAddress, remotePort, connectionId);

                connection.RequestClose();

                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            throttler.RegisterConnection(ipAddress, sessionId);

            await next(context);
        }

        #region private methods

        bool IsStaticFileRequest(HttpRequest request)
        {
            var path = request.Path;

            if (path.StartsWithSegments("/_content") || path.StartsWithSegments("/_blazor"))
                return true;

            return path.HasValue && Path.HasExtension(path) && StaticExtensions.Contains(Path.GetExtension(path));
        }

        bool IsSocketHubRequet(HttpRequest request)
        {
            var path = request.Path;

            if (path.StartsWithSegments("/socket"))
                return true;

            return false;
        }

        #endregion
    }
}