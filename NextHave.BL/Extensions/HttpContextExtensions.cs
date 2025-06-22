using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace NextHave.BL.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetUserIp(this IHttpContextAccessor httpContextAccessor)
        {
            var context = httpContextAccessor.HttpContext;
            if (context == null)
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