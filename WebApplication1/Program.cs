using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Filters;
using WebApplication1.Models;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавьте сессии
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDevContext>();
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

    try
    {
        // Убедимся, что база данных создана
        context.Database.EnsureCreated();

        // Создание администратора по умолчанию
        if (!context.Users.Any())
        {
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@company.com",
                Password = "admin123",
                Role = "Admin"
            };

            authService.Register(adminUser);
            Console.WriteLine("Администратор создан: admin / admin123");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при инициализации БД: {ex.Message}");
    }
}

// МАРШРУТИЗАЦІЯ
app.MapCustomRoutes();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();