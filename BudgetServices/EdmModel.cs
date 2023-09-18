using BudgetModel.Models;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace BudgetServices;

public static class EdmModel
{
    public static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<User>("Users");
        builder.EntitySet<BudgetFile>("Budgets");
        builder.EntitySet<Category>("Categories");
        builder.EntitySet<Transaction>("Transactions");
        return builder.GetEdmModel();
    }
}