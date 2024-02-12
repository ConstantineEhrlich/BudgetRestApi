using System.ComponentModel.DataAnnotations;

namespace BudgetWebApi.Dto;

public class SignUp
{
    [Required] public string Id { get; set; } = null!;
    [Required] public string Name { get; set; } = null!;
    [Required] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
}