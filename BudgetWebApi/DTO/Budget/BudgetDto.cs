namespace BudgetWebApi.Dto;

public class BudgetDto
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? IsPrivate { get; set; }
}