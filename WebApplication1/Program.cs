using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Filters;
using WebApplication1.Models;
using WebApplication1.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// Добавление сессии
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax; 
});

//// Настройка Anti-Forgery Token
//builder.Services.AddAntiforgery(options =>
//{
//    options.HeaderName = "X-XSRF-TOKEN";
//    options.Cookie.Name = "XSRF-TOKEN";
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    options.Cookie.SameSite = SameSiteMode.Lax;
//});

builder.Services.AddHttpContextAccessor();

// Регистрация сервисов
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AdminAuthorizationFilter>();
builder.Services.AddScoped<AuthAuthorizationFilter>();

// Додаємо сервіси MVC
builder.Services.AddControllersWithViews();



// Додаємо DbContext
builder.Services.AddDbContext<GameDevContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSession();

app.UseStaticFiles(); // css 

// Налаштування HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

// МАРШРУТИЗАЦІЯ
app.MapCustomRoutes();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "tasks",
    pattern: "Task/{action=Index}/{id?}",
    defaults: new { controller = "Task" });


app.Run();