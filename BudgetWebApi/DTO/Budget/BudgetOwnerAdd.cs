using System.ComponentModel.DataAnnotations;

namespace BudgetWebApi.Dto;

public class BudgetOwnerAdd
{
    [Required] public string? UserId { get; set; }
}