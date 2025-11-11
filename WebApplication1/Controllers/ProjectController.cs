using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProjectController : Controller
    {
        private readonly GameDevContext _context;

        public ProjectController(GameDevContext context)
        {
            _context = context;
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

        [HttpPost]
        public IActionResult AssignTask(Models.Task task)
        {
            if (ModelState.IsValid)
            {
                _context.Tasks.Add(task);
                _context.SaveChanges();
                return RedirectToAction("ProjectDetails", new { id = task.ProjectId });
            }
            return View(task);
        }
    }
}