namespace BudgetModel.Models;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }

    public string Email { get; set; } = string.Empty;

    public virtual ICollection<Transaction>? AuthoredTransactions { get; set; }
    public virtual ICollection<Transaction>? Transactions { get; set; }

    public virtual ICollection<BudgetFile>? BudgetFiles { get; set; } = new List<BudgetFile>();


    public User(string id, string name)
    {
        Id = id;
        Name = name;
    }
}