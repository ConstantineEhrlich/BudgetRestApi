namespace BudgetServices.Reports;

public class SummaryByPeriodAndCategory: ReportBuilder
{
    public override IQueryable<object> Summarize() =>
        Transactions.GroupBy(t => new { t.Year, t.Period, t.Type, t.CategoryId })
                    .Select(group => new
                    {
                        group.Key.Year,
                        group.Key.Period,
                        group.Key.Type,
                        group.Key.CategoryId,
                        Count = group.Count(),
                        Amount = group.Sum(t => t.Amount)
                    });
}