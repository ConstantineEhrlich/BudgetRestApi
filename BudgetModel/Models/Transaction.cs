using System.Text.Json.Serialization;
using BudgetModel.Enums;
using BudgetModel.Interfaces;

namespace BudgetModel.Models;

public class Transaction: IPeriodic
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public string BudgetFileId { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
    public DateTime RecordedAt { get; set; }
    public string OwnerId { get; set; } = null!;
    public string AuthorId { get; set; } = null!;
    public int Year { get; set; }
    public int Period { get; set; }

    public string Description { get; set; } = string.Empty;
    public string CategoryId { get; set; } = null!;
    public decimal Amount { get; set; }
    
    // Navigation properties
    [JsonIgnore]
    public virtual User? Owner { get; set; }
    
    [JsonIgnore]
    public virtual User? Author { get; set; }
    
    [JsonIgnore]
    public virtual Category? Category { get; set; }
    
    [JsonIgnore]
    public virtual BudgetFile? BudgetFile { get; set; }

    protected Transaction()
    {
        
    }
    public Transaction(BudgetFile budget,
                       User owner,
                       User author,
                       DateTime date,
                       TransactionType type,
                       Category cat,
                       string description,
                       decimal amount)
    {
        BudgetFileId = budget.Id;
        Type = type;
        Date = date;
        RecordedAt = DateTime.UtcNow;
        OwnerId = owner.Id;
        AuthorId = author.Id;
        Year = Date.Year;
        Period = Date.Month;
        CategoryId = cat.Id;
        Description = description;
        Amount = amount;
    }

    public Transaction(BudgetFile budget,
                       User author,
                       TransactionType type,
                       Category cat,
                       string description,
                       decimal amount) 
          : this(budget, author, author, DateTime.Now, type, cat, description, amount) { }
}