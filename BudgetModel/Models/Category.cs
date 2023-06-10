namespace BudgetModel.Models;

public class Category
{
    public string Id { get; set; }
    public string Description { get; set; }
    
    // Navigation properties
    public  virtual  ICollection<Transaction>? Transactions { get; set; }

    public Category(string id, string description)
    {
        Id = id;
        Description = description;
    }
}