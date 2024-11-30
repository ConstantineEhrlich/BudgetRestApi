namespace BudgetServices.Reports;

public class ReportFactory: IReportFactory
{
    private readonly TransactionService _transactions;
    private readonly BudgetFileService _budgets;

    public ReportFactory(TransactionService transactionService, BudgetFileService budgetFileService)
    {
        _transactions = transactionService;
        _budgets = budgetFileService;
    }
    
    public IReportBuilder CreateReport<T>(string budgetId, string? requestingUserId) where T : IReportBuilder, new()
    {
        return new T()
        {
            Transactions = _transactions.GetAllTransactions(budgetId, requestingUserId).Result,
            Budget = _budgets.GetBudgetFile(budgetId, requestingUserId).Result,
        };
    }
}