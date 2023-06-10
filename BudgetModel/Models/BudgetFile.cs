namespace BudgetModel.Models;

public class BudgetFile
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    
    // Navigation properties
    public virtual ICollection<Transaction>? Transactions { get; set; }
    public virtual ICollection<User>? Owners { get; set; } = new List<User>();


    protected BudgetFile(){}

    public BudgetFile(string description)
    {
        Description = description;
    }
}