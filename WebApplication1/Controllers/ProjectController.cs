using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Patterns.Observer;

namespace WebApplication1.Controllers
{
    public class ProjectController : Controller
    {
        private readonly GameDevContext _context;
        private readonly NotificationService _notification;

        public ProjectController(GameDevContext context)
        {
            _context = context;
            _notification = NotificationService.GetInstance();
        }

        [HttpGet]
        public IActionResult ProjectDetails(int id)
        {
            var project = _context.Projects.Find(id);
            ViewBag.Project = project;
            return View();
        }

        [HttpGet]
        public IActionResult AssignTask(int projectId)
        {
            Models.Task task = new Models.Task();
            task.ProjectId = projectId;
            ViewBag.Task = task;
            ViewBag.Employees = _context.Employees;
            return View();
        }

        //[HttpPost]
        //public IActionResult AssignTask(Models.Task task)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Tasks.Add(task);
        //        _context.SaveChanges();
        //        return RedirectToAction("ProjectDetails", new { id = task.ProjectId });
        //    }
        //    return View(task);
        //}
        [HttpPost]
        public IActionResult AssignTask(int projectId, IFormCollection form)
        {
            try
            {
                var project = _context.Projects.Find(projectId);

                var task = new Models.Task
                {
                    Description = form["Description"],
                    Priority = form["Priority"],
                    Status = form["Status"],
                    EstimatedTime = int.Parse(form["EstimatedTime"]),
                    ProjectId = projectId,
                    CreatedDate = DateTime.Now
                };

                _context.Tasks.Add(task);
                _context.SaveChanges();

                // УВЕДОМЛЕНИЕ: задача создана
                _notification.Notify("Task", "created", task.Description);

                TempData["SuccessMessage"] = $"Задача '{task.Description}' успешно создана!";
                return RedirectToAction("Projects", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Ошибка: {ex.Message}";
                return View();
            }
        }
    }
}