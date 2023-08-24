using BudgetModel;
using BudgetModel.Enums;
using BudgetModel.Models;

namespace BudgetDataLoader;

public class SampleDataCreator
{
    private readonly Config _cfg;
    private Context? _context;

    public SampleDataCreator(Config cfg)
    {
        _cfg = cfg;
    }

    public void Create()
    {
        CreateDatabase();
        if (_context is not null)
        {
            CreateUsers();
            CreateBudgetFiles();
            CreateCategories();
            CreateTransactions();            
        }
        
    }

    private void CreateDatabase()
    {
        if (_cfg.DeleteDatabase && File.Exists(_cfg.DatabasePath))
        {
            File.Delete(_cfg.DatabasePath);
        }

        if (!Directory.Exists(Path.GetDirectoryName(_cfg.DatabasePath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_cfg.DatabasePath)!);
        }

        _context = new(_cfg.DatabasePath);
        _context.Database.EnsureCreated();
    }
    
    private void CreateUsers()
    {
        Console.WriteLine("Creating users...");
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
            if (_context!.Users.FirstOrDefault(user => user.Id == u.Id) is null)
            {
                _context.Add(u);
            }
            else
            {
                Console.WriteLine($"User with id {u.Id} already exists!");
            }
        }

        _context!.SaveChanges();
    }

    private void CreateBudgetFiles()
    {
        Console.WriteLine("Creating budget files...");
        BudgetFile b1 = new("Family budget");
        BudgetFile b2 = new("Project budget");
        foreach (User u in _context!.Users)
        {
            b1.Owners!.Add(u);
            b2.Owners!.Add(u);
        }
        _context.Add(b1);
        _context.Add(b2);
        _context.SaveChanges();
    }

    private void CreateCategories()
    {
        Console.WriteLine("Creating categories");
        Category[] familyCats =
        {
            new Category(1, "salary", "Salary"),
            new Category(1, "transfer", "Transfer"),
            new Category(1, "rent", "Rent"),
            new Category(1, "municipal", "Municipal"),
            new Category(1, "loans", "Loans"),
            new Category(1, "health", "Health"),
            new Category(1, "transport", "Transport"),
            new Category(1, "digicom", "Digicom"),
            new Category(1, "gadgets", "Gadgets"),
            new Category(1, "savings", "Savings"),
            new Category(1, "food", "Food"),
            new Category(1, "pharm", "Pharm"),
            new Category(1, "leisure", "Leisure"),
            new Category(1, "hns", "H&S"),
            new Category(1, "other", "Other"),
        };

        Category[] projectCats =
        {
            new Category(2, "revenue", "Revenue"),
            new Category(2, "labor", "Labor"),
            new Category(2, "material", "Material"),
            new Category(2, "subcontractor", "Subcontractor"),
        };
        
        _context!.Categories.AddRange(familyCats);
        _context.Categories.AddRange(projectCats);
        _context.SaveChanges();
    }

    private void CreateTransactions()
    {
        List<Transaction> transactions = new();
        Random rnd = new();
        int transCount = rnd.Next(10, 100);
        Console.WriteLine($"Creating {transCount} transactions");

        for (int i = 0; i < transCount; i++)
        {
            User owner = _context!.Users.GetRandom();
            User author = _context.Users.GetRandom();
            BudgetFile budget = _context.Budgets.GetRandom();
            Category cat = budget.Categories.GetRandom();
            decimal amt = rnd.Next(10, 5000);

            int peri = rnd.Next(1, 12);
            int day = rnd.Next(1, DateTime.DaysInMonth(2023, peri));
            
            TransactionType type = cat.Id switch
            {
                "salary" => TransactionType.Income,
                "transfer" => TransactionType.Income,
                "revenue" => TransactionType.Income,
                _ => TransactionType.Expense,
            };
            
            Transaction t = new(budget,
                owner, 
                author, 
                new DateTime(2023, peri, day, 12, 00, 00),
                type,
                cat,
                amt);


            transactions.Add(t);
            _context.Add(t);
            _context.SaveChanges();
        }
        //_context.AddRange(transactions);
        //_context!.SaveChanges();
    }
}

internal static class DbSetExtension
{
    public static T GetRandom<T>(this IEnumerable<T> enumerable)
    {
        Random rnd = new();
        List<T> lst = enumerable.ToList();
        return lst.Skip(rnd.Next(0, lst.Count - 1)).Take(1).First();
    }
}