using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using BudgetModel;
using BudgetModel.Models;

using Microsoft.IdentityModel.Tokens;

namespace BudgetServices;

public class UserService
{
    private readonly Context _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(Context context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public User CreateUser(string id, string name, string email)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id can't be empty");

        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name can't be empty");

        if (_context.Users!.FirstOrDefault(u => u.Id == id) is not null)
            throw new ArgumentException("User already exists!");

        User u = new User(id, name, email);
        _context.Add(u);
        _context.SaveChanges();
        return u;
    }

    public User GetUser(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id can't be empty", nameof(id));
        
        User? u = _context.Users!.FirstOrDefault(usr => usr.Id == id);
        if (u is null)
            throw new ArgumentException($"User {id} does not exist!");

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