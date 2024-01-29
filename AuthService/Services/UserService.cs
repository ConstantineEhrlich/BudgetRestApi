using AuthService.Models;
using AuthService.Services.Dto;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace AuthService.Services;

public class UserService
{
    private readonly IPasswordHasher<User> _hashser;
    private readonly UsersDatabase _database;

    public UserService(IPasswordHasher<User> hasher, UsersDatabase database)
    {
        _hashser = hasher;
        _database = database;
    }
    
    public async Task CreateUser(SignUp signUpData)
    {
        User u = new User
        {
            Login = signUpData.Login,
            Email = signUpData.Email,
        };
        u.PasswordHash = _hashser.HashPassword(u, signUpData.Password);
        
        try
        {
            await _database.Users.InsertOneAsync(u);
        }
        catch (MongoWriteException e) // Occurs when the login is not unique
        {
            throw new UserServiceException($"User {signUpData.Login} already exists!");
        }

        Profile p = new Profile()
        {
            Login = signUpData.Login,
            FullName = signUpData.Name,
            PublicEmail = signUpData.Email,
        };

        await _database.Profiles.InsertOneAsync(p);
        await _database.Users.UpdateOneAsync(FilterByLogin(signUpData.Login),
            Builders<User>.Update.Set(nameof(User.ProfileId), p.Id));
    }

    public async Task<User> GetUser(string login)
    {
        return await _database.Users.Find(FilterByLogin(login)).FirstOrDefaultAsync();
    }
    
    public async Task FailedLogin(string userLogin)
    {
        
    }

    public async Task SuccessLogin(string userLogin)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> VerifyPassword(string userLogin, string password)
    {
        User u = await GetUser(userLogin);
        switch (_hashser.VerifyHashedPassword(u, u.PasswordHash, password))
        {
            case PasswordVerificationResult.Failed:
                await FailedLogin(userLogin);
                return false;
            case PasswordVerificationResult.Success:
                await SuccessLogin(userLogin);
                return true;
            case PasswordVerificationResult.SuccessRehashNeeded:
                await RehashPassword(userLogin, password);
                await SuccessLogin(userLogin);
                return true;
            default:
                return false;
        }
    }

    public Task UpdatePassword(string UserLogin, string oldPassword, string newPassword)
    {
        throw new NotImplementedException();
    }
    
    private FilterDefinition<User> FilterByLogin(string login)
    {
        return Builders<User>.Filter.Eq(nameof(User.Login), login);
    }

    private async Task RehashPassword(string login, string password)
    {
        User u = await GetUser(login);
        string rehash = _hashser.HashPassword(u, password);
        await _database.Users.UpdateOneAsync(FilterByLogin(u.Login),
            Builders<User>.Update.Set(nameof(User.PasswordHash), rehash));
    }
    
}