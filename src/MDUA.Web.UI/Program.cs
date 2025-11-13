using MDUA.Facade;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.Cookies; // <-- 1. ADD THIS NAMESPACE

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 2. ADD AUTHENTICATION SERVICES
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // This is the path your app will redirect to when a user
        // tries to access an [Authorize] page without being logged in.
        options.LoginPath = "/Account/Login";

        // This is the path your app will redirect to when a user
        // is logged in but doesn't have the right permissions.
        options.AccessDeniedPath = "/Account/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Set cookie expiration
        options.SlidingExpiration = true;
    });

// This adds your custom services (IProductFacade, etc.)
builder.Services.AddService();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

// 3. ADD AUTHENTICATION MIDDLEWARE (in the correct order)
app.UseAuthentication();
app.UseAuthorization();


app.MapStaticAssets();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images")),
    RequestPath = "/products"
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();