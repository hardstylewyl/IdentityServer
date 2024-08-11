using System.Reflection;
using IdentityServer.Domain.Notification;
using IdentityServer.Infrastructure.DateTimes;
using IdentityServer.Infrastructure.Identity;
using IdentityServer.Infrastructure.Localization;
using IdentityServer.Mvc.Services;
using IdentityServer.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddDateTimeProvider();
builder.Services.AddPersistence("Server=127.0.0.1;Port=5432;Database=identityserver;Uid=postgres;Pwd=wyl123567;",
	Assembly.GetExecutingAssembly().GetName().Name!);
builder.Services.AddIdentityServerLocalization();
builder.Services.AddIdentity();

#region Fake

builder.Services.AddScoped<ISmsNotification, FakeNoticeService>();
builder.Services.AddScoped<IEmailNotification, FakeNoticeService>();

#endregion Fake

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
