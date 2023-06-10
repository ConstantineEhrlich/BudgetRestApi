using BudgetModel.Enums;
using BudgetModel.Interfaces;

namespace BudgetModel.Models;

public class Transaction: IPeriodic
{
    public int Id { get; set; }
    
    public int BudgetFileId { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
    public DateTime RecordedAt { get; set; }
    public string OwnerId { get; set; } = null!;
    public string AuthorId { get; set; } = null!;
    public int Year { get; set; }
    public int Period { get; set; }

    public string CategoryId { get; set; } = null!;
    public decimal Amount { get; set; }
    
    // Navigation properties
    public virtual User? Owner { get; set; }
    public virtual User? Author { get; set; }
    
    public virtual Category? Category { get; set; }
    
    public virtual BudgetFile? BudgetFile { get; set; }

    protected Transaction()
    {
        
    }
    public Transaction(BudgetFile budget, User owner, User author, DateTime date, TransactionType type, Category cat, decimal amount)
    {
        BudgetFileId = budget.Id;
        Type = type;
        Date = date;
        RecordedAt = DateTime.Now;
        OwnerId = owner.Id;
        AuthorId = author.Id;
        Year = Date.Year;
        Period = Date.Month;
        CategoryId = cat.Id;
        Amount = amount;
    }

    public Transaction(BudgetFile budget, User author, TransactionType type, Category cat, decimal amount) 
        : this(budget, author, author, DateTime.Now, type, cat, amount)
    {
    }
}