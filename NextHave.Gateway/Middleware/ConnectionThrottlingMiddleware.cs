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

        private bool IsStaticFileRequest(HttpRequest request)
        {
            var path = request.Path;

            if (path.StartsWithSegments("/_content"))
                return true;

            return path.HasValue && Path.HasExtension(path) && StaticExtensions.Contains(Path.GetExtension(path));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (IsStaticFileRequest(context.Request))
            {
                await next(context);
                return;
            }

            var connection = context.Connection;
            var ipAddress = context.GetUserIp();
            var remotePort = connection.RemotePort;
            var localIpAddress = connection.LocalIpAddress?.ToString();
            var localPort = connection.LocalPort;
            var connectionId = connection.Id;

            logger.LogDebug("Connection from {ipAddress}:{remotePort} to {localIpAddress}:{localPort} (ID: {connectionId})", ipAddress, remotePort, localIpAddress, localPort, connectionId);

            if (!throttler.ShouldAllowConnection(ipAddress))
            {
                logger.LogWarning("Connection throttled for IP: {ipAddress}:{remotePort} (Connection ID: {connectionId})", ipAddress, remotePort, connectionId);

                connection.RequestClose();

                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            throttler.RegisterConnection(ipAddress);

            await next(context);
        }
    }
}