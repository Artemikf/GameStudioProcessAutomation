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

        // Регистрация пользователя
        public bool Register(User user)
        {
            try
            {
                // Проверяем нет ли пользователя с таким именем или email
                if (_context.Users.Any(u => u.Username == user.Username || u.Email == user.Email))
                {
                    return false;
                }

                // Хешируем пароль
                user.Password = HashPassword(user.Password);
                user.CreatedAt = DateTime.Now;

                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
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