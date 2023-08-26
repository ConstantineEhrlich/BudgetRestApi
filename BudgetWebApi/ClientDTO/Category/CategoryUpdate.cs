using System.ComponentModel.DataAnnotations;
namespace BudgetWebApi.ClientDto;

public class CategoryUpdate
{
    public string Description { get; set; }
    public int DefaultType { get; set; }
}