using AuthService.Models;
using AuthService.Services.Dto;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace AuthService.Services;

public class UserService
{
    private readonly IPasswordHasher<User> _hasher;
    private readonly UsersDatabase _database;
    private readonly ILogger<UserService> _logger;
    private readonly ProfileService _profiles;

    public UserService(IPasswordHasher<User> hasher, UsersDatabase database, ILogger<UserService> logger)
    {
        _hasher = hasher;
        _database = database;
        _logger = logger;
    }
    
    public async Task CreateUser(SignUp signUpData)
    {
        User u = new User
        {
            Login = signUpData.Login,
            Email = signUpData.Email,
        };
        u.PasswordHash = _hasher.HashPassword(u, signUpData.Password);
        
        try
        {
            await _database.Users.InsertOneAsync(u);
        }
        catch (MongoWriteException e) // Occurs when the login is not unique
        {
            throw new AuthServiceException($"User {signUpData.Login} already exists!");
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
        User u = await _database.Users.Find(FilterByLogin(login)).FirstOrDefaultAsync();
		if (u is null)
			throw new AuthServiceException($"User {login} does not exists!");
        return u;
    }
    
    public async Task FailedLogin(User u)
    {
		u.LastSuccessLogin = null;
		u.LastFailedLogin = DateTime.UtcNow;
		u.FailedLoginCount++;
		await _database.Users.ReplaceOneAsync(FilterByLogin(u.Login), u);
    }

    public async Task SuccessLogin(User u)
    {
		u.LastSuccessLogin = DateTime.UtcNow;
		u.FailedLoginCount = 0;
		u.LastFailedLogin = null;
		await _database.Users.ReplaceOneAsync(FilterByLogin(u.Login), u);
    }

    public async Task<bool> PasswordIsCorrect(User u, string password)
    {
        switch (_hasher.VerifyHashedPassword(u, u.PasswordHash, password))
        {
            case PasswordVerificationResult.Failed:
                return false;
            case PasswordVerificationResult.Success:
                return true;
            case PasswordVerificationResult.SuccessRehashNeeded:
                await RehashPassword(u.Login, password);
                return true;
            default:
                return false;
        }
    }

    public async Task UpdatePassword(User u, string oldPassword, string newPassword)
    {
        if (await PasswordIsCorrect(u, oldPassword))
		{
			u.PasswordHash = _hasher.HashPassword(u, newPassword);
			await _database.Users.ReplaceOneAsync(FilterByLogin(u.Login), u);
		}
		else
		{
			throw new AuthServiceException($"Incorrect password");
		}
    }
    
    private FilterDefinition<User> FilterByLogin(string login)
    {
        return Builders<User>.Filter.Eq(nameof(User.Login), login);
    }

    private async Task RehashPassword(string login, string password)
    {
        User u = await GetUser(login);
        string rehash = _hasher.HashPassword(u, password);
        await _database.Users.UpdateOneAsync(FilterByLogin(u.Login),
            Builders<User>.Update.Set(nameof(User.PasswordHash), rehash));
    }
    
}