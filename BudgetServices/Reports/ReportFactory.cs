namespace BudgetServices.Reports;

public class ReportFactory: IReportFactory
{
    private readonly TransactionService _transactions;

    public ReportFactory(TransactionService transactionService)
    {
        _transactions = transactionService;
    }
    
    public IReportBuilder CreateReport<T>(string budgetId, string? requestingUserId) where T : IReportBuilder, new()
    {
        return new T()
        {
            Transactions = _transactions.GetAllTransactions(budgetId, requestingUserId).Result
        };
    }
}