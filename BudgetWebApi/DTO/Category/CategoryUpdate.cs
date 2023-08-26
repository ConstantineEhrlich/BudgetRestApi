using System.ComponentModel.DataAnnotations;
namespace BudgetWebApi.Dto;

public class CategoryUpdate
{
    public string Description { get; set; }
    public int DefaultType { get; set; }
}