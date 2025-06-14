using Dolphin.Core.Configurations;
using Dolphin.Core.Database;
using Dolphin.Core.Injection;
using NextHave;
using NextHave.Components;
using NextHave.DAL.Mongo;
using NextHave.DAL.MySQL;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureDolphinApplication("Dolphin", ProjectConstants.ProjectVersion);

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