using BudgetModel.Enums;
using BudgetModel.Models;

namespace BudgetServices.Reports;

public class BalanceToPeriodReport: ReportBuilder
{
    public override IQueryable<object> Summarize()
    {
        var totals = Transactions.GroupBy(t => new { t.Type, t.OwnerId })
            .Select(group => new
            {
                group.Key.Type,
                group.Key.OwnerId,
                Amount = group.Sum(t => t.Amount)
            });

        var result = new List<object>();
        
        foreach (var owner in Budget.Owners)
        {
            var ownerTotals = totals.Where(t => t.OwnerId == owner.Id);
            var income = ownerTotals.FirstOrDefault(t => t.Type == TransactionType.Income)?.Amount ?? 0m;
            var expense = ownerTotals.FirstOrDefault(t => t.Type == TransactionType.Expense)?.Amount ?? 0m;
            var recurring = ownerTotals.FirstOrDefault(t => t.Type == TransactionType.Recurring)?.Amount ?? 0m;
            var balance = income - expense - recurring;
            
            result.Add(new
            {
                owner,
                income,
                expense,
                recurring,
                balance,
            });
        }
        return result.AsQueryable();
    }
}
