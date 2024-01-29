using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AuthService.Models;

public class UsersDatabase
{
    private readonly MongoClient _client;
    private readonly IOptions<UsersDatabaseSettings> _settings;

    public UsersDatabase(IOptions<UsersDatabaseSettings> usersDbSettings)
    {
        _settings = usersDbSettings;
        _client = new MongoClient(_settings.Value.ConnectionUrl);
        // Define unique index for the User type 
        IndexKeysDefinition<User> indexKeys = Builders<User>.IndexKeys.Ascending(user => user.Login);
        Users.Indexes.CreateOne(new CreateIndexModel<User>(indexKeys, new(){Unique = true}));

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