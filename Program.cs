using NextHave.DAL.MySQL;
using NextHave.DAL.Mongo;
using NextHave.Components;
using Dolphin.Core.Injection;
using Dolphin.Core.Configurations;
using Dolphin.Core.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureDolphinApplication("Dolphin");

builder.Services.AddDbContextFactory<MySQLDbContext>();
builder.Services.AddDbContextFactory<MongoDbContext>();
builder.Services.AddDbContext<MySQLDBContext, MySQLDbContext>();
builder.Services.AddDbContext<MongoDBContext, MongoDbContext>();
builder.Services.RegisterDolphinApplication();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

await app.RunAsync();