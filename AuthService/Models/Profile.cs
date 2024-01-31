using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace AuthService.Models;

public class Profile
{
    [JsonIgnore]
    public ObjectId Id { get; set; }
    
    public string? Login { get; set; }
    public string? FullName { get; set; }
    public string? Signature { get; set; }
    public string? PublicEmail { get; set; }
}