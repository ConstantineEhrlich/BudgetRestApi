using Microsoft.Extensions.Options;
using MongoDB.Driver;
using AuthService.Models;

namespace AuthService.Services;

public class UsersDatabase
{
    private readonly MongoClient _client;
    private readonly IOptions<UsersDatabaseSettings> _settings;

    public UsersDatabase(IOptions<UsersDatabaseSettings> usersDbSettings)
    {
        _settings = usersDbSettings;
        _client = new MongoClient(_settings.Value.ConnectionUrl);

    }
    
    public IMongoDatabase Database
    {
        get => _client.GetDatabase(_settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users
    {
        get =>  Database.GetCollection<User>(_settings.Value.UsersCollectionName);
    }

    public IMongoCollection<Profile> Profiles
    {
        get => Database.GetCollection<Profile>(_settings.Value.ProfilesCollectionName);
    }
}