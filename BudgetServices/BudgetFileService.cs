using BudgetModel;
using BudgetModel.Models;
using BudgetServices.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slugify;

namespace BudgetServices;

public class BudgetFileService
{
    private readonly Context _context;
    private readonly UserService _userService;
    private readonly ICacheService<BudgetFile> _cache;
    private readonly ILogger<BudgetFileService> _logger;

    public BudgetFileService(Context context, UserService userService, ICacheService<BudgetFile> cache, ILogger<BudgetFileService> logger)
    {
        _context = context;
        _userService = userService;
        _cache = cache;
        _logger = logger;
    }

    public async Task<BudgetFile> AddBudgetFile(string description, string requestingUserId)
    {
        User u = await _userService.GetUserAsync(requestingUserId);
        _context.Attach(u);
        BudgetFile b = new(description);
        b.Owners.Add(u);
        b.Slug = CreateSlug(b.Description);
        await _context.AddAsync(b);
        await _context.SaveChangesAsync();
        return b;
    }

    public async Task<BudgetFile> AddOwner(string budgetId, string newOwnerId, string requestingUserId)
    {
        BudgetFile b = await GetBudgetFile(budgetId, requestingUserId);
        _context.Attach(b);
        User newOwner = await _userService.GetUserAsync(newOwnerId);
        _context.Attach(newOwner);
        if (b.Owners.Contains(newOwner))
            throw new BudgetServiceException($"{newOwnerId} is already listed as owner");
        
        b.Owners.Add(newOwner);
        await _cache.DeleteCache(budgetId);
        await _context.SaveChangesAsync();
        return b;
    }

    public async Task<BudgetFile> RemoveOwner(string budgetId, string ownerId, string requestingUserId)
    {
        BudgetFile b = await GetBudgetFile(budgetId, requestingUserId);
        _context.Attach(b);
        User ownerToDelete = b.Owners.FirstOrDefault(o => o.Id == ownerId)
            ?? throw new BudgetServiceException($"User {ownerId} is not listed in the owners of the requested budget!");
        b.Owners.Remove(ownerToDelete);
        await _cache.DeleteCache(budgetId);
        await _context.SaveChangesAsync();
        return b;
    }

    public async Task<BudgetFile> GetBudgetFile(string id, string? requestingUserId)
    {
        BudgetFile? b = await _cache.GetFromCache(id);
        // If the object does not present in the cache
        if (b is null)
        {
            // Query the budget from the database
            b = await _context.Budgets!
                .Include(bf => bf.Owners)
                .Include(bf => bf.Categories)
                .FirstOrDefaultAsync(bf => bf.Id == id);
            // If still not found
            if (b is null)
                throw new BudgetServiceException($"Budget not found");
            // Else - update the cache
            await _cache.UpdateCache(b, b.Id);
        }

        if (b.IsDeleted)
            // Don't return deleted budget
            throw new BudgetServiceException($"Budget is deleted");
        
        if (b.IsPrivate && requestingUserId is null)
            // Don't return private budget to unauthorized user
            throw new BudgetServiceException($"The budget is private");
        
        if(b.IsPrivate && !await IsOwner(requestingUserId!, id))
            // Don't return private budget to non-owner
            throw new BudgetServiceException($"The budget is not owned by {requestingUserId}");
        
        return b;
    }

    public async Task<BudgetFile> UpdateBudgetFile(string id, string requestingUserId, BudgetFile b)
    {
        BudgetFile target = await GetBudgetFile(id, requestingUserId);
        _context.Attach(target);
        if (!await IsOwner(requestingUserId, id))
            throw new BudgetServiceException($"User {requestingUserId} is not allowed to change budget {id}");

        if (target.Id != b.Id)
            throw new BudgetServiceException($"Can't update Budget {id} with the Budget {b.Id}");
        
        target.Description = b.Description;
        target.IsPrivate = b.IsPrivate;
        await _cache.DeleteCache(id);
        await _context.SaveChangesAsync();
        return target;
    }

    public async Task DeleteBudgetFile(string id, string requestingUserId)
    {
        BudgetFile target = await GetBudgetFile(id, requestingUserId);
        _context.Attach(target);
        if (!await IsOwner(requestingUserId, id))
            throw new BudgetServiceException($"User {requestingUserId} is not allowed to delete budget {id}");

        target.IsDeleted = true;
        await _cache.DeleteCache(id);
        await _context.SaveChangesAsync();
    }

    public async Task<List<BudgetFile>> GetAllBudgetFiles(string? requestingUserId = null)
    {
        List<BudgetFile> budgetFiles = new();
        budgetFiles.AddRange(await _context.Budgets!.Where(b => !b.IsPrivate).ToListAsync());

        if (requestingUserId is not null)
            budgetFiles.AddRange(await GetOwnBudgetFiles(requestingUserId));
        
        return budgetFiles;
    }

    public async Task<List<BudgetFile>> GetOwnBudgetFiles(string requestingUserId)
    {
        return await _context.Budgets!
            .Include(b => b.Owners)
            .Where(b => b.Owners.Select(o => o.Id).Contains(requestingUserId))
            .ToListAsync();
    }
    
    private async Task<bool> IsOwner(string userId, string budgetId)
    {
        BudgetFile? b = await _cache.GetFromCache(budgetId)
                        ?? await _context.Budgets!.FirstOrDefaultAsync(b => b.Id == budgetId);
        return b is not null && b.Owners.Select(u => u.Id).Contains(userId);
    }
    
    private string CreateSlug(string description)
    {
        SlugHelper slugify = new();
        string[] desc = description.Split(' ').Take(3).ToArray();
        return slugify.GenerateSlug(string.Join(" ", desc));
    }

    public async Task ThrowIfNotOwner(string userId, string budgetId)
    {
        if (!await this.IsOwner(userId, budgetId))
            throw new BudgetServiceException($"User {userId} is not allowed on budget {budgetId}");
    }
}