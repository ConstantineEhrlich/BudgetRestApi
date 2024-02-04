using BudgetModel;
using BudgetModel.Enums;
using BudgetModel.Models;

namespace BudgetServices;

public class CategoryService
{
    private readonly Context _context;
    private readonly BudgetFileService _budgetService;

    public CategoryService(Context context, BudgetFileService budgetService)
    {
        _context = context;
        _budgetService = budgetService;
    }

    public void ChangeStatus(string budgetFileId, string categoryId, string requestingUserId)
    {
        Category cat = GetCategory(budgetFileId, categoryId, requestingUserId);
        cat.IsActive = !cat.IsActive;
        _context.SaveChanges();
    }

    public Category AddCategory(string budgetFileId, string categoryId, string requestingUserId, string? description = null, TransactionType defaultType = TransactionType.Expense)
    {
        _budgetService.ThrowIfNotOwner(requestingUserId, budgetFileId);
        if (_context.Categories!.Any(c => c.Id == categoryId && c.BudgetFileId == budgetFileId))
        {
            throw new BudgetServiceException("Category already exists!");
        }
        Category cat = new(budgetFileId, categoryId, description ?? string.Empty)
        {
            DefaultType = defaultType,
        };

        _context.Add(cat);
        _context.SaveChanges();
        return cat;
    }

    public Category GetCategory(string budgetFileId, string categoryId, string requestingUserId)
    {
        _budgetService.ThrowIfNotOwner(requestingUserId, budgetFileId);
        return _context.Categories?.FirstOrDefault(c => c.Id == categoryId && c.BudgetFileId == budgetFileId)
            ?? throw new BudgetServiceException($"Category not found");
    }

    public Category UpdateCategory(string budgetFileId, string id, string requestingUserId, Category category)
    {
        Category target = GetCategory(budgetFileId, id, requestingUserId);

        target.Description = category.Description;
        target.IsActive = category.IsActive;
        _context.SaveChanges();
        return target;
    }
    
    public void DeleteCategory(string budgetFileId, string id, string requestingUserId)
    {
        Category target = GetCategory(budgetFileId, id, requestingUserId);
        target.IsActive = false;
        _context.SaveChanges();
    }

    public List<Category> GetAllCategories(string budgetFileId, string requestingUserId)
    {
        BudgetFile b = _budgetService.GetBudgetFile(budgetFileId, requestingUserId);
        return b.Categories.Where(c => c.IsActive).ToList();
    }
}