using Microsoft.AspNetCore.Mvc;

namespace BudgetWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ILogger<TransactionsController> _logger;
    private readonly BudgetModel.Context _context;
    
    public TransactionsController(ILogger<TransactionsController> logger, BudgetModel.Context context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet(Name = "GetTransactions")]
    public IEnumerable<object> Get()
    {
        return _context.Transactions.Select(t => new
        {
            Id = t.Id,
            Date = t.Date,
            Author = t.Author.Name,
            Amount = t.Amount,
            Category = t.Category.Description
        }).ToArray();

    }
    
    
}