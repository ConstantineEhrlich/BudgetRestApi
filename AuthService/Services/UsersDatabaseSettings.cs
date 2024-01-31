using MongoDB.Driver;

namespace AuthService.Services;

public class UsersDatabaseSettings
{
    public string MongoServer { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string DbUser { get; set; } = null!;
    public string UsersCollectionName { get; set; } = null!;
    public string ProfilesCollectionName { get; set; } = null!;
    public MongoUrl ConnectionUrl
    {
        get
        {
            string mongoPassword = System.Environment.GetEnvironmentVariable("MONGO_PASSWORD") 
                                   ?? throw new KeyNotFoundException("MONGO_PASSWORD variable not set");
            
            MongoUrlBuilder builder = new()
            {
                Server = new MongoServerAddress(MongoServer, 27017),
                Username = "user",
                Password = mongoPassword
            
            };
            return builder.ToMongoUrl();
        }
    }
}