namespace BudgetModel.Models;

public class Category
{
    public int BudgetFileId { get; set; }
    public string Id { get; set; }
    public string Description { get; set; }
    
    public virtual BudgetFile BudgetFile { get; set; }
    public Category(int budgetFileId, string id, string description)
    {
        Id = id;
        BudgetFileId = budgetFileId;
        Description = description;
    }
}