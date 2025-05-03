namespace BudgetServices.Excel;

public class ExportResult
{
    public ExportResult(string name)
    {
        FileName = name;
    }
    public string FileName { get; set; }
}