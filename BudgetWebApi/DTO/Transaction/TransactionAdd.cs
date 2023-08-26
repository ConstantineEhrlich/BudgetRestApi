using System.ComponentModel.DataAnnotations;

namespace BudgetWebApi.Dto;

public class TransactionAdd
{
    [Required] public string CategoryId { get; set; }
    [Required] public decimal Amount { get; set; }
    public string? Description { get; set; }
    public int? TransactionType { get; set; }
    public string? OwnerId { get; set; }
    public DateTime? Date { get; set; }
    public int? Year { get; set; }
    public int? Period { get; set; }
    
}