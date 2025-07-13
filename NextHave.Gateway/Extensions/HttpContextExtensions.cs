using Microsoft.Extensions.Primitives;

namespace NextHave.Gateway.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetUserIp(this HttpContext context)
        {
            if (context == default)
                return "unknown";

            var headers = context.Request.Headers;

            if (headers.TryGetValue("CF-Connecting-IP", out StringValues value))
                return value.ToString();

            if (headers.TryGetValue("X-Forwarded-For", out value))
            {
                var ip = value.ToString();

                if (!string.IsNullOrWhiteSpace(ip))
                    return ip.Split(',').FirstOrDefault()?.Trim() ?? "unknown";
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}