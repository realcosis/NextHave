using NextHave.Parsers;
using NextHave.DAL.MySQL;
using NextHave.DAL.Mongo;
using NextHave.Components;
using Dolphin.Core.Injection;
using Dolphin.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfiguration("Dolphin");
builder.Services.AddDbContextFactory<MySQLDbContext>();
builder.Services.AddDbContextFactory<MongoDbContext>();
builder.Services.AddDolphinCore<MySQLDbContext, MongoDbContext>();
builder.Services.AddKeyedScoped<IParser, GameParser>("GameParser");
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

await app.RunAsync();