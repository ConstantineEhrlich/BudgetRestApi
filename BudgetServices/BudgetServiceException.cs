namespace BudgetServices;

public class BudgetServiceException: Exception
{
    public BudgetServiceException(string message) : base(message){}
}