using System.ComponentModel.DataAnnotations;
namespace BudgetWebApi.ClientDto;

public class CategoryAdd
{
    [Required] public string CategoryId { get; set; }
    [Required] public string Description { get; set; }
}