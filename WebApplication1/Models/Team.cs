using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Team
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string TeamName { get; set; }

    public int ProjectId { get; set; }
    public virtual Project Project { get; set; }
    public virtual ICollection<Employee> Employees { get; set; }
}
