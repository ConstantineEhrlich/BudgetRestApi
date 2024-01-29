using MongoDB.Bson;

namespace AuthService.Models;

public class User
{   
    public string Login { get; set; }
    public string Email { get; set; }
    
    public string PasswordHash { get; set; }
    public DateTime? LastSuccessLogin { get; set; }
    public DateTime? LastFailLogin { get; set; }
    public int FailedLoginCount { get; set; }
    
    public ObjectId ProfileId { get; set; }
}