using Blazored.Toast;
using Dolphin.Core.Configurations;
using Dolphin.Core.Injection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
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
builder.Services.AddHttpContextAccessor();
builder.Services.RegisterDolphinApplication();
builder.Services.AddSignalR().AddMessagePackProtocol();
builder.Services.AddDbContext<MySQLDbContext>();
builder.Services.AddDbContext<MongoDbContext>();
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

StaticWebAssetsLoader.UseStaticWebAssets(app.Environment, app.Configuration);
await app.RunAsync();