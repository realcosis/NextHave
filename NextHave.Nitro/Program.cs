using Blazored.Toast;
using Dolphin.Core.Configurations;
using Dolphin.Core.Injection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using NextHave.BL;
using NextHave.BL.Models.Configurations;
using NextHave.DAL.Mongo;
using NextHave.DAL.MySQL;
using NextHave.Nitro.Authentications;
using NextHave.Nitro.Components;
using NextHave.Nitro.Sockets;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureDolphinApplication<NextHaveConfiguration>("NextHave", "1");

builder.Services.AddBlazoredToast();
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.RegisterDolphinApplication();
builder.Services.AddSignalR().AddMessagePackProtocol();
builder.Services.AddDbContextFactory<MySQLDbContext>();
builder.Services.AddDbContextFactory<MongoDbContext>();
builder.Services.AddDbContext<DbContext, MySQLDbContext>();
builder.Services.AddDbContext<DbContext, MongoDbContext>();
builder.Services.AddNextHaveServices(Assembly.GetExecutingAssembly());
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddAuthentication("NextHaveAuth").AddCookie("NextHaveAuth", options =>
{
    options.Cookie.Name = "NextHave.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/access-denied";
});
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<NextHaveAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<NextHaveAuthenticationStateProvider>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.UseRouting();
app.UseCors("AllowFrontend");
app.MapHub<SocketHub>("/socket");
app.UseAntiforgery();

var services = app.Services.GetRequiredService<IEnumerable<IStartableService>>();
foreach (var service in services)
    await service.StartAsync();

await app.RunAsync();