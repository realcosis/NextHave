using Blazored.Toast;
using Dolphin.Core.Configurations;
using Dolphin.Core.Injection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.RateLimiting;
using NextHave.BL;
using NextHave.BL.Models.Configurations;
using NextHave.DAL.Mongo;
using NextHave.DAL.MySQL;
using NextHave.Nitro.Authentications;
using NextHave.Nitro.Components;
using NextHave.Nitro.Sockets;
using System.Reflection;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ApplicationName = "NextHave.Nitro"
});

builder.Host.ConfigureDolphinApplication<NextHaveConfiguration>("NextHave", "1");

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("login", options =>
    {
        options.Window = TimeSpan.FromMinutes(1);
        options.PermitLimit = 5;
    });
});
builder.Services.AddBlazoredToast();
builder.Services.AddHttpContextAccessor();
builder.Services.RegisterDolphinApplication();
builder.Services.AddSignalR().AddMessagePackProtocol();
builder.Services.AddDbContext<MySQLDbContext>(ServiceLifetime.Singleton);
builder.Services.AddDbContext<MongoDbContext>(ServiceLifetime.Singleton);
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
builder.Services.AddScoped<NextHaveAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<NextHaveAuthenticationStateProvider>());

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

app.UseStaticFiles();
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