using Dolphin.Core.Configurations;
using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using NextHave;
using NextHave.Components;
using NextHave.DAL.Mongo;
using NextHave.DAL.MySQL;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureDolphinApplication("NextHave", ProjectConstants.ProjectVersion);

builder.Services.AddDbContext<DbContext, MySQLDbContext>();
builder.Services.AddDbContext<DbContext, MongoDbContext>();
builder.Services.RegisterDolphinApplication();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

var services = app.Services.GetRequiredService<IEnumerable<IStartableService>>();
foreach (var service in services)
    await service.StartAsync();

await app.RunAsync();