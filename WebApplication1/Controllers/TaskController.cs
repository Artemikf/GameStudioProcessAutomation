using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Filters;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class TaskController : Controller
    {
        private readonly GameDevContext _context;

        public TaskController(GameDevContext context)
        {
            _context = context;
        }

        // GET: Все задачи с фильтрацией
        public IActionResult Index(string statusFilter, string priorityFilter, string searchString, int? projectId)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["StatusFilter"] = statusFilter;
            ViewData["PriorityFilter"] = priorityFilter;
            ViewData["ProjectId"] = projectId;

            var tasks = _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedEmployee)
                .AsQueryable();

            // Фильтрация по проекту
            if (projectId.HasValue)
            {
                tasks = tasks.Where(t => t.ProjectId == projectId.Value);
                ViewBag.Project = _context.Projects.Find(projectId.Value);
            }

            // Поиск по описанию
            if (!string.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(t => t.Description.Contains(searchString));
            }

            // Фильтрация по статусу
            if (!string.IsNullOrEmpty(statusFilter))
            {
                tasks = tasks.Where(t => t.Status == statusFilter);
            }

            // Фильтрация по приоритету
            if (!string.IsNullOrEmpty(priorityFilter))
            {
                tasks = tasks.Where(t => t.Priority == priorityFilter);
            }

            ViewBag.Statuses = _context.Tasks.Select(t => t.Status).Distinct().ToList();
            ViewBag.Priorities = _context.Tasks.Select(t => t.Priority).Distinct().ToList();
            ViewBag.Projects = _context.Projects.ToList();
            ViewBag.Employees = _context.Employees.ToList();

            return View(tasks.ToList());
        }

        // GET: Детали задачи
        public IActionResult Details(int id)
        {
            var task = _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedEmployee)
                .FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Создание задачи
        [TypeFilter(typeof(AuthAuthorizationFilter))]
        [HttpGet]
        public IActionResult Create(int? projectId)
        {
            ViewBag.Projects = _context.Projects.ToList();
            ViewBag.Employees = _context.Employees.ToList();

            var task = new Models.Task();
            if (projectId.HasValue)
            {
                task.ProjectId = projectId.Value;
            }

            return View(task);
        }

        // POST: Создание задачи
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Models.Task task)
        {
            if (ModelState.IsValid)
            {
                task.CreatedDate = DateTime.Now;
                _context.Tasks.Add(task);
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Задача '{task.Description}' успешно создана!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Projects = _context.Projects.ToList();
            ViewBag.Employees = _context.Employees.ToList();
            return View(task);
        }

        // GET: Редактирование задачи
        [TypeFilter(typeof(AuthAuthorizationFilter))]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null)
            {
                return NotFound();
            }

            ViewBag.Projects = _context.Projects.ToList();
            ViewBag.Employees = _context.Employees.ToList();
            return View(task);
        }

        // POST: Редактирование задачи
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Models.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Tasks.Update(task);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = $"Задача '{task.Description}' успешно обновлена!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Projects = _context.Projects.ToList();
            ViewBag.Employees = _context.Employees.ToList();
            return View(task);
        }

        // GET: Удаление задачи
        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var task = _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedEmployee)
                .FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Удаление задачи
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Задача '{task.Description}' успешно удалена!";
            }
            return RedirectToAction(nameof(Index));
        }

        // Быстрое обновление статуса
        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var task = _context.Tasks.Find(id);
            if (task != null)
            {
                task.Status = status;
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Статус задачи изменен на '{status}'!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}