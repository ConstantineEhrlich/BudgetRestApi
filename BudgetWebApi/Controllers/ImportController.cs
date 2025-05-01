using BudgetServices.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetWebApi.Controllers;

[ApiController]
[Route("budgets")]
public class ImportController : ControllerBase
{
    private readonly ILogger<ImportController> _logger;
    private readonly DataImportService _importService;

    public ImportController(ILogger<ImportController> logger, DataImportService importService)
    {
        _logger = logger;
        _importService = importService;
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
}