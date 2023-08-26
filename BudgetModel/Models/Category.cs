using System.Text.Json.Serialization;
using BudgetModel.Enums;

namespace BudgetModel.Models;

public class Category
{
    public string BudgetFileId { get; set; }
    public string Id { get; set; }

    public TransactionType DefaultType { get; set; } = TransactionType.Expense;
    public string Description { get; set; }

    public bool IsActive { get; set; } = true;
    
    [JsonIgnore]
    public virtual BudgetFile? BudgetFile { get; set; }

    public Category(string budgetFileId, string id, string description)
    {
        Id = id;
        BudgetFileId = budgetFileId;
        Description = description;
    }
}