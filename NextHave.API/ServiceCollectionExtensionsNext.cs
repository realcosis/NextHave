using Dolphin.Core.API;
using Dolphin.Core.API.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.IdentityModel.Logging;
using System.Net;

namespace NextHave.API
{
    public static class ServiceCollectionExtensionsNext
    {
        public static IServiceCollection RegisterAPIApplicationCustom(this IServiceCollection services, IConfiguration configuration, bool useAuthorization = true, Action<MvcOptions>? mvcOptionsAction = null)
        {
            IdentityModelEventSource.ShowPII = true;

            services.AddAPIControllers(mvcOptionsAction);
            services.AddHttpClient();
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.SuppressMapClientErrors = true;
            });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
                });
            });
            services.AddAPIVersioning(configuration);
            services.AddSwaggerCustom(configuration, useAuthorization);

            return services;
        }

        public static IApplicationBuilder UseAPIApplicationCustom(this IApplicationBuilder application, IWebHostEnvironment environment, IConfiguration configuration)
        {
            var projectconfiguration = configuration.GetProjectConfiguration()!;

            if (environment.IsDevelopment())
                application.UseDeveloperExceptionPage();
            else
                application.UseHsts();

            var forwardedHeadersOptions = new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.All
            };

            forwardedHeadersOptions.KnownProxies.Clear();
            forwardedHeadersOptions.KnownNetworks.Clear();
            application.UseForwardedHeaders(forwardedHeadersOptions);
            application.Use((context, next) =>
            {
                context.Request.Scheme = "https";

                return next();
            });
            application.UseCors("default");
            application.UseHttpsRedirection();
            application.UseRouting();
            application.UseAuthentication();
            application.UseAuthorization();
            application.UseSwaggerGenCustom(projectconfiguration.Version);
            application.UseRewriter(new RewriteOptions().AddRedirect("^$", "/swagger"));
            application.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller}/{action}/{id?}");
            });

            return application;
        }

        internal static void RegisterFilter(this ActionModel action, HttpStatusCode statusCode, Type? type = default)
        {
            if (type != default)
                action.Filters.Add(new ProducesResponseTypeAttribute(type, (int)statusCode));
            else
                action.Filters.Add(new ProducesResponseTypeAttribute((int)statusCode));
        }
    }
}