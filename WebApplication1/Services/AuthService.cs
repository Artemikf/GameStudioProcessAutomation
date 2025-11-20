using Microsoft.AspNetCore.Http;
using WebApplication1.Models;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Services
{
    public interface IAuthService
    {
        bool Register(User user);
        User Login(string username, string password);
        void Logout();
        bool IsAuthenticated();
        bool IsAdmin();
        string GetCurrentUsername();
    }

    public class AuthService : IAuthService
    {
        private readonly GameDevContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(GameDevContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Хеширование пароля
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public bool Register(User user)
{
    try
    {
        Console.WriteLine($"=== РЕГИСТРАЦИЯ НАЧАТА ===");
        Console.WriteLine($"Username: {user.Username}");
        Console.WriteLine($"Email: {user.Email}");
        Console.WriteLine($"Role: {user.Role}");

        // Проверяем нет ли пользователя с таким именем или email
        var userExists = _context.Users.Any(u => u.Username == user.Username || u.Email == user.Email);
        Console.WriteLine($"Пользователь уже существует: {userExists}");

        if (userExists)
        {
            return false;
        }

        // Хешируем пароль
        var originalPassword = user.Password;
        user.Password = HashPassword(user.Password);
        user.CreatedAt = DateTime.Now;

        Console.WriteLine($"Пароль захеширован: {originalPassword} -> {user.Password}");

        _context.Users.Add(user);
        var result = _context.SaveChanges();
        
        Console.WriteLine($"=== РЕГИСТРАЦИЯ УСПЕШНА ===");
        Console.WriteLine($"Сохранено записей: {result}");
        Console.WriteLine($"ID нового пользователя: {user.Id}");
        
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"=== ОШИБКА РЕГИСТРАЦИИ ===");
        Console.WriteLine($"Сообщение: {ex.Message}");
        Console.WriteLine($"Тип исключения: {ex.GetType()}");
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
        
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Внутренняя ошибка: {ex.InnerException.Message}");
        }
        
        return false;
    }
}

        // Авторизация пользователя
        public User Login(string username, string password)
        {
            var hashedPassword = HashPassword(password);
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);

            if (user != null)
            {
                // Обновляем время последнего входа
                user.LastLogin = DateTime.Now;
                _context.SaveChanges();

                // Сохраняем в сессии
                _httpContextAccessor.HttpContext.Session.SetString("UserId", user.Id.ToString());
                _httpContextAccessor.HttpContext.Session.SetString("Username", user.Username);
                _httpContextAccessor.HttpContext.Session.SetString("Role", user.Role);
            }

            return user;
        }

        // Выход
        public void Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
        }

        // Проверка аутентификации
        public bool IsAuthenticated()
        {
            return !string.IsNullOrEmpty(_httpContextAccessor.HttpContext.Session.GetString("UserId"));
        }

        // Проверка роли администратора
        public bool IsAdmin()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("Role") == "Admin";
        }

        // Получение текущего пользователя
        public string GetCurrentUsername()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("Username") ?? "Гость";
        }
    }
}