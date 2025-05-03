using System.Data;
using System.Reflection;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace BudgetServices.Excel;

public class DataExportService
{
    private readonly ILogger<DataExportService> _logger;
    private readonly TransactionService _transactions;

    public DataExportService(ILogger<DataExportService> logger, TransactionService transactionService)
    {
        _logger = logger;
        _transactions = transactionService;
    }

    public async Task<ExportResult> ExportTransactions(string budgetId, string? requestingUserId)
    {
        var fileName = $"report_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{Guid.NewGuid().ToString("N")[..6]}.xlsx";
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                       ?? throw new BudgetServiceException("Base path not found");
        var exportPath = Path.Combine(basePath, "data", "export", budgetId);
        if (!Directory.Exists(exportPath))
        {
            Directory.CreateDirectory(exportPath);
        }

        var filePath = Path.Combine(exportPath, fileName);

        var transactions = await _transactions.GetAllTransactions(budgetId, requestingUserId);
        var table = new DataTable();

        table.Columns.Add("Id", typeof(string));
        table.Columns.Add("Owner", typeof(string));
        table.Columns.Add("Date", typeof(DateTime));
        table.Columns.Add("Category", typeof(string));
        table.Columns.Add("Amount", typeof(decimal));
        table.Columns.Add("Description", typeof(string));
        table.Columns.Add("TransactionType", typeof(string));
        table.Columns.Add("Year", typeof(int));
        table.Columns.Add("Period", typeof(int));

        foreach (var t in transactions.ToList())
        {
            var row = table.NewRow();
            row["Id"] = t.Id;
            row["Owner"] = t.Owner?.Id ?? string.Empty;
            row["Date"] = t.Date;
            row["Category"] = t.Category?.Id ?? string.Empty;
            row["Amount"] = t.Amount;
            row["Description"] = t.Description ?? string.Empty;
            row["TransactionType"] = t.Type.ToString();
            row["Year"] = t.Year;
            row["Period"] = t.Period;
            table.Rows.Add(row);
        }

        var writer = new ExcelWriter(filePath);
        writer.WriteData("Transactions", table.CreateDataReader());
        writer.Dispose();

        return new ExportResult(fileName);
    }
    
    
    
}