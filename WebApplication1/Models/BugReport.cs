using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class BugReport
{
    public int Id { get; set; }

    [Required]
    public string BugDescription { get; set; }

    public string Priority { get; set; }
    public string Status { get; set; }

    public int ProjectId { get; set; }
    public virtual Project Project { get; set; }

    public int? TesterId { get; set; }
    public virtual Tester Tester { get; set; }
}
