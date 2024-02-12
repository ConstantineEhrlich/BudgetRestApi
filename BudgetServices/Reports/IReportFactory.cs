namespace BudgetServices.Reports;

public interface IReportFactory
{
    IReportBuilder CreateReport<T>(string budgetId, string? requestingUserId) where T: IReportBuilder, new();
}