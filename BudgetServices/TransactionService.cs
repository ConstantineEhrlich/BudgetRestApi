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

    public async Task<Transaction> AddTransaction(string budgetFileId,
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
        await _budgetService.ThrowIfNotOwner(requestingUserId, budgetFileId);
        
        BudgetFile budgetFile = await _budgetService.GetBudgetFile(budgetFileId, requestingUserId);
        
        User author = await _userService.GetUserAsync(requestingUserId);
        User owner = ownerId is null ? author : await _userService.GetUserAsync(ownerId);
        Category cat = await _categoryService.GetCategory(budgetFileId, categoryId, requestingUserId);
        
        DateTime entryDate = date ?? DateTime.UtcNow;
        
        Transaction t = new(budgetFile, author, type, cat, description ?? string.Empty, amount)
        {
            OwnerId = owner.Id!,
            Date = entryDate,
            Year = year ?? entryDate.Year,
            Period = period ?? entryDate.Month
        };
        await _context.AddAsync(t);
        await _context.SaveChangesAsync();
        return t;
    }

    public async Task<Transaction> GetTransaction(string id, string requestingUserId)
    {
        Transaction? t = await _context.Transactions!.Include(transaction => transaction.BudgetFile)
            .FirstOrDefaultAsync(tr => tr.Id == id);
        if(t is null)                
            throw new BudgetServiceException($"Transaction {id} not found!");
    
        if (t.BudgetFile?.IsPrivate ?? false)
            await _budgetService.ThrowIfNotOwner(requestingUserId, t.BudgetFileId);
        
        return t;
    }
    
    public async Task<Transaction> UpdateTransaction(string id, string requestingUserId, Transaction transaction)
    {
        Transaction target = await GetTransaction(id, requestingUserId);
        
        target.Type = transaction.Type;
        target.Date = transaction.Date;
        target.OwnerId = transaction.OwnerId;
        target.Year = transaction.Year;
        target.Period = transaction.Period;
        target.Description = transaction.Description;
        await _context.SaveChangesAsync();
        return target;
    }

    public async Task<IQueryable<Transaction>> GetAllTransactions(string budgetId, string? requestingUserId)
    {
        _ = await _budgetService.GetBudgetFile(budgetId, requestingUserId);

        IQueryable<Transaction> trns = _context.Transactions!
            .Where(t => t.BudgetFileId == budgetId)
            .OrderByDescending(t => t.Date);

        return trns;
    }
}