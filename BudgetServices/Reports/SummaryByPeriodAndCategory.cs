namespace BudgetServices.Reports;

public class SummaryByPeriodAndCategory: ReportBuilder
{
    public override IQueryable<object> Summarize() =>
        Transactions.GroupBy(t => new { t.Year, t.Period, t.Type, t.CategoryId })
                    .Select(group => new
                    {
                        Year = group.Key.Year,
                        Period = group.Key.Period,
                        Type = group.Key.Type,
                        CategoryId = group.Key.CategoryId,
                        Count = group.Count(),
                        Amount = group.Sum(t => t.Amount)
                    });
}