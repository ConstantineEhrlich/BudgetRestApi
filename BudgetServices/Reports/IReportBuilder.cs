using BudgetModel.Models;

namespace BudgetServices.Reports;

public interface IReportBuilder
{
    IQueryable<Transaction> Transactions { get; set; }
    IQueryable<object> Summarize();
    
    IReportBuilder ForPeriod(int year, int period);
    IReportBuilder FromPeriod(int year, int period);
    IReportBuilder ToPeriod(int year, int period);

    IReportBuilder ForCategory(string categoryId);
    IReportBuilder ForCategories(ICollection<string> categories);

    IReportBuilder ForOwner(string ownerId);
    IReportBuilder ForOwners(ICollection<string> owners);
}