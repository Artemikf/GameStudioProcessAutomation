using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication1.Filters;
using WebApplication1.Models;

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


        // Get Create
        [TypeFilter(typeof(AuthAuthorizationFilter))]
        [HttpGet]
        public IActionResult CreateProject()
        {
            return View();
        }

        // Post Create
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

        // GET: Projects с фильтрацией и сортировкой
        public IActionResult Projects(string statusFilter, string sortOrder, string searchString)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["BudgetSortParam"] = sortOrder == "budget_asc" ? "budget_desc" : "budget_asc";
            ViewData["DateSortParam"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["StatusFilter"] = statusFilter;
            ViewData["SearchString"] = searchString;

            var projects = _context.Projects.AsQueryable();

            // ФИЛЬТРАЦИЯ по статусу
            if (!string.IsNullOrEmpty(statusFilter))
            {
                projects = projects.Where(p => p.Status == statusFilter);
            }

            // ПОИСК по названию
            if (!string.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(p => p.Name.Contains(searchString));
            }

            // СОРТИРОВКА
            projects = sortOrder switch
            {
                "name_desc" => projects.OrderByDescending(p => p.Name),
                "budget_asc" => projects.OrderBy(p => p.Budget),
                "budget_desc" => projects.OrderByDescending(p => p.Budget),
                "date_asc" => projects.OrderBy(p => p.Deadline),
                "date_desc" => projects.OrderByDescending(p => p.Deadline),
                _ => projects.OrderBy(p => p.Name)
            };

            var projectList = projects.ToList();

            // Передаем список статусов для фильтра
            ViewBag.Statuses = _context.Projects.Select(p => p.Status).Distinct().ToList();

            return View(projectList);
        }

        // GET: Edit Project
        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpGet]
        public IActionResult EditProject(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Edit Project
        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpPost]
        public IActionResult EditProject(int id, IFormCollection form)
        {
            try
            {
                Debug.WriteLine($"=== EDIT PROJECT STARTED ===");
                Debug.WriteLine($"Project ID: {id}");
                Debug.WriteLine($"Form data received:");
                foreach (var key in form.Keys)
                {
                    Debug.WriteLine($"{key}: {form[key]}");
                }

                // Находим проект по ID
                var project = _context.Projects.Find(id);
                if (project == null)
                {
                    Debug.WriteLine($"PROJECT WITH ID {id} NOT FOUND IN DATABASE!");
                    ViewBag.Error = $"Проект с ID {id} не найден!";
                    return View();
                }

                Debug.WriteLine($"Found project: {project.Name} (ID: {project.Id})");

                // Обновляем данные
                project.Name = form["Name"];
                project.Budget = decimal.Parse(form["Budget"]);
                project.Deadline = DateTime.Parse(form["Deadline"]);
                project.Status = form["Status"];

                Debug.WriteLine($"Updated project: {project.Name}, Budget: {project.Budget}, Deadline: {project.Deadline}, Status: {project.Status}");

                // Сохраняем изменения
                _context.Projects.Update(project);
                int changes = _context.SaveChanges();

                Debug.WriteLine($"Database changes saved. Rows affected: {changes}");
                Debug.WriteLine($"=== EDIT PROJECT COMPLETED ===");

                TempData["SuccessMessage"] = $"Проект '{project.Name}' успешно обновлен!";
                return RedirectToAction("Projects");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"=== EDIT PROJECT ERROR ===");
                Debug.WriteLine($"Error: {ex.Message}");
                Debug.WriteLine($"Stack: {ex.StackTrace}");
                Debug.WriteLine($"=== EDIT PROJECT ERROR END ===");

                ViewBag.Error = $"Ошибка обновления: {ex.Message}";

                // Создаем модель для возврата в форму
                var project = new Project
                {
                    Id = id,
                    Name = form["Name"],
                    Budget = decimal.TryParse(form["Budget"], out decimal budget) ? budget : 0,
                    Deadline = DateTime.TryParse(form["Deadline"], out DateTime deadline) ? deadline : DateTime.Now,
                    Status = form["Status"]
                };
                return View(project);
            }
        }

        // GET: Delete Project
        [TypeFilter(typeof(AdminAuthorizationFilter))]
        [HttpGet]
        public IActionResult DeleteProject(int id)
        {
            var project = _context.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Delete Project
        [HttpPost]
        [ActionName("DeleteProject")]
        public IActionResult DeleteProjectConfirmed(int id)
        {
            try
            {
                var project = _context.Projects.Find(id);
                if (project != null)
                {
                    _context.Projects.Remove(project);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = $"Проект '{project.Name}' успешно удален!";
                }
                return RedirectToAction("Projects");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка удаления: {ex.Message}";
                return RedirectToAction("Projects");
            }
        }

        // GET: Project Details
        public IActionResult ProjectDetails(int id)
        {
            var project = _context.Projects
                .Include(p => p.Tasks)
                .Include(p => p.Teams)
                .FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // Quick Status Update (быстрое изменение статуса)
        [HttpPost]
        public IActionResult UpdateProjectStatus(int id, string status)
        {
            try
            {
                var project = _context.Projects.Find(id);
                if (project != null)
                {
                    project.Status = status;
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = $"Статус проекта '{project.Name}' изменен на '{status}'!";
                }
                return RedirectToAction("Projects");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка обновления статуса: {ex.Message}";
                return RedirectToAction("Projects");
            }
        }

        // GET: Assign Task to Project
        [HttpGet]
        public IActionResult AssignTask(int projectId)
        {
            try
            {
                Debug.WriteLine($"=== ASSIGN TASK GET STARTED ===");
                Debug.WriteLine($"Project ID: {projectId}");

                var project = _context.Projects.Find(projectId);
                if (project == null)
                {
                    Debug.WriteLine($"PROJECT WITH ID {projectId} NOT FOUND!");
                    return NotFound();
                }

                Debug.WriteLine($"Found project: {project.Name} (ID: {project.Id})");

                ViewBag.Project = project;
                ViewBag.ProjectId = projectId;

                // Получаем список сотрудников для dropdown
                ViewBag.Employees = _context.Employees.ToList();

                Debug.WriteLine($"=== ASSIGN TASK GET COMPLETED ===");
                return View();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ASSIGN TASK GET ERROR: {ex.Message}");
                ViewBag.Error = $"Ошибка: {ex.Message}";
                return View();
            }
        }

        // POST: Assign Task to Project
        [HttpPost]
        public IActionResult AssignTask(int projectId, IFormCollection form)
        {
            try
            {
                Debug.WriteLine($"=== ASSIGN TASK POST STARTED ===");
                Debug.WriteLine($"Project ID: {projectId}");
                Debug.WriteLine($"Form data received:");
                foreach (var key in form.Keys)
                {
                    Debug.WriteLine($"{key}: {form[key]}");
                }

                // Проверяем что проект существует
                var project = _context.Projects.Find(projectId);
                if (project == null)
                {
                    Debug.WriteLine($"PROJECT WITH ID {projectId} NOT FOUND IN DATABASE!");
                    ViewBag.Error = $"Проект с ID {projectId} не найден!";
                    ViewBag.ProjectId = projectId;
                    return View();
                }

                Debug.WriteLine($"Project found: {project.Name}");

                // Создаем новую задачу
                var task = new Models.Task
                {
                    Description = form["Description"],
                    Priority = form["Priority"],
                    Status = form["Status"],
                    EstimatedTime = int.Parse(form["EstimatedTime"]),
                    ProjectId = projectId,
                    //CreatedDate = DateTime.Now
                };

                // Обрабатываем назначенного сотрудника (если есть)
                if (!string.IsNullOrEmpty(form["AssignedEmployeeId"]) && int.TryParse(form["AssignedEmployeeId"], out int employeeId))
                {
                    var employee = _context.Employees.Find(employeeId);
                    if (employee != null)
                    {
                        task.AssignedEmployeeId = employeeId;
                        Debug.WriteLine($"Task assigned to employee: {employee.FirstName} {employee.LastName}");
                    }
                }

                Debug.WriteLine($"Creating task: {task.Description}, Priority: {task.Priority}, Status: {task.Status}, Estimated: {task.EstimatedTime}h");

                // Сохраняем в базу
                _context.Tasks.Add(task);
                int changes = _context.SaveChanges();

                Debug.WriteLine($"Task saved to database. Changes: {changes}, Task ID: {task.Id}");
                Debug.WriteLine($"=== ASSIGN TASK POST COMPLETED ===");

                TempData["SuccessMessage"] = $"Задача '{task.Description}' успешно создана в проекте '{project.Name}'!";
                return RedirectToAction("Projects");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"=== ASSIGN TASK POST ERROR ===");
                Debug.WriteLine($"Error: {ex.Message}");
                Debug.WriteLine($"Stack: {ex.StackTrace}");
                Debug.WriteLine($"=== ASSIGN TASK POST ERROR END ===");

                ViewBag.Error = $"Ошибка создания задачи: {ex.Message}";
                ViewBag.ProjectId = projectId;
                ViewBag.Employees = _context.Employees.ToList();
                return View();
            }
        }

        // -------------

        // GET: Project Statistics
        [HttpGet]
        public IActionResult ProjectStatistics()
        {
            var model = new StatsFilter
            {
                StartDate = DateTime.Now.AddMonths(-1),
                EndDate = DateTime.Now,
                PeriodType = "month",
                StatusFilter = "all"
            };

            ViewBag.Statuses = _context.Projects.Select(p => p.Status).Distinct().ToList();
            return View(model);
        }

        // POST: Project Statistics with filtering
        [HttpPost]
        public IActionResult ProjectStatistics(StatsFilter filter)
        {
            try
            {
                Debug.WriteLine($"=== STATISTICS CALCULATION STARTED ===");
                Debug.WriteLine($"Period: {filter.PeriodType}, From: {filter.StartDate}, To: {filter.EndDate}, Status: {filter.StatusFilter}");

                // Базовый запрос с фильтрацией по дате
                var projectsQuery = _context.Projects
                    .Where(p => p.Deadline >= filter.StartDate && p.Deadline <= filter.EndDate);

                // Дополнительная фильтрация по статусу
                if (filter.StatusFilter != "all")
                {
                    projectsQuery = projectsQuery.Where(p => p.Status == filter.StatusFilter);
                }

                var projects = projectsQuery.ToList();
                Debug.WriteLine($"Found {projects.Count} projects for statistics");

                // Расчет статистики
                var stats = new StatsSummary
                {
                    ProjectsCount = projects.Count,
                    TotalBudget = projects.Sum(p => p.Budget),
                    AverageBudget = projects.Any() ? projects.Average(p => p.Budget) : 0,
                    MaxBudget = projects.Any() ? projects.Max(p => p.Budget) : 0,
                    MinBudget = projects.Any() ? projects.Min(p => p.Budget) : 0,
                    MostCommonStatus = projects.Any() ?
                        projects.GroupBy(p => p.Status)
                               .OrderByDescending(g => g.Count())
                               .First().Key : "Нет данных"
                };

                // Определяем период для отображения
                stats.Period = filter.PeriodType switch
                {
                    "day" => $"День: {filter.StartDate:dd.MM.yyyy}",
                    "month" => $"Месяц: {filter.StartDate:MMMM yyyy}",
                    "quarter" => $"Квартал: {GetQuarter(filter.StartDate)}",
                    "year" => $"Год: {filter.StartDate:yyyy}",
                    "custom" => $"Период: {filter.StartDate:dd.MM.yyyy} - {filter.EndDate:dd.MM.yyyy}",
                    _ => "Неизвестный период"
                };

                Debug.WriteLine($"Statistics calculated: {stats.ProjectsCount} projects, Total: {stats.TotalBudget:C}");
                Debug.WriteLine($"=== STATISTICS CALCULATION COMPLETED ===");

                ViewBag.Stats = stats;
                ViewBag.Projects = projects;
                ViewBag.Statuses = _context.Projects.Select(p => p.Status).Distinct().ToList();
                return View(filter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"=== STATISTICS ERROR ===");
                Debug.WriteLine($"Error: {ex.Message}");
                Debug.WriteLine($"Stack: {ex.StackTrace}");

                ViewBag.Error = $"Ошибка расчета статистики: {ex.Message}";
                ViewBag.Statuses = _context.Projects.Select(p => p.Status).Distinct().ToList();
                return View(filter);
            }
        }

        // Вспомогательный метод для определения квартала
        private string GetQuarter(DateTime date)
        {
            int quarter = (date.Month - 1) / 3 + 1;
            return $"{quarter} квартал {date:yyyy}";
        }

        // GET: Quick Statistics (быстрая статистика)
        public IActionResult QuickStats(string period = "month")
        {
            var endDate = DateTime.Now;
            var startDate = period switch
            {
                "day" => endDate.AddDays(-1),
                "week" => endDate.AddDays(-7),
                "month" => endDate.AddMonths(-1),
                "quarter" => endDate.AddMonths(-3),
                "year" => endDate.AddYears(-1),
                _ => endDate.AddMonths(-1)
            };

            var projects = _context.Projects
                .Where(p => p.Deadline >= startDate && p.Deadline <= endDate)
                .ToList();

            var stats = new StatsSummary
            {
                Period = period switch
                {
                    "day" => "За сегодня",
                    "week" => "За неделю",
                    "month" => "За месяц",
                    "quarter" => "За квартал",
                    "year" => "За год",
                    _ => "За месяц"
                },
                ProjectsCount = projects.Count,
                TotalBudget = projects.Sum(p => p.Budget),
                AverageBudget = projects.Any() ? projects.Average(p => p.Budget) : 0,
                MaxBudget = projects.Any() ? projects.Max(p => p.Budget) : 0,
                MinBudget = projects.Any() ? projects.Min(p => p.Budget) : 0
            };

            return View(stats);
        }
    }
}