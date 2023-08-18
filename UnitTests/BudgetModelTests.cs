using System.Text;
using BudgetModel.Enums;
using static UnitTests.Helpers;
using BudgetModel.Models;

namespace UnitTests;

[TestClass]
public class BudgetModelTests
{
    [TestMethod]
    public void CreateUsers()
    {
        var context = GetContext(true);

        User[] users =
        {
            new User("john", "John Wilson"),
            new User("jack", "Jack Anderson"),
            new User("mary", "Mary Sanders"),
            new User("jane", "Jane Tyler"),
            new User("kent", "Kent Paul"),
        };

        foreach (User u in users)
        {
            if (context.Users.FirstOrDefault(user => user.Id == u.Id) is null)
            {
                context.Add(u);
            }
            else
            {
                Console.WriteLine($"User with id {u.Id} already exists!");
            }
        }

        context.SaveChanges();
    }

    [TestMethod]
    public void CreateCategories()
    {
        var context = GetContext();
        Category[] cats =
        {
            new Category("salary", "Salary"),
            new Category("transfer", "Transfer"),
            new Category("rent", "Rent"),
            new Category("municipal", "Municipal"),
            new Category("loans", "Loans"),
            new Category("health", "Health"),
            new Category("transport", "Transport"),
            new Category("digicom", "Digicom"),
            new Category("gadgets", "Gadgets"),
            new Category("savings", "Savings"),
            new Category("food", "Food"),
            new Category("pharm", "Pharm"),
            new Category("leisure", "Leisure"),
            new Category("hns", "H&S"),
            new Category("other", "Other"),
        };
        
        foreach (Category cat in cats)
        {
            if (context.Categories.FirstOrDefault(c => c.Id == cat.Id) is null)
            {
                context.Add(cat);
            }
            else
            {
                Console.WriteLine($"Category {cat.Description} already exists!");
            }
        }

        context.SaveChanges();
    }

    [TestMethod]
    public void CreateBudgetFile()
    {
        var context = GetContext();
        BudgetFile budget = new("Default budget");
        foreach (User u in context.Users)
        {
            budget.Owners.Add(u);
        }
        context.Add(budget);
        context.SaveChanges();
    }

    [TestMethod]
    public void CreateTransactions()
    {
        Random rnd = new();
        var context = GetContext();

        List<Category> cats = context.Categories.ToList();
        List<User> users = context.Users.ToList();
        BudgetFile budget = context.Budgets.FirstOrDefault();
        if (budget is null)
            Assert.Fail("There are no budgets defined!");

        int transactions = rnd.Next(10, 100);
        Console.WriteLine($"Creating {transactions} transactions");
        for (int i = 0; i < transactions; i++)
        {
            User owner = users[rnd.Next(0, users.Count)];
            User author = users[rnd.Next(0, users.Count)];
            Category cat = cats[rnd.Next(0, cats.Count)];
            decimal amt = rnd.Next(10, 5000);

            int peri = rnd.Next(1, 12);
            int day = rnd.Next(1, DateTime.DaysInMonth(2023, peri));
            int fiscPeri = rnd.Next(1, 12);

            TransactionType type = cat.Id switch
            {
                "salary" => TransactionType.Income,
                "transfer" => TransactionType.Income,
                _ => TransactionType.Expense,
            };
            
            Transaction t = new(budget,
                                owner, 
                                author, 
                                new DateTime(2023, peri, day, 12, 00, 00),
                                type,
                                cat,
                                amt);
            
            context.Add(t);
        }

        context.SaveChanges();
    }

    [TestMethod]
    public void ReadUsers()
    {
        var context = GetContext();
        
        Console.WriteLine($"{"Full name", -20}{"Id", -15}");
        foreach (User u in context.Users)
        {
            Console.WriteLine($"{u.Name, -20}{u.Id, -15}");
        }
    }
    
    [TestMethod]
    public void ReadTransactions()
    {
        var context = GetContext();
        
        Console.WriteLine($"{"Id", -4}{"Date", -15}{"Year", -7}{"Period", -7}{"Spent By", -15}{"Amount", -10}");
        foreach (Transaction t in context.Transactions)
        {
            Console.WriteLine($"{t.Id, 4}{t.RecordedAt, -15:dd-MMM-yy}{t.Year, 7}{t.Period, 7}{t.Owner?.Name, -15}{t.Amount,-10:F2}");
        }
        
    }

    [TestMethod]
    public void ReadOwners()
    {
        var context = GetContext();
        Console.WriteLine($"{"Id", -4}{"Description", -20}{"Owner Id", -15}{"Owner Name", -20}");
        foreach (BudgetFile budget in context.Budgets)
        {
            foreach (User u in budget.Owners)
            {
                Console.WriteLine($"{budget.Id, 4}{budget.Description, -20}{u.Id, -15}{u.Name, -20}");
            }
        }
    }
}