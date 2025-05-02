namespace BudgetServices.Excel;

public class ImportResult
{
    public ImportResult(string row, string result)
    {
        Row = row;
        Result = result;
    }
    public string Row { get; set; }
    public string Result { get; set; }
}