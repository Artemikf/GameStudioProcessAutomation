using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Task
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Описание задачи обязательно")]
    [Display(Name = "Описание задачи")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Приоритет обязателен")]
    [Display(Name = "Приоритет")]
    public string Priority { get; set; } = "Medium";

    [Required(ErrorMessage = "Статус обязателен")]
    [Display(Name = "Статус")]
    public string Status { get; set; } = "New";

    [Required(ErrorMessage = "Оценочное время обязательно")]
    [Range(1, int.MaxValue, ErrorMessage = "Время должно быть положительным")]
    [Display(Name = "Оценочное время (часы)")]
    public int EstimatedTime { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Связи
    public int ProjectId { get; set; }
    public virtual Project Project { get; set; }

    public int? AssignedEmployeeId { get; set; }
    public virtual Employee AssignedEmployee { get; set; }
}