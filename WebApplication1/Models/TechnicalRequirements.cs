namespace WebApplication1.Models;

public class TechnicalRequirements
{
    public int Id { get; set; }

    public string Platforms { get; set; }
    public string MinimumRequirements { get; set; }
    public string RecommendedRequirements { get; set; }

    public int ProjectId { get; set; }
    public virtual Project Project { get; set; }
}
