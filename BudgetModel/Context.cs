using BudgetModel.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;


namespace BudgetModel;

public class Context : DbContext
{
    private string _connStr;

    public DbSet<User> Users { get; internal set; }
    public DbSet<Transaction> Transactions { get; internal set; }
    public DbSet<Category> Categories { get; internal set; }
    public DbSet<BudgetFile> Budgets { get; internal set; }
    public Context(string dbPath, bool loadingMode = false)
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
        optionsBuilder.UseSqlite(_connStr);
        optionsBuilder.UseLazyLoadingProxies();
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .ValueGeneratedOnAdd();
        
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Transaction>()
            .HasOne(tr => tr.Owner)
            .WithMany(u => u.Transactions)
            .HasForeignKey(tr => tr.OwnerId);
        
        modelBuilder.Entity<Transaction>()
            .HasOne(tr => tr.Author)
            .WithMany(u => u.AuthoredTransactions)
            .HasForeignKey(tr => tr.AuthorId);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasConversion<Double>();


        modelBuilder.Entity<BudgetFile>()
            .Property(bf => bf.Id)
            .ValueGeneratedOnAdd();
        
    }
    
}