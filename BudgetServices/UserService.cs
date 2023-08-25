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

    public User CreateUser(string id, string name)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id can't be empty", nameof(id));

        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name can't be empty", nameof(name));

        if (_context.Users.FirstOrDefault(u => u.Id == id) is not null)
            throw new ArgumentException("User already exists!", nameof(id));
        
        User u = new User(id, name);
        _context.Add(u);
        _context.SaveChanges();
        return u;
    }
    
    public User GetUser(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id can't be empty", nameof(id));
        
        User? u = _context.Users.FirstOrDefault(usr => usr.Id == id);
        if (u is null)
            throw new ArgumentException($"User {id} does not exist!", nameof(id));

        return u;
    }

    public User UpdateUser(string id, User user)
    {
        User target = GetUser(id);
        target.Name = user.Name;
        target.Email = user.Email;
        
        _context.SaveChanges();
        return target;
    }
}