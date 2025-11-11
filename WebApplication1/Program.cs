using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Додаємо сервіси MVC
builder.Services.AddControllersWithViews();

// Додаємо DbContext
builder.Services.AddDbContext<GameDevContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseStaticFiles(); // css


// для тестовых данных
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDevContext>();

    // cоздаем бд если не существует
    context.Database.EnsureCreated();

    //  таблица пустая
    if (!context.Projects.Any())
    {
        context.Projects.AddRange(
            new Project
            {
                Name = "test",
                Budget = 100000,
                Deadline = DateTime.Now.AddMonths(6),
                Status = "Development"
            },
            new Project
            {
                Name = "test_2",
                Budget = 90000,
                Deadline = DateTime.Now.AddMonths(3),
                Status = "Development"
            }
        );
        context.SaveChanges();
        Console.WriteLine("Добавлены тестовые проекты");
    }
}


// Налаштування HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

// МАРШРУТИЗАЦІЯ
app.MapCustomRoutes();

app.Run();