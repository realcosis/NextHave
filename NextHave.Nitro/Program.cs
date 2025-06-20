using Dolphin.Core.Configurations;
using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using NextHave.DAL.Mongo;
using NextHave.BL;
using NextHave.DAL.MySQL;
using NextHave.Nitro.Components;
using System.Reflection;
using NextHave.Nitro.Sockets;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureDolphinApplication("NextHave", "1");

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
builder.Services.AddSignalR().AddMessagePackProtocol();
builder.Services.AddDbContextFactory<MySQLDbContext>();
builder.Services.AddDbContext<DbContext, MySQLDbContext>();
builder.Services.AddDbContext<DbContext, MongoDbContext>();
builder.Services.RegisterDolphinApplication();
builder.Services.AddNextHaveServices(Assembly.GetExecutingAssembly());
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.MapHub<SocketHub>("/socket");
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

var services = app.Services.GetRequiredService<IEnumerable<IStartableService>>();
foreach (var service in services)
    await service.StartAsync();

await app.RunAsync();