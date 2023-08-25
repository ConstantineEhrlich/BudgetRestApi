using BudgetModel.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


namespace BudgetModel;

public class Context : DbContext
{
    private readonly string _connStr = null!;
   public DbSet<User> Users { get; internal set; }
    public DbSet<Transaction> Transactions { get; internal set; }
    public DbSet<Category> Categories { get; internal set; }
    public DbSet<BudgetFile> Budgets { get; internal set; }

    public Context(DbContextOptions<Context> options) : base(options)
    {
        
    } 

    public Context(string? dbPath, bool loadingMode = false)
    {
        SqliteConnectionStringBuilder connStr = new()
        {
            DataSource = dbPath,
            ForeignKeys = !loadingMode,
        };
        _connStr = connStr.ConnectionString;
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_connStr is not null)
        {
            optionsBuilder.UseSqlite(_connStr);
        }
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .HasOne(tr => tr.Owner)
            .WithMany(u => u.Transactions)
            .HasForeignKey(tr => tr.OwnerId);
        
        modelBuilder.Entity<Transaction>()
            .HasOne(tr => tr.Author)
            .WithMany(u => u.AuthoredTransactions)
            .HasForeignKey(tr => tr.AuthorId);

        modelBuilder.Entity<Transaction>()
            .HasOne(tr => tr.Category)
            .WithMany()
            .HasForeignKey(tr => new { tr.BudgetFileId, tr.CategoryId });
        
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasConversion<double>();


        modelBuilder.Entity<Category>()
            .HasKey(category => new { category.BudgetFileId, category.Id });

        modelBuilder.Entity<Category>()
            .HasOne(cat => cat.BudgetFile)
            .WithMany(budgetFile => budgetFile.Categories)
            .HasForeignKey(cat => cat.BudgetFileId);

    }
    
}