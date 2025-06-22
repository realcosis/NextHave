using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using NextHave.DAL.Mongo;
using NextHave.DAL.MySQL;
using Dolphin.Core.API;
using Dolphin.Core.Configurations;
using NextHave.BL.Models.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureDolphinApplication<NextHaveConfiguration>("NextHave", "1");

builder.Services.AddDbContext<DbContext, MySQLDbContext>();
builder.Services.AddDbContext<DbContext, MongoDbContext>();
builder.Services.RegisterDolphinApplication();
builder.Services.RegisterAPIApplication(builder.Configuration);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.UseAPIApplication(app.Environment, builder.Configuration);

await app.RunAsync();