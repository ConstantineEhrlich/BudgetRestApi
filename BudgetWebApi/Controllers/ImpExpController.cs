using BudgetServices.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetWebApi.Controllers;

[ApiController]
[Route("budgets")]
public class ImpExpController : ControllerBase
{
    private readonly ILogger<ImpExpController> _logger;
    private readonly DataImportService _importService;
    private readonly DataExportService _exportService;

    public ImpExpController(ILogger<ImpExpController> logger, DataImportService importService, DataExportService exportService)
    {
        _logger = logger;
        _importService = importService;
        _exportService = exportService;
    }

    [HttpPost]
    [Authorize]
    [Route("{budgetId}/import")]
    public async Task<ActionResult<List<ImportResult>>> Import(string budgetId, [FromBody] ImportConfiguration configDto, CancellationToken cancellationToken)
    {
        string? requestingUser = User.Identity?.Name;
        
        if (configDto.BudgetId != budgetId)
            return BadRequest();

        if (requestingUser is null)
            return Unauthorized("User identity is not found");

        // Merge route ID and payload
        var config = new ImportConfiguration
        {
            BudgetId = budgetId,
            PathToFile = configDto.PathToFile,
            WorksheetName = configDto.WorksheetName
        };

        _logger.LogInformation("User {User} is importing data into budget {Budget}", requestingUser, budgetId);

        var result = await _importService.ImportData(config, requestingUser, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet]
    [Authorize]
    [Route("{budgetId}/export")]
    public async Task<ActionResult<ExportResult>> Export(string budgetId, CancellationToken cancellationToken)
    {
        string? requestingUser = User.Identity?.Name;
        
        if (requestingUser is null)
            return Unauthorized("User identity is not found");

        return await _exportService.ExportTransactions(budgetId, requestingUser);
    }
    
    
}