using BudgetModel.Enums;
using BudgetModel.Models;

namespace BudgetWebApi.Dto;

public class TransactionDto
{
    public string? Id { get; set; }
    public string? BudgetFileId { get; set; }
    public TransactionType? Type { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? RecordedAt { get; set; }
    public string? OwnerId { get; set; }
    public string? AuthorId { get; set; }
    public int? Year { get; set; }
    public int? Period { get; set; }
    public string? Description { get; set; }
    public string? CategoryId { get; set; }
    public decimal? Amount { get; set; }

    public TransactionDto(Transaction t)
    {
        Id = t.Id;
        BudgetFileId = t.BudgetFileId;
        Type = t.Type;
        Date = t.Date;
        RecordedAt = t.RecordedAt;
        OwnerId = t.OwnerId;
        AuthorId = t.AuthorId;
        Year = t.Year;
        Period = t.Period;
        Description = t.Description;
        CategoryId = t.CategoryId;
        Amount = t.Amount;
    }
}