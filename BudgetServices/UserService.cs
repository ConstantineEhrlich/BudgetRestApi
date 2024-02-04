using BudgetModel;
using BudgetModel.Models;

namespace BudgetServices;

public class UserService
{
    private readonly Context _context;

    public UserService(Context context)
    {
        _context = context;
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

    public User GetUser(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new BudgetServiceException("Id can't be empty");
        
        User? u = _context.Users!.FirstOrDefault(usr => usr.Id == id);
        if (u is null)
            throw new BudgetServiceException($"User {id} does not exist!");

        return u;
    }

    public User UpdateUser(string id, User user)
    {
        User target = GetUser(id);
        if (target.Id != user.Id)
            throw new BudgetServiceException($"Can't update user {id} with the profile of user {user.Id}");
        
        target.Name = user.Name;
        target.Email = user.Email;
        
        _context.SaveChanges();
        return target;
    }

}