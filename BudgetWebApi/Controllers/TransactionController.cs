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
        string? requestingUser = User.Identity?.Name;
        TransactionDto trans = new(_transactionService.AddTransaction(budgetId,
                                                                       requestingUser!,
                                                                       payload.CategoryId,
                                                                       payload.Amount,
                                                                       payload.Description,
                                                                       (TransactionType?)payload.TransactionType ?? TransactionType.Expense,
                                                                       payload.OwnerId ?? requestingUser,
                                                                       payload.Date ?? DateTime.UtcNow,
                                                                       payload.Year ?? DateTime.UtcNow.Year,
                                                                       payload.Period ?? DateTime.UtcNow.Month));
        
        _updateManager.BroadcastUpdate(budgetId);
        return Ok(trans);

    }

    [HttpGet]
    [Route("{budgetId}/transactions")]
    public IActionResult GetAll(string budgetId)
    {
        string? requestingUser = User.Identity?.Name;
        return Ok(_transactionService
            .GetAllTransactions(budgetId, requestingUser)
            .OrderByDescending(t => t.Date)
            .Select(t=>new Dto.TransactionDto(t)));
    }

    [HttpGet]
    [Route("{budgetId}/transactions/{transId}")]
    public IActionResult GetOne(string budgetId, string transId)
    {
        string? requestingUser = User.Identity?.Name;
        return Ok(new Dto.TransactionDto(_transactionService.GetTransaction(transId, requestingUser!)));
    }

    [HttpPut]
    [Route("{budgetId}/transactions/{transId}")]
    public IActionResult Update([FromBody] Dto.TransactionUpdate payload, string budgetId, string transId)
    {
        string? requestingUser = User.Identity?.Name;
        var t = _transactionService.GetTransaction(transId, requestingUser!);
        t.CategoryId = payload.CategoryId!;
        t.Description = payload.Description ?? string.Empty;
        t.Type = (TransactionType)payload.TransactionType!;
        t.OwnerId = payload.OwnerId ?? requestingUser!;
        t.Date = payload.Date ?? t.Date;
        t.Year = payload.Year ?? t.Year;
        t.Period = payload.Period ?? t.Period;
        
        return Ok(new Dto.TransactionDto(_transactionService.UpdateTransaction(transId, requestingUser!, t)));
    }

    
}