using System.ComponentModel.DataAnnotations;
using BudgetServices.Reports;
using Microsoft.AspNetCore.Mvc;

namespace BudgetWebApi.Controllers;

[ApiController]
[Route("budgets")]
public class ReportsController : ControllerBase
{
    private readonly ILogger<ReportsController> _logger;
    private readonly IReportFactory _reportFactory;

    public ReportsController(ILogger<ReportsController> logger, IReportFactory reportFactory)
    {
        _logger = logger;
        _reportFactory = reportFactory;
    }

    [HttpGet]
    [Route("{budgetId}/SummaryByCategory")]
    public IActionResult SummaryByCategory(string budgetId,
                                           [FromQuery, Range(1900, 2999)] int? year,
                                           [FromQuery, Range(1,12)] int? period)
        => Ok(CreateReport<SummaryByPeriodAndCategory>(budgetId, year, period).Summarize());
    


    [HttpGet]
    [Route("{budgetId}/SummaryByOwner")]
    public IActionResult SummaryByOwner(string budgetId,
                                        [FromQuery, Range(1900, 2999)] int? year,
                                        [FromQuery, Range(1, 12)] int? period)
        => Ok(CreateReport<SummaryByPeriodAndOwner>(budgetId, year, period).Summarize());



    private IReportBuilder CreateReport<T>(string budget, int? year, int? period) where T: IReportBuilder, new()
    {
        _logger.LogInformation("Report {} requested for budget {}", typeof(T).Name, budget);
        string? requestingUser = User.Identity?.Name;
        IReportBuilder report = _reportFactory.CreateReport<T>(budget, requestingUser);
        if (year is not null && period is not null)
            report = report.ForPeriod(year.Value, period.Value);
        return report;
    }
    
    
}