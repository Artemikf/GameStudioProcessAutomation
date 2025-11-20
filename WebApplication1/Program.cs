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

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<GameDevContext>();
//    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

//    try
//    {
//        Console.WriteLine("Проверка базы данных...");

//        // Убедимся, что база данных создана
//        context.Database.EnsureCreated();
//        Console.WriteLine("База данных проверена/создана");

//        // Создание администратора по умолчанию
//        if (!context.Users.Any())
//        {
//            Console.WriteLine("Создание администратора...");
//            var adminUser = new User
//            {
//                Username = "admin",
//                Email = "admin@company.com",
//                Password = "admin123",
//                Role = "Admin"
//            };

//            var success = authService.Register(adminUser);
//            Console.WriteLine($"Администратор создан: {success}");
//        }
//        else
//        {
//            Console.WriteLine("Пользователи уже существуют в базе данных");
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Ошибка при инициализации БД: {ex.Message}");
//        Console.WriteLine($"StackTrace: {ex.StackTrace}");
//    }
//}

// МАРШРУТИЗАЦІЯ
app.MapCustomRoutes();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();