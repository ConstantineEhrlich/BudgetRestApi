namespace BudgetModel.Models;

public class BudgetFile
{
    public int Id { get; set; }
    public string Slug { get; set; }
    public string Description { get; set; } = null!;
    
    // Navigation properties
    public virtual ICollection<Transaction>? Transactions { get; set; }
    public virtual ICollection<User>? Owners { get; set; } = new List<User>();

    public virtual ICollection<Category>? Categories { get; set; }
    
    protected BudgetFile(){}

    public BudgetFile(string description)
    {
        Description = description;
        Slug = CreateSlug(description);
    }
    
    
    public static string CreateSlug(string description)
    {
        string[] words = description.Split(' ').Take(3).ToArray();
        string slug = string.Join("-", words).ToLower();
        return slug;
    }

}