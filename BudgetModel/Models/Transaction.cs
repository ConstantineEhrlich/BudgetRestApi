using BudgetModel.Enums;
using BudgetModel.Interfaces;

namespace BudgetModel.Models;

public class Transaction: IPeriodic
{
    public int Id { get; set; }
    public TransactionType Type { get; set; }
    public DateTime Date { get; set; }
    public DateTime RecordedAt { get; set; }
    public string OwnerId { get; set; }
    public string AuthorId { get; set; }
    public int Year { get; set; }
    public int Period { get; set; }
    
    public string CategoryId { get; set; }
    public decimal Amount { get; set; }
    
    // Navigation properties
    public virtual User? Owner { get; set; }
    public virtual User? Author { get; set; }
    
    public virtual Category? Category { get; set; }

    protected Transaction()
    {
        
    }
    public Transaction(User owner, User author, DateTime date, TransactionType type, string categoryId, decimal amount)
    {
        Type = type;
        Date = date;
        RecordedAt = DateTime.Now;
        OwnerId = owner.Id;
        AuthorId = author.Id;
        Year = Date.Year;
        Period = Date.Month;
        CategoryId = categoryId;
        Amount = amount;
    }

    public Transaction(User author, TransactionType type, string categoryId, decimal amount) 
        : this(author, author, DateTime.Now, type, categoryId, amount)
    {
    }
}