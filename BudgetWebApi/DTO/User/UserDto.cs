using BudgetModel.Models;

namespace BudgetWebApi.Dto;

public class UserDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }

    public UserDto(User u)
    {
        Id = u.Id;
        Name = u.Name;
        Email = u.Email;
    }
}