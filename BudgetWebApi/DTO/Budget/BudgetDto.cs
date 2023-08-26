using BudgetModel.Models;

namespace BudgetWebApi.Dto;

public class BudgetDto
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public bool? IsPrivate { get; set; }
    
    public List<UserDto> Owners { get; set; }

    public BudgetDto(BudgetFile b)
    {
        Id = b.Id;
        Slug = b.Slug;
        Description = b.Description;
        IsPrivate = b.IsPrivate;
        Owners = b.Owners.Select(o => new UserDto(o)).ToList();
    }
    
}