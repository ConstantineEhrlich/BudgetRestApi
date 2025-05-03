namespace BudgetServices.Excel;

public class ExportSettings
{
    public ICollection<ExportTask>? Exports { get; set; }
}

public class ExportTask
{
    public string? User { get; set; }
    public string? Budget { get; set; }
}