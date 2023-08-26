using BudgetModel;
using BudgetModel.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetServices;

public class BudgetFileService
{
    private readonly Context _context;
    private readonly UserService _userService;

    public BudgetFileService(Context context, UserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public BudgetFile AddBudgetFile(string description, string requestingUserId)
    {
        User u = _userService.GetUser(requestingUserId);
        BudgetFile b = new(description)
        {
            Slug = CreateSlug(description)
        };
        b.Owners.Add(u);
        _context.Add(b);
        _context.SaveChanges();
        return b;
    }

    public BudgetFile AddOwner(string budgetId, string newOwnerId, string requestingUserId)
    {
        BudgetFile b = GetBudgetFile(budgetId, requestingUserId);
        User newOwner = _userService.GetUser(newOwnerId);
        if (b.Owners.Contains(newOwner))
            throw new ArgumentException($"{newOwnerId} is already owner of the budget!", nameof(newOwnerId));
        
        b.Owners.Add(newOwner);
        _context.SaveChanges();
        return b;
    }

    public BudgetFile RemoveOwner(string budgetId, string ownerId, string requestingUserId)
    {
        BudgetFile b = GetBudgetFile(budgetId, requestingUserId);
        User newOwner = _userService.GetUser(ownerId);
        b.Owners.Remove(newOwner);
        _context.SaveChanges();
        return b;
    }

    public BudgetFile GetBudgetFile(string id, string? requestingUserId = null)
    {
        BudgetFile? b = _context.Budgets?.Include(budgetFile => budgetFile.Owners).FirstOrDefault(bd => bd.Id == id);
        if (b is null)
            throw new ArgumentException($"Budget not found", nameof(id));

        if (b.IsDeleted)
            throw new ArgumentException($"Budget is deleted", nameof(id));
        
        if (b.IsPrivate && requestingUserId is null)
            throw new ArgumentException($"The budget is private and the user is not specified", nameof(requestingUserId));
        
        if(b.IsPrivate && !IsOwner(requestingUserId!, id))
                throw new ArgumentException($"The budget is not owned by {requestingUserId}", nameof(requestingUserId));

        return b;
    }

    public BudgetFile UpdateBudgetFile(string id, string requestingUserId, BudgetFile b)
    {
        BudgetFile target = GetBudgetFile(id, requestingUserId);
        if (!IsOwner(requestingUserId, id))
            throw new ArgumentException($"User {requestingUserId} is not allowed to change budget {id}", nameof(requestingUserId));

        target.Description = b.Description;
        target.IsPrivate = b.IsPrivate;
        _context.SaveChanges();
        return target;
    }

    public void DeleteBudgetFile(string id, string requestingUserId)
    {
        BudgetFile target = GetBudgetFile(id, requestingUserId);
        if (!IsOwner(requestingUserId, id))
            throw new ArgumentException($"User {requestingUserId} is not allowed to delete budget {id}", nameof(requestingUserId));

        target.IsDeleted = true;
        _context.SaveChanges();

    }

    public List<BudgetFile> GetAllBudgetFiles(string? requestingUserId = null)
    {
        List<BudgetFile> budgetFiles = new();
        budgetFiles.AddRange(_context.Budgets!.Where(b => !b.IsPrivate));

        if (requestingUserId is not null)
            budgetFiles.AddRange(_context.Budgets!.Where(b => b.Owners.Select(o => o.Id).Contains(requestingUserId))
                .Where(b => b.IsPrivate));
        
        return budgetFiles;
    }

    private bool IsOwner(string userId, string budgetId)
    {
        BudgetFile? b = _context.Budgets!.Include(budgetFile => budgetFile.Owners).FirstOrDefault(b => b.Id == budgetId);
        if (b is null)
            return false;
        
        return b.Owners.Select(u => u.Id).Contains(userId);
    }
    
    private string CreateSlug(string description)
    {
        string[] words = description.Split(' ').Take(3).ToArray();
        string slug = string.Join("-", words).ToLower();

        if (_context.Budgets!.Select(b => b.Slug).Contains(slug))
        {
            string date = DateTime.Now.ToString("dd-MMM-yyyy");
            return slug + "-" + date;
        }
        else
        {
            return slug;
        }
        
    }

    public void ThrowIfNotOwner(string userId, string budgetId)
    {
        if (!this.IsOwner(userId, budgetId))
        {
            throw new ArgumentException($"User {userId} is not allowed on budget {budgetId}",
                nameof(userId));
        }
    }
}