using BudgetModel;
using BudgetModel.Enums;
using BudgetModel.Models;
using BudgetServices.Cache;
using Microsoft.EntityFrameworkCore;

namespace BudgetServices;

public class CategoryService
{
    private readonly Context _context;
    private readonly BudgetFileService _budgetService;
    private readonly ICacheService<BudgetFile> _cache;

    public CategoryService(Context context, BudgetFileService budgetService, ICacheService<BudgetFile> budgetCache)
    {
        _context = context;
        _budgetService = budgetService;
        _cache = budgetCache;
    }

    public async Task ChangeStatus(string budgetFileId, string categoryId, string requestingUserId)
    {
        Category cat = await GetCategory(budgetFileId, categoryId, requestingUserId);
        _context.Attach(cat);
        cat.IsActive = !cat.IsActive;
        await _cache.DeleteCache(cat.BudgetFileId);
        await _context.SaveChangesAsync();
    }

    public async Task<Category> AddCategory(string budgetFileId, string categoryId, string requestingUserId, string? description = null, TransactionType defaultType = TransactionType.Expense)
    {
        await _budgetService.ThrowIfNotOwner(requestingUserId, budgetFileId);
        if (await _context.Categories!.AnyAsync(c => c.Id == categoryId && c.BudgetFileId == budgetFileId))
        {
            throw new BudgetServiceException("Category already exists!");
        }
        Category cat = new(budgetFileId, categoryId, description ?? string.Empty)
        {
            DefaultType = defaultType,
        };

        await _context.AddAsync(cat);
        await _cache.DeleteCache(budgetFileId);
        await _context.SaveChangesAsync();
        return cat;
    }

    public async Task<Category> GetCategory(string budgetFileId, string categoryId, string requestingUserId)
    {
        BudgetFile b = await _budgetService.GetBudgetFile(budgetFileId, requestingUserId);
        return  b.Categories.FirstOrDefault(c => c.Id == categoryId && c.BudgetFileId == budgetFileId)
               ?? throw new BudgetServiceException($"Category not found");
    }

    public async Task<Category> UpdateCategory(string budgetFileId, string id, string requestingUserId, Category category)
    {
        Category target = await GetCategory(budgetFileId, id, requestingUserId);
        _context.Attach(target);
        target.Description = category.Description;
        target.DefaultType = category.DefaultType;
        target.IsActive = category.IsActive;
        await _cache.DeleteCache(budgetFileId);
        await _context.SaveChangesAsync();
        return target;
    }
    
    public async Task DeleteCategory(string budgetFileId, string id, string requestingUserId)
    {
        Category target = await GetCategory(budgetFileId, id, requestingUserId);
        target.IsActive = false;
        await _cache.DeleteCache(budgetFileId);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Category>> GetAllCategories(string budgetFileId, string requestingUserId)
    {
        BudgetFile b = await _budgetService.GetBudgetFile(budgetFileId, requestingUserId);
        return b.Categories.OrderBy(c => c.Id).ToList();
    }
}