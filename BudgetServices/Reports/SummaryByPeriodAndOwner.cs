namespace BudgetServices.Reports;

public class SummaryByPeriodAndOwner: ReportBuilder
{
    public override IQueryable<object> Summarize() => 
        Transactions.GroupBy(t => new { t.Year, t.Period, t.Type, t.OwnerId })
                    .Select(group => new
                    {
                        group.Key.Year,
                        group.Key.Period,
                        group.Key.Type,
                        group.Key.OwnerId,
                        Amount = group.Sum(t => t.Amount)
                    });
}