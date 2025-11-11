using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Task
{
    public int Id { get; set; }

    [Required]
    public string Description { get; set; }

    public string Priority { get; set; }
    public string Status { get; set; }
    public int EstimatedTime { get; set; }

    public int ProjectId { get; set; }
    public virtual Project Project { get; set; }

    public int? AssignedEmployeeId { get; set; }
    public virtual Employee AssignedEmployee { get; set; }
}