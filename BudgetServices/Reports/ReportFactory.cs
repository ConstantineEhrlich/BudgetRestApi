using Microsoft.Extensions.DependencyInjection;

namespace BudgetServices.Reports;

public class ReportFactory: IReportFactory
{
    private readonly IServiceProvider _provider;

    public ReportFactory(IServiceProvider provider)
    {
        _provider = provider;
    }
    
    public IReportBuilder CreateReport<T>(string budgetId, string? requestingUserId) where T : IReportBuilder, new()
    {
        //TransactionService service = _provider.CreateScope().ServiceProvider.GetRequiredService<TransactionService>();
        TransactionService service = _provider.GetRequiredService<TransactionService>();
        return new T()
        {
            Transactions = service.GetAllTransactions(budgetId, requestingUserId)
        };
    }
}