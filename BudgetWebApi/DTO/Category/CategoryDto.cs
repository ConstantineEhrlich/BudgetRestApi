using BudgetModel.Enums;
using BudgetModel.Models;

namespace BudgetWebApi.Dto;

public class CategoryDto
{
    public string? BudgetFileId { get; set; }
    public string? Id { get; set; }
    public TransactionType? DefaultType { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }

    public CategoryDto(Category c)
    {
        BudgetFileId = c.BudgetFileId;
        Id = c.Id;
        DefaultType = c.DefaultType;
        Description = c.Description;
        IsActive = c.IsActive;
    }
    
}