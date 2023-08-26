﻿using System.IdentityModel.Tokens.Jwt;
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

    public User CreateUser(string id, string name, string password)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id can't be empty", nameof(id));

        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name can't be empty", nameof(name));

        if (_context.Users.FirstOrDefault(u => u.Id == id) is not null)
            throw new ArgumentException("User already exists!", nameof(id));

        User u = new User(id, name);
        u.PasswordHash = _passwordHasher.HashPassword(u, password);
        _context.Add(u);
        _context.SaveChanges();
        return u;
    }

    public void FailedLogin(User u)
    {
        u.FailedLoginCount++;
        u.LastFailedLogin = DateTime.Now;
        _context.SaveChanges();
    }

    public void SuccessLogin(User u)
    {
        u.FailedLoginCount = 0;
        u.LastFailedLogin = null;
        _context.SaveChanges();
    }
    public bool VerifyPassword(User user, string password)
    {
        return _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password)
                == PasswordVerificationResult.Success;
    }

    public bool UpdatePassword(User user, string oldPassword, string newPassword)
    {
        if (VerifyPassword(user, oldPassword))
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            return true;
        }
        
        return false;
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

    public string GenerateJwtKey(User user)
    {
        string JWT_KEY = System.Environment.GetEnvironmentVariable("JWT_KEY") ?? "testkey";
        byte[] byteKey = Encoding.ASCII.GetBytes(JWT_KEY);
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityTokenDescriptor descriptor = new()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(byteKey), SecurityAlgorithms.HmacSha256Signature)
        };
        SecurityToken token = tokenHandler.CreateToken(descriptor);
        return tokenHandler.WriteToken(token);

    }
}