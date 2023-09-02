using BudgetServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BudgetModel.Enums;
using BudgetWebApi.Dto;

namespace BudgetWebApi.Controllers;

[ApiController]
[Route("budgets")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;
    private readonly TransactionService _transactionService;

    public TransactionController(ILogger<TransactionController> logger, TransactionService transactionService)
    {
        _logger = logger;
        _transactionService = transactionService;
    }

    [HttpPost]
    [Authorize]
    [Route("{budgetId}/transactions/add")]
    public IActionResult Add([FromBody] Dto.TransactionAdd payload, string budgetId)
    {
        string requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            return Ok(new Dto.TransactionDto(_transactionService.AddTransaction(budgetId,
                                                                                  requestingUser,
                                                                                  payload.CategoryId,
                                                                                  payload.Amount,
                                                                                  payload.Description,
                                                                                  (TransactionType)payload.TransactionType,
                                                                                  payload.OwnerId,
                                                                                  payload.Date,
                                                                                  payload.Year,
                                                                                  payload.Period)));
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
    [Route("{budgetId}/transactions")]
    public IActionResult GetAll(string budgetId)
    {
        string requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            return Ok(_transactionService
                .GetAllTransactions(budgetId, requestingUser)
                .OrderByDescending(t => t.Date)
                .Select(t=>new Dto.TransactionDto(t)));
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
    [Route("{budgetId}/transactions/{transId}")]
    public IActionResult GetOne(string budgetId, string transId)
    {
        string requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            return Ok(new Dto.TransactionDto(_transactionService.GetTransaction(transId, requestingUser)));
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
    [Route("{budgetId}/transactions/{transId}")]
    public IActionResult Update([FromBody] Dto.TransactionUpdate payload, string budgetId, string transId)
    {
        string requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            var t = _transactionService.GetTransaction(transId, requestingUser);
            t.CategoryId = payload.CategoryId;
            t.Description = payload.Description;
            t.Type = (TransactionType)payload.TransactionType;
            t.OwnerId = payload.OwnerId;
            t.Date = payload.Date ?? t.Date;
            t.Year = payload.Year ?? t.Year;
            t.Period = payload.Period ?? t.Period;
            
            return Ok(new Dto.TransactionDto(_transactionService.UpdateTransaction(transId, requestingUser, t)));
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