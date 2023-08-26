using System.ComponentModel.DataAnnotations;

namespace BudgetWebApi.ClientDto;

public class SignIn
{
    [Required] public string UserId { get; set; }
    [Required] public string Password { get; set; }
}