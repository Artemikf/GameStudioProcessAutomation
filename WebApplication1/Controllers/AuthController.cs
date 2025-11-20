using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            if (_authService.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _authService.Login(model.Username, model.Password);
                if (user != null)
                {
                    TempData["SuccessMessage"] = $"Добро пожаловать, {user.Username}!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Неверное имя пользователя или пароль");
                }
            }
            return View(model);
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            if (_authService.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult Register(RegisterModel model)
        {
            Console.OutputEncoding = System.Text.Encoding.Default;

            Console.WriteLine("=== REGISTER CONTROLLER ===");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            if (ModelState.IsValid)
            {
                Console.WriteLine("Model is valid. Attempting to register...");
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    Role = model.Role
                };

                if (_authService.Register(user))
                {
                    Console.WriteLine("Register returned true. Redirecting to login...");
                    TempData["SuccessMessage"] = "Регистрация прошла успешно! Теперь вы можете войти.";
                    return RedirectToAction("Login");
                }
                else
                {
                    Console.WriteLine("Register returned false. User might already exist.");
                    ModelState.AddModelError("", "Пользователь с таким именем или email уже существует");
                }
            }
            else
            {
                Console.WriteLine("Model is not valid. Errors:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($" - {error.ErrorMessage}");
                }
            }
            return View(model);
        }

        // POST: Logout
        [HttpPost]
        public IActionResult Logout()
        {
            _authService.Logout();
            TempData["SuccessMessage"] = "Вы успешно вышли из системы";
            return RedirectToAction("Index", "Home");
        }

        // User Profile
        public IActionResult Profile()
        {
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Login");
            }

            ViewBag.Username = _authService.GetCurrentUsername();
            ViewBag.IsAdmin = _authService.IsAdmin();
            return View();
        }
    }
}