using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class GameConcept
{
    public int Id { get; set; }

    [Required]
    public string Genre { get; set; }

    public string TargetAudience { get; set; }
    public string CoreMechanics { get; set; }

    public int ProjectId { get; set; }
    public virtual Project Project { get; set; }
}
