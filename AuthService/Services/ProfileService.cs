using AuthService.Models;
using AuthService.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AuthService.Services;

public class ProfileService
{
    private readonly UsersDatabase _database;
    private readonly ILogger<ProfileService> _logger;
    private readonly UserService _users;

    public ProfileService(ILogger<ProfileService> logger, UsersDatabase database, UserService users)
    {
        _database = database;
        _logger = logger;
        _users = users;
    }

    public async Task<Profile> GetProfile(string login)
    {
        User u = await _users.GetUser(login);
        Profile p = await _database.Profiles.Find(Builders<Profile>.Filter.Eq(nameof(Profile.Id), u.ProfileId)).FirstOrDefaultAsync();
        return p;
    }
    
    public async Task UpdateProfile(Profile profile)
    {
        Profile target = await GetProfile(profile.Login);
        await _database.Profiles.ReplaceOneAsync(Builders<Profile>.Filter.Eq(nameof(Profile.Id), profile.Id), profile);
    }
}