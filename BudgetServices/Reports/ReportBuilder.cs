using BudgetModel.Extensions;
using BudgetModel.Models;

namespace BudgetServices.Reports;

public abstract class ReportBuilder: IReportBuilder
{
    public IQueryable<Transaction> Transactions { get; set; } = null!;

    public IReportBuilder ForPeriod(int year, int period)
    {
        Transactions = Transactions.Where(t => t.Year == year && t.Period == period);
        return this;
    }

    public IReportBuilder FromPeriod(int year, int period)
    {
        Transactions = Transactions.StartingFrom(year, period);
        return this;
    }

    public IReportBuilder ToPeriod(int year, int period)
    {
        Transactions = Transactions.UpToPeriod(year, period);
        return this;
    }

    public IReportBuilder ForCategory(string categoryId)
    {
        Transactions = Transactions.Where(t => t.CategoryId == categoryId);
        return this;
    }

    public IReportBuilder ForCategories(ICollection<string> categories)
    {
        Transactions = Transactions.Where(t => categories.Contains(t.CategoryId));
        return this;
    }

    public IReportBuilder ForOwner(string ownerId)
    {
        Transactions = Transactions.Where(t => t.OwnerId == ownerId);
        return this;
    }

    public IReportBuilder ForOwners(ICollection<string> owners)
    {
        Transactions = Transactions.Where(t => owners.Contains(t.OwnerId));
        return this;
    }

    public abstract IQueryable<object> Summarize();
}