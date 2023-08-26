using System.Text.Json.Serialization;

namespace BudgetModel.Models;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; } = string.Empty;

    [JsonIgnore] public int FailedLoginCount { get; set; } = 0;
    [JsonIgnore] public DateTime? LastFailedLogin { get; set; } = null;
    
    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    [JsonIgnore]
    public virtual ICollection<Transaction>? AuthoredTransactions { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<Transaction>? Transactions { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<BudgetFile> BudgetFiles { get; set; } = new List<BudgetFile>();


    public User(string id, string name, string email = "")
    {
        Id = id;
        Name = name;
        Email = email;
    }
}