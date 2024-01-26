using BudgetModel.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;


namespace BudgetModel;

public class Context : DbContext
{
    public DbSet<User>? Users { get; internal set; }
    public DbSet<Transaction>? Transactions { get; internal set; }
    public DbSet<Category>? Categories { get; internal set; }
    public DbSet<BudgetFile>? Budgets { get; internal set; }

    public Context(DbContextOptions<Context> options) : base(options) {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
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

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Date)
            .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<Transaction>()
            .Property(t => t.RecordedAt)
            .HasConversion(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        modelBuilder.Entity<Category>()
            .HasKey(category => new { category.BudgetFileId, category.Id });

        modelBuilder.Entity<Category>()
            .HasOne(cat => cat.BudgetFile)
            .WithMany(budgetFile => budgetFile.Categories)
            .HasForeignKey(cat => cat.BudgetFileId);

    }
    public static string GetPostgresConnectionString()
    {
        string server = System.Environment.GetEnvironmentVariable("POSTGRES_SERVER") ??
                        throw new KeyNotFoundException("POSTGRES_SERVER variable not set!");
        string password = System.Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ??
                        throw new KeyNotFoundException("POSTGRES_PASSWORD variable not set!");

        NpgsqlConnectionStringBuilder builder = new()
        {
            Host = server,
            Port = 5432,
            Database = "budget",
            Username = "user",
            Password = password,
        };

        return builder.ConnectionString;
    }
}