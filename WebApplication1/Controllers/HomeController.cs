using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Diagnostics;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly GameDevContext _context;

        public HomeController(GameDevContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Projects()
        {
            var projects = _context.Projects.ToList();
            return View(projects);
        }

        [HttpGet]
        public IActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateProject(IFormCollection form)
        {
            try
            {
                // логируем полученные данные
                Debug.WriteLine("=== DATA RECEIVED FROM FORM ===");
                foreach (var key in form.Keys)
                {
                    Debug.WriteLine($"{key}: {form[key]}");
                }

                // парсим данные из формы
                var name = form["Name"];
                var budget = decimal.Parse(form["Budget"]);
                var deadline = DateTime.Parse(form["Deadline"]);
                var status = form["Status"];

                // создаем новый проект
                var project = new Project
                {
                    Name = name,
                    Budget = budget,
                    Deadline = deadline,
                    Status = status
                };

                // сохраняем в бд
                _context.Projects.Add(project);
                _context.SaveChanges();

                Debug.WriteLine($"project saves: {project.Name}, ID: {project.Id}");

                // перенаправляю на список проектов
                return RedirectToAction("Projects");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"error: {ex.Message}");
                Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                ViewBag.Error = $"error: {ex.Message}";
                return View();
            }
        }

        //  отладкa посмотреть что в бд
        public IActionResult DebugDb()
        {
            var projects = _context.Projects.ToList();
            ViewBag.Projects = projects;
            ViewBag.Count = projects.Count;
            return View();
        }
    }
}