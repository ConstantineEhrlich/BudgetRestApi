using System.ComponentModel.DataAnnotations;

namespace BudgetWebApi.Dto;

public class SignIn
{
    [Required] public string UserId { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
    
}