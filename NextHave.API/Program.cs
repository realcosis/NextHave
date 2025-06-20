using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using NextHave.API;
using NextHave.API.Conf;
using NextHave.BL;
using NextHave.DAL.Mongo;
using NextHave.DAL.MySQL;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureDolphinApplication("NextHave", ProjectConstants.ProjectVersion);

builder.Services.AddDbContext<DbContext, MySQLDbContext>();
builder.Services.AddDbContext<DbContext, MongoDbContext>();
builder.Services.RegisterDolphinApplication();
builder.Services.RegisterAPIApplicationCustom(builder.Configuration);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

app.UseAPIApplicationCustom(app.Environment, builder.Configuration);

await app.RunAsync();