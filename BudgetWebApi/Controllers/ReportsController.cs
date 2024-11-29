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
    [Route("{budgetId}/[action]")]
    public IActionResult SummaryByCategory(string budgetId,
                                           [FromQuery, Range(1900, 2999)] int? year,
                                           [FromQuery, Range(1,12)] int? period)
        => Ok(CreateReport<SummaryByPeriodAndCategory>(budgetId, year, period, ForOnePeriod).Summarize());
    

    [HttpGet]
    [Route("{budgetId}/[action]")]
    public IActionResult SummaryByOwner(string budgetId,
                                        [FromQuery, Range(1900, 2999)] int? year,
                                        [FromQuery, Range(1, 12)] int? period)
        => Ok(CreateReport<BalanceToPeriodReport>(budgetId, year, period, ForOnePeriod).Summarize());

    [HttpGet]
    [Route("{budgetId}/[action]")]
    public IActionResult GetClosingBalance(string budgetId,
                                           [FromQuery, Range(1900, 2999), Required] int year,
                                           [FromQuery, Range(1, 12), Required] int period)
        => Ok(CreateReport<BalanceToPeriodReport>(budgetId, year, period, UpToPeriod).Summarize());


    [HttpGet]
    [Route("{budgetId}/[action]")]
    public IActionResult GetOpeningBalance(string budgetId,
                                           [FromQuery, Range(1900, 2999), Required] int year,
                                           [FromQuery, Range(1, 12), Required] int period)
    {
        // Calculate previous period
        if (period == 1)
            year -= 1;
        else
            period -= 1;
        
        return Ok(CreateReport<BalanceToPeriodReport>(budgetId, year, period, UpToPeriod).Summarize());    
    }
    

    private IReportBuilder CreateReport<T>(string budget,
                                           int? year,
                                           int? period,
                                           Func<IReportBuilder, int, int, IReportBuilder> periodFunc)
                                           where T: IReportBuilder, new()
    {
        _logger.LogInformation("Report {} requested for budget {}", typeof(T).Name, budget);
        string? requestingUser = User.Identity?.Name;
        IReportBuilder report = _reportFactory.CreateReport<T>(budget, requestingUser);
        if (year is not null && period is not null)
            report = periodFunc(report, year.Value, period.Value);
        return report;
    }

    private static IReportBuilder ForOnePeriod(IReportBuilder report, int year, int period)
        => report.ForPeriod(year, period);
    
    private static IReportBuilder UpToPeriod(IReportBuilder report, int year, int period)
        => report.ToPeriod(year, period);
}