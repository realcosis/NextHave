using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Dolphin.Core.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerCustom(this IServiceCollection services, IConfiguration configuration, bool useAuthorization = true)
        {
            var apiInfo = configuration.GetProjectConfiguration()!;
            var apiVersion = $"v{apiInfo.Version}";

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(apiVersion, new OpenApiInfo
                {
                    Title = $"{apiInfo.Title} API",
                    Version = apiVersion,
                });
                options.CustomOperationIds(cOIds => $"{cOIds.ActionDescriptor.RouteValues["controller"]}_{cOIds.ActionDescriptor.RouteValues["action"]}");

                if (useAuthorization)
                {
                    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey
                    });
                    options.OperationFilter<SecurityRequirementsOperationFilter>();
                }

                options.MapType<IFormFile>(() => new OpenApiSchema()
                {
                    Type = "string",
                    Format = "binary"
                });

            }).AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        public static IApplicationBuilder UseSwaggerGenCustom(this IApplicationBuilder app, string? version = default)
        {
            app.UseSwagger(options =>
            {
                options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    var baseUrl = $"{httpReq.Scheme}://{httpReq.Host.Value}";

                    swaggerDoc.Servers =
                    [
                        new()
                        {
                            Description = $"{httpReq.Host.Value} server",
                            Url = baseUrl
                        }
                    ];
                });
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/v{version}/swagger.json", $"v{version}");
                options.OAuthScopeSeparator(" ");
            });

            return app;
        }
    }
}