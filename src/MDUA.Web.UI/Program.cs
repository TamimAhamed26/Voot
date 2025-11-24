using MDUA.Facade;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.Cookies;
using MDUA.Framework; 
var builder = WebApplication.CreateBuilder(args);
ConfigurationBlock.Configuration = builder.Configuration;
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

builder.Services.AddService();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// -------------------- Middleware Pipeline --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// MUST be BEFORE routing (static handling)
app.UseStaticFiles();

// YOUR custom static folder
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images")
    ),
    RequestPath = "/products"
});

app.UseRouting();

// MUST be here only
app.UseAuthentication();
app.UseAuthorization();

// MVC Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
