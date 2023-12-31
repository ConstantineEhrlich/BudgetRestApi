using System.Net.WebSockets;
using BudgetServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using BudgetModel.Enums;
using BudgetWebApi.Dto;
using BudgetWebApi.Sockets;

namespace BudgetWebApi.Controllers;

[ApiController]
[Route("budgets")]
public class TransactionController : ControllerBase
{
    private readonly ILogger<TransactionController> _logger;
    private readonly TransactionService _transactionService;
    private readonly BudgetUpdateManager _updateManager;

    public TransactionController(ILogger<TransactionController> logger, TransactionService transactionService, BudgetUpdateManager updateManager)
    {
        _logger = logger;
        _transactionService = transactionService;
        _updateManager = updateManager;
    }

    [Route("{budgetId}/updates")]
    public async Task Get(string budgetId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            WebSocket socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _updateManager.AddSocket(budgetId, socket);
            await SocketListener.Listen(socket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    
    
    
    

    [HttpPost]
    [Authorize]
    [Route("{budgetId}/transactions/add")]
    public IActionResult Add([FromBody] Dto.TransactionAdd payload, string budgetId)
    {
        string requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            _updateManager.BroadcastUpdate(budgetId);
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