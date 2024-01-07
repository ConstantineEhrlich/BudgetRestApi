using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BudgetModel;

public class DesignContextFactory: IDesignTimeDbContextFactory<Context>
{
    public Context CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<Context> optsBuilder = new();
        optsBuilder.UseNpgsql(Context.GetPostgresConnectionString());
        return new Context(optsBuilder.Options);
    }
}