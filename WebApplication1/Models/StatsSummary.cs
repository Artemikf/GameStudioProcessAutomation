using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class StatsSummary
{
    public string Period { get; set; }
    public int ProjectsCount { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal AverageBudget { get; set; }
    public decimal MaxBudget { get; set; }
    public decimal MinBudget { get; set; }
    public string MostCommonStatus { get; set; }
}

public class StatsFilter
{
    [Display(Name = "Период")]
    public string PeriodType { get; set; } = "month"; // day, month, quarter, year

    [Display(Name = "Начальная дата")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);

    [Display(Name = "Конечная дата")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.Now;

    [Display(Name = "Статус проекта")]
    public string StatusFilter { get; set; } = "all";
}

