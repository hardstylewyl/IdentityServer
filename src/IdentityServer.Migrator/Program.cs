using System.Reflection;
using IdentityServer.Infrastructure.DateTimes;
using IdentityServer.Infrastructure.Localization;
using IdentityServer.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDateTimeProvider();
builder.Services.AddIdentityServerLocalization();
builder.Services.AddPersistence("Server=127.0.0.1;Port=5432;Database=identityserver;Uid=postgres;Pwd=wyl123567;",
	Assembly.GetExecutingAssembly().GetName().Name!);
var app = builder.Build();
app.MigrateIdentityServerDb();
app.Run();
