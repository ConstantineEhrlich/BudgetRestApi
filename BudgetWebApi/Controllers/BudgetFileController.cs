using BudgetModel.Models;
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
    public async Task<ActionResult<List<BudgetFile>>> GetPublic()
    {
        _logger.LogInformation("Requesting all budgets");
        string? requestingUser = User.Identity?.Name;
        return await _budgetFileService.GetPublicBudgetFiles(requestingUser);
    }

    [HttpGet]
    [Authorize]
    [Route("my")]
    public async Task<ActionResult<List<BudgetFile>>> GetMy()
    {
        _logger.LogInformation("Requesting user budgets");
        string? requestingUser = User.Identity?.Name;
        return await _budgetFileService.GetOwnBudgetFiles(requestingUser!);
    }

    [HttpPost]
    [Authorize]
    [Route("new")]
    public async Task<ActionResult<BudgetFile>> Add([FromBody] BudgetFileAdd payload)
    {
        _logger.LogInformation("Creating new budget");
        string? requestingUser = User.Identity?.Name;
        return await _budgetFileService.AddBudgetFile(payload.Description ?? string.Empty, requestingUser!);
    }
    
    [HttpGet]
    [Route("{budgetId}")]
    public async Task<ActionResult<BudgetFile>> GetOne(string budgetId)
    {
        _logger.LogInformation("Getting budget {}", budgetId);
        string? requestingUser = User.Identity?.Name;
        return await _budgetFileService.GetBudgetFile(budgetId, requestingUser);
    }


    [HttpPost]
    [Authorize]
    [Route("{budgetId}/addOwner")]
    public async Task<ActionResult<BudgetFile>> AddOwner([FromBody] BudgetOwnerAdd payload, string budgetId)
    {
        _logger.LogInformation("Adding owner {} to budget {}", payload.UserId, budgetId);
        string? requestingUser = User.Identity?.Name;
        return await _budgetFileService.AddOwner(budgetId, payload.UserId!, requestingUser!);
    }
    
    [HttpPost]
    [Authorize]
    [Route("{budgetId}/removeOwner")]
    public async Task<ActionResult<BudgetFile>> RemoveOwner([FromBody] BudgetOwnerAdd payload, string budgetId)
    {
        _logger.LogInformation("Removing owner {} from budget {}", payload.UserId, budgetId);
        string? requestingUser = User.Identity?.Name;
        return await _budgetFileService.RemoveOwner(budgetId, payload.UserId!, requestingUser!);
    }

    [HttpDelete]
    [Authorize]
    [Route("{budgetId}")]
    public async Task<ActionResult> Delete(string budgetId)
    {
        _logger.LogInformation("Delete budget {}", budgetId);
        string? requestingUser = User.Identity?.Name;
        await _budgetFileService.DeleteBudgetFile(budgetId, requestingUser!);
        return Ok();
    }
    
    [HttpPut]
    [Authorize]
    [Route("{budgetId}")]
    public async Task<ActionResult> Update([FromBody] BudgetFileUpdate payload, string budgetId)
    {
        _logger.LogInformation("Update budget {}", budgetId);
        string? requestingUser = User.Identity?.Name;
        var b = await _budgetFileService.GetBudgetFile(budgetId, requestingUser);
        b.Description = payload.Description ?? string.Empty;
        b.IsPrivate = payload.IsPrivate ?? b.IsPrivate;
        await _budgetFileService.UpdateBudgetFile(budgetId, requestingUser!, b);
        return Ok();
    }

}