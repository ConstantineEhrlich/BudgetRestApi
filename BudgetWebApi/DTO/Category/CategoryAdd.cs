using System.ComponentModel.DataAnnotations;
using BudgetModel.Enums;

namespace BudgetWebApi.Dto;

public class CategoryAdd
{
    [Required] public string? CategoryId { get; set; }
    [Required] public string? Description { get; set; }
    [Required] public TransactionType DefaultType { get; set; }
}