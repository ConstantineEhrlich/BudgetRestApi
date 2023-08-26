using System.ComponentModel.DataAnnotations;

namespace BudgetWebApi.ClientDto;

public class BudgetFileUpdate
{
    public string? Description { get; set; }
    public bool? IsPrivate { get; set; }
}