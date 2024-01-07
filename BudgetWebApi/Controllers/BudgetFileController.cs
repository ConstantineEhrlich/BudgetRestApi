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
        string? requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            return Ok(_budgetFileService.GetAllBudgetFiles(requestingUser).Select(b => new Dto.BudgetDto(b)));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
    }

    [HttpGet]
    [Authorize]
    [Route("my")]
    public IActionResult GetMy()
    {
        string? requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            return Ok(_budgetFileService.GetOwnBudgetFiles(requestingUser).Select(b => new Dto.BudgetDto(b)));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
    }

    [HttpPost]
    [Authorize]
    [Route("new")]
    public IActionResult Add([FromBody] Dto.BudgetFileAdd payload)
    {
        string? requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        if (requestingUser is null)
        {
            return Unauthorized();
        }

        try
        {
            return Ok(new BudgetDto(_budgetFileService.AddBudgetFile(payload.Description, requestingUser)));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }
    
    [HttpGet]
    [Route("{budgetId}")]
    public IActionResult GetOne(string budgetId)
    {
        string? requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            return Ok(new BudgetDto(_budgetFileService.GetBudgetFile(budgetId, requestingUser)));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }


    [HttpPost]
    [Authorize]
    [Route("{budgetId}/addOwner")]
    public IActionResult AddOwner([FromBody] Dto.BudgetOwnerAdd payload, string budgetId)
    {
        string? requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        if (requestingUser is null)
        {
            return Unauthorized();
        }

        try
        {
            return Ok(new BudgetDto(_budgetFileService.AddOwner(budgetId, payload.UserId, requestingUser)));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }
    
    [HttpPost]
    [Authorize]
    [Route("{budgetId}/removeOwner")]
    public IActionResult RemoveOwner([FromBody] Dto.BudgetOwnerAdd payload, string budgetId)
    {
        string? requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        if (requestingUser is null)
        {
            return Unauthorized();
        }

        try
        {
            return Ok(new BudgetDto(_budgetFileService.RemoveOwner(budgetId, payload.UserId, requestingUser)));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpDelete]
    [Authorize]
    [Route("{budgetId}")]
    public IActionResult Delete(string budgetId)
    {
        string? requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        if (requestingUser is null)
        {
            return Unauthorized();
        }

        try
        {
            _budgetFileService.DeleteBudgetFile(budgetId, requestingUser);
            return Ok();
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }
    
    [HttpPut]
    [Authorize]
    [Route("{budgetId}")]
    public IActionResult Update([FromBody] Dto.BudgetFileUpdate payload, string budgetId)
    {
        string? requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        if (requestingUser is null)
        {
            return Unauthorized();
        }
        try
        {
            var b = _budgetFileService.GetBudgetFile(budgetId);
            b.Description = payload.Description;
            b.IsPrivate = payload.IsPrivate ?? b.IsPrivate;
            _budgetFileService.UpdateBudgetFile(budgetId, requestingUser, b);
            return Ok();
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

}