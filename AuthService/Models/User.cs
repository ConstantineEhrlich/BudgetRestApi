using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthService.Models;

public class User
{   
    [BsonId]
    public string Login { get; set; }
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }
    public DateTime? LastSuccessLogin { get; set; }
    public DateTime? LastFailedLogin { get; set; }
    public int FailedLoginCount { get; set; }
    
    public ObjectId ProfileId { get; set; }
}