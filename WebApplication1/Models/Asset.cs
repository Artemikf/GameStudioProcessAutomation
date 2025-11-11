using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Asset
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    public string Type { get; set; }
    public double Size { get; set; }
    public string Version { get; set; }

    public int ProjectId { get; set; }
    public virtual Project Project { get; set; }
}
