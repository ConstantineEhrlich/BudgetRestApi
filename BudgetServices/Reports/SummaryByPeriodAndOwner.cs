namespace BudgetServices.Reports;

public class SummaryByPeriodAndOwner: ReportBuilder
{
    public override IQueryable<object> Summarize() => 
        Transactions.GroupBy(t => new { t.Year, t.Period, t.Type, t.OwnerId })
                    .Select(group => new
                    {
                        Year = group.Key.Year,
                        Period = group.Key.Period,
                        Type = group.Key.Type,
                        OwnerId = group.Key.OwnerId,
                        Amount = group.Sum(t => t.Amount)
                    });
}