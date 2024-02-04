using BudgetServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        string? requestingUser = User.Identity?.Name;
        return Ok(_budgetFileService.GetAllBudgetFiles(requestingUser).Select(b => new Dto.BudgetDto(b)));
    }

    [HttpGet]
    [Authorize]
    [Route("my")]
    public IActionResult GetMy()
    {
        string? requestingUser = User.Identity?.Name;
        return Ok(_budgetFileService.GetOwnBudgetFiles(requestingUser!).Select(b => new Dto.BudgetDto(b)));
    }

    [HttpPost]
    [Authorize]
    [Route("new")]
    public IActionResult Add([FromBody] Dto.BudgetFileAdd payload)
    {
        string? requestingUser = User.Identity?.Name;
        return Ok(new BudgetDto(_budgetFileService.AddBudgetFile(payload.Description, requestingUser!)));
    }
    
    [HttpGet]
    [Route("{budgetId}")]
    public IActionResult GetOne(string budgetId)
    {
        string? requestingUser = User.Identity?.Name;
        return Ok(new BudgetDto(_budgetFileService.GetBudgetFile(budgetId, requestingUser)));
    }


    [HttpPost]
    [Authorize]
    [Route("{budgetId}/addOwner")]
    public IActionResult AddOwner([FromBody] Dto.BudgetOwnerAdd payload, string budgetId)
    {
        string? requestingUser = User.Identity?.Name;
        return Ok(new BudgetDto(_budgetFileService.AddOwner(budgetId, payload.UserId, requestingUser)));
    }
    
    [HttpPost]
    [Authorize]
    [Route("{budgetId}/removeOwner")]
    public IActionResult RemoveOwner([FromBody] Dto.BudgetOwnerAdd payload, string budgetId)
    {
        string? requestingUser = User.Identity?.Name;
        return Ok(new BudgetDto(_budgetFileService.RemoveOwner(budgetId, payload.UserId, requestingUser!)));
    }

    [HttpDelete]
    [Authorize]
    [Route("{budgetId}")]
    public IActionResult Delete(string budgetId)
    {
        string? requestingUser = User.Identity?.Name;
        _budgetFileService.DeleteBudgetFile(budgetId, requestingUser);
        return Ok();
    }
    
    [HttpPut]
    [Authorize]
    [Route("{budgetId}")]
    public IActionResult Update([FromBody] Dto.BudgetFileUpdate payload, string budgetId)
    {
        string? requestingUser = User.Identity?.Name;
        var b = _budgetFileService.GetBudgetFile(budgetId, requestingUser);
        b.Description = payload.Description ?? string.Empty;
        b.IsPrivate = payload.IsPrivate ?? b.IsPrivate;
        _budgetFileService.UpdateBudgetFile(budgetId, requestingUser, b);
        return Ok();
    }

}