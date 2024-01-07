using BudgetModel;
using BudgetModel.Enums;
using BudgetModel.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetServices;

public class TransactionService
{
    private readonly Context _context;
    private readonly UserService _userService;
    private readonly BudgetFileService _budgetService;
    private readonly CategoryService _categoryService;

    public TransactionService(Context context, UserService userService, BudgetFileService budgetService,
        CategoryService categoryService)
    {
        _context = context;
        _userService = userService;
        _budgetService = budgetService;
        _categoryService = categoryService;
    }

    public Transaction AddTransaction(string budgetFileId,
                               string requestingUserId,
                               string categoryId,
                               decimal amount,
                               string? description = "",
                               TransactionType type = (TransactionType.Expense),
                               string? ownerId = null,
                               DateTime? date = null,
                               int? year = null,
                               int? period = null)
    {
        _budgetService.ThrowIfNotOwner(requestingUserId, budgetFileId);
        
        BudgetFile budgetFile = _budgetService.GetBudgetFile(budgetFileId, requestingUserId);
        User author = _userService.GetUser(requestingUserId);
        User owner = ownerId is null ? author : _userService.GetUser(ownerId);
        Category cat = _categoryService.GetCategory(budgetFileId, categoryId, requestingUserId);
        
        DateTime entryDate = date ?? DateTime.UtcNow;
        
        Transaction t = new(budgetFile, author, type, cat, description ?? string.Empty, amount)
        {
            OwnerId = owner.Id,
            Date = entryDate,
            Year = year ?? entryDate.Year,
            Period = period ?? entryDate.Month
        };
        _context.Add(t);
        _context.SaveChanges();
        return t;
    }

    public Transaction GetTransaction(string id, string requestingUserId)
    {
        Transaction? t = _context.Transactions!.Include(transaction => transaction.BudgetFile)
            .FirstOrDefault(tr => tr.Id == id);
        if(t is null)                
            throw new ArgumentException($"Transaction {id} not found!");
    
        if (t.BudgetFile.IsPrivate)
            _budgetService.ThrowIfNotOwner(requestingUserId, t.BudgetFileId);
        
        return t;
    }
    
    public Transaction UpdateTransaction(string id, string requestingUserId, Transaction transaction)
    {
        Transaction target = GetTransaction(id, requestingUserId);
        
        target.Type = transaction.Type;
        target.Date = transaction.Date;
        target.OwnerId = transaction.OwnerId;
        target.Year = transaction.Year;
        target.Period = transaction.Period;
        target.Description = transaction.Description;
        _context.SaveChanges();
        return target;
    }

    public List<Transaction> GetAllTransactions(string budgetId,
                                   string requestingUserId,
                                   int? forYear = null,
                                   int? forPeriod = null,
                                   string? byCategory = null,
                                   string? byOwner = null)
    {
        BudgetFile b = _budgetService.GetBudgetFile(budgetId, requestingUserId);
        if(b.IsPrivate)
            _budgetService.ThrowIfNotOwner(requestingUserId, budgetId);

        IEnumerable<Transaction> trns = _context.Transactions!
            .Where(t => t.BudgetFileId == budgetId);

        if (forYear is not null)
            trns = trns.Where(t => t.Year == forYear);

        if (forPeriod is not null)
            trns = trns.Where(t => t.Period == forPeriod);

        if (byCategory is not null)
            trns = trns.Where(t => t.CategoryId == byCategory);

        if (byOwner is not null)
            trns = trns.Where(t => t.OwnerId == byOwner);

        return trns.ToList();
    }
}