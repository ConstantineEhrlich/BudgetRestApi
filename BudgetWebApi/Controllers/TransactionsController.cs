using BudgetModel.Models;
using BudgetServices;
using Microsoft.AspNetCore.Mvc;

namespace BudgetWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ILogger<TransactionsController> _logger;
    private readonly TransactionService _transactionService;
    
    public TransactionsController(ILogger<TransactionsController> logger, TransactionService transactionService)
    {
        _logger = logger;
        _transactionService = transactionService;
    }

    [HttpGet("{transactionId}")]
    public Transaction Get(string transactionId)
    {
        return _transactionService.GetTransaction(transactionId, "john");
    }

    [HttpGet(Name = "transactions")]
    public IEnumerable<object> Get()
    {
        return _transactionService.GetAllTransactions("1", "john").ToArray();

    }
    
    
}