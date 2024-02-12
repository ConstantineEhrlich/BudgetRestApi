using System.ComponentModel.DataAnnotations;

namespace BudgetWebApi.Dto;

public class BudgetFileAdd
{
    [Required] public string? Description { get; set; }
}