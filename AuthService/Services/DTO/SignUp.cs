using System.ComponentModel.DataAnnotations;

namespace AuthService.Services.Dto;

public class SignUp
{
    [Required] public string Login { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string Name { get; set; }
    [Required] public string Email { get; set; }
}