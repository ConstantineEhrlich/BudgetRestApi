using BudgetServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BudgetWebApi.Dto;

namespace BudgetWebApi.Controllers;

[ApiController]
[Route("budgets")]
public class BudgetFileController : ControllerBase
{
    private readonly ILogger<BudgetFileController> _logger;
    private readonly BudgetFileService _budgetFileService;

    public BudgetFileController(BudgetFileService budgetFileService, ILogger<BudgetFileController> logger)
    {
        _logger = logger;
        _budgetFileService = budgetFileService;
    }
    
    [HttpGet]
    [Route("")]
    public IActionResult GetAll()
    {
        _logger.LogInformation("Requesting all budgets");
        string? requestingUser = User.Identity?.Name;
        return Ok(_budgetFileService.GetAllBudgetFiles(requestingUser).Select(b => new BudgetDto(b)));
    }

    [HttpGet]
    [Authorize]
    [Route("my")]
    public IActionResult GetMy()
    {
        _logger.LogInformation("Requesting user budgets");
        string? requestingUser = User.Identity?.Name;
        return Ok(_budgetFileService.GetOwnBudgetFiles(requestingUser!).Select(b => new BudgetDto(b)));
    }

    [HttpPost]
    [Authorize]
    [Route("new")]
    public IActionResult Add([FromBody] BudgetFileAdd payload)
    {
        _logger.LogInformation("Creating new budget");
        string? requestingUser = User.Identity?.Name;
        return Ok(new BudgetDto(_budgetFileService.AddBudgetFile(payload.Description!, requestingUser!)));
    }
    
    [HttpGet]
    [Route("{budgetId}")]
    public IActionResult GetOne(string budgetId)
    {
        _logger.LogInformation("Getting budget {}", budgetId);
        string? requestingUser = User.Identity?.Name;
        return Ok(new BudgetDto(_budgetFileService.GetBudgetFile(budgetId, requestingUser)));
    }


    [HttpPost]
    [Authorize]
    [Route("{budgetId}/addOwner")]
    public IActionResult AddOwner([FromBody] BudgetOwnerAdd payload, string budgetId)
    {
        _logger.LogInformation("Adding owner {} to budget {}", payload.UserId, budgetId);
        string? requestingUser = User.Identity?.Name;
        return Ok(new BudgetDto(_budgetFileService.AddOwner(budgetId, payload.UserId!, requestingUser!)));
    }
    
    [HttpPost]
    [Authorize]
    [Route("{budgetId}/removeOwner")]
    public IActionResult RemoveOwner([FromBody] BudgetOwnerAdd payload, string budgetId)
    {
        _logger.LogInformation("Removing owner {} from budget {}", payload.UserId, budgetId);
        string? requestingUser = User.Identity?.Name;
        return Ok(new BudgetDto(_budgetFileService.RemoveOwner(budgetId, payload.UserId!, requestingUser!)));
    }

    [HttpDelete]
    [Authorize]
    [Route("{budgetId}")]
    public IActionResult Delete(string budgetId)
    {
        _logger.LogInformation("Delete budget {}", budgetId);
        string? requestingUser = User.Identity?.Name;
        _budgetFileService.DeleteBudgetFile(budgetId, requestingUser!);
        return Ok();
    }
    
    [HttpPut]
    [Authorize]
    [Route("{budgetId}")]
    public IActionResult Update([FromBody] BudgetFileUpdate payload, string budgetId)
    {
        _logger.LogInformation("Update budget {}", budgetId);
        string? requestingUser = User.Identity?.Name;
        var b = _budgetFileService.GetBudgetFile(budgetId, requestingUser);
        b.Description = payload.Description ?? string.Empty;
        b.IsPrivate = payload.IsPrivate ?? b.IsPrivate;
        _budgetFileService.UpdateBudgetFile(budgetId, requestingUser!, b);
        return Ok();
    }

}