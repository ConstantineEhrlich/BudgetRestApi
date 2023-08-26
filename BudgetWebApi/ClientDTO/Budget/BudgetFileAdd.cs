using System.ComponentModel.DataAnnotations;

namespace BudgetWebApi.ClientDto;

public class BudgetFileAdd
{
    [Required] public string Description { get; set; }
}