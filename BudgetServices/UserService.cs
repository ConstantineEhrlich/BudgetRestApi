using BudgetModel;
using BudgetModel.Models;
using BudgetServices.Cache;
using Microsoft.EntityFrameworkCore;

namespace BudgetServices;

public class UserService
{
    private readonly Context _context;
    private readonly ICacheService<User> _cache;

    public UserService(Context context, ICacheService<User> userCache)
    {
        _context = context;
        _cache = userCache;
    }

    public async Task CreateUserAsync(string id, string name, string email)
    {
        if (string.IsNullOrEmpty(id))
            throw new BudgetServiceException("Id can't be empty");

        if (string.IsNullOrEmpty(name))
            throw new BudgetServiceException("Name can't be empty");

        if (_context.Users!.FirstOrDefault(u => u.Id == id) is not null)
            throw new BudgetServiceException("User already exists!");

        User u = new User(id, name, email);
        await _context.AddAsync(u);
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetUserAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new BudgetServiceException("Id can't be empty");

        User? cachedUser = await _cache.GetFromCache(id);
        if (cachedUser is not null)
            return cachedUser;
        
        User? dbUser = await _context.Users!.FirstOrDefaultAsync(usr => usr.Id == id);
        if (dbUser is null)
            throw new BudgetServiceException($"User {id} does not exist!");

        await _cache.UpdateCache(dbUser, dbUser.Id!);
        
        return dbUser;
    }

    public async Task<User> UpdateUser(string id, User user)
    {
        User target = await GetUserAsync(id);
        _context.Attach(target);
        if (target.Id != user.Id)
            throw new BudgetServiceException($"Can't update user {id} with the profile of user {user.Id}");
        
        target.Name = user.Name;
        target.Email = user.Email;

        await _cache.DeleteCache(id);
        await _context.SaveChangesAsync();
        return target;
    }

}