namespace BudgetWebApi.Dto;

public class CategoryUpdate
{
    public string Description { get; set; } = string.Empty;
    public int DefaultType { get; set; }
}