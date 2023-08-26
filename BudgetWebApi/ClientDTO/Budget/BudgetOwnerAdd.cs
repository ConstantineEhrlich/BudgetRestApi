using System.ComponentModel.DataAnnotations;

namespace BudgetWebApi.ClientDto;

public class BudgetOwnerAdd
{
    [Required] public string UserId { get; set; }
}