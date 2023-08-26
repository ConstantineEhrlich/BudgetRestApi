using System.Text.Json.Serialization;

namespace BudgetModel.Models;

public class BudgetFile
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = null!;

    public bool IsDeleted { get; set; } = false;

    public bool IsPrivate { get; set; } = false;
    
    // Navigation properties
    [JsonIgnore]
    public virtual ICollection<Transaction>? Transactions { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<User> Owners { get; set; } = new List<User>();

    [JsonIgnore]
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    
    protected BudgetFile(){}

    public BudgetFile(string description)
    {
        Description = description;
    }
    
    
    

}