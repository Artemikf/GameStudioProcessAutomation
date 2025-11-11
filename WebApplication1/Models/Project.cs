using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Project
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название проекта обязательно")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Бюджет обязателен")]
    [Range(0, double.MaxValue, ErrorMessage = "Бюджет должен быть положительным")]
    public decimal Budget { get; set; }

    [Required(ErrorMessage = "Дедлайн обязателен")]
    public DateTime Deadline { get; set; }

    [Required(ErrorMessage = "Статус обязателен")]
    public string Status { get; set; }

    // СВЯЗИ 
    public virtual ICollection<Team> Teams { get; set; }
    public virtual ICollection<Task> Tasks { get; set; }
    public virtual GameConcept GameConcept { get; set; }
    public virtual TechnicalRequirements TechnicalRequirements { get; set; }
    public virtual ICollection<Asset> Assets { get; set; }
    public virtual ICollection<BugReport> BugReports { get; set; }
}