using System.Collections.Concurrent;
using BudgetModel.Enums;
using BudgetModel.Models;
using Microsoft.Extensions.Logging;

namespace BudgetServices.Excel;

public class DataImportService
{
    private readonly ILogger<DataImportService> _logger;
    private readonly TransactionService _transactions;
    private readonly BudgetFileService _budgets;

    public DataImportService(ILogger<DataImportService> logger, TransactionService transactionService, BudgetFileService budgetFile)
    {
        _logger = logger;
        _transactions = transactionService;
        _budgets = budgetFile;
    }


    public async Task<List<ImportResult>> ImportData(ImportConfiguration configuration, string requestingUserId, CancellationToken cancellationToken)
    {
        if (configuration.BudgetId is null)
            throw new BudgetServiceException("Budget must be provided");
        await _budgets.ThrowIfNotOwner(requestingUserId, configuration.BudgetId);
        
        var budget = await _budgets.GetBudgetFile(configuration.BudgetId, requestingUserId);
        
        if (configuration.PathToFile is null || configuration.WorksheetName is null)
        {
            throw new BudgetServiceException("PathToFile or WorksheetName must be provided");
        }

        using var document = new ExcelIterator(configuration.PathToFile);
        document.WorksheetName = configuration.WorksheetName;
        document.MinRow = 2;
        document.MinCol = 1;
        document.MaxCol = 9;

        var ids = (await _transactions.GetAllTransactions(configuration.BudgetId, requestingUserId))
            .Select(t => t.Id).ToArray();
        
        var results = new ConcurrentBag<ImportResult>();

        foreach (var row in document)
        {
            try
            {
                await HandleRow(row, budget, ids, requestingUserId);
                results.Add(new ImportResult(document.RowIndex.ToString(), "Success"));
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to import data");
                var msg = $"{e.GetType().Name} - {e.Message}";
                _logger.LogError(msg);
                results.Add(new ImportResult(document.RowIndex.ToString(), $"Error: {e.Message}"));
            }
        }
        
        return results.ToList();
    }

    private Task HandleRow(object?[] row, BudgetFile budget, string[] ids, string requestingUser)
    {
        var id = string.IsNullOrEmpty(row[0] as string) ? null : row[0] as string;
        var authorId = row[1] as string ?? throw new BudgetServiceException("Author must be provided");
        var author = budget.Owners.FirstOrDefault(x => x.Id == authorId) ?? throw new BudgetServiceException($"Author {authorId} not found - failed at ID {id}");
        var dateVal = row[2] as decimal? ?? throw new BudgetServiceException($"Date cannot be null - failed at ID {id}");
        var oaDate = DateTime.FromOADate(Convert.ToDouble(dateVal));
        var date = DateTime.SpecifyKind(oaDate, DateTimeKind.Utc);
        var categoryId = row[3] as string ?? throw new BudgetServiceException($"Category cannot be null - failed at ID {id}");
        var category = budget.Categories.FirstOrDefault(c => c.Id == categoryId) ?? throw new BudgetServiceException($"Category {categoryId} not found - failed at ID {id}");
        var amount = row[4] as decimal? ?? throw new BudgetServiceException($"Amount cannot be null - failed at ID {id}");
        var description = row[5] as string ?? throw new BudgetServiceException($"Description cannot be null - failed at ID {id}");
        var transactionTypeName = row[6] as string ?? throw new BudgetServiceException($"TransactionType cannot be null - failed at ID {id}");
        var transactionType = transactionTypeName switch
        {
            "Income" => TransactionType.Income,
            "Expense" => TransactionType.Expense,
            "Recurring" => TransactionType.Recurring,
            "Budget" => TransactionType.Budget,
            _ => throw new BudgetServiceException($"TransactionType is incorrect - failed at ID {id}")
        };
        var year = Convert.ToInt32(row[7] as decimal? ?? throw new BudgetServiceException($"Year cannot be null - failed at ID {id}"));
        var period =  Convert.ToInt32(row[8] as decimal? ?? throw new BudgetServiceException($"Period cannot be null - failed at ID {id}"));

        if (id is null || !ids.Contains(id))
        {
            _logger.LogInformation("Adding new transaction with id {NoId}", id ?? "<new>");
            return _transactions.AddTransaction(budget.Id, requestingUser, categoryId, amount, description,
                transactionType, authorId, date, year, period, id);
        }
        
        var newTr = new Transaction(budget: budget,
            owner: author,
            author: author,
            date: date,
            type: transactionType,
            cat: category,
            description: description,
            amount: amount,
            id: id,
            year: year,
            period: period);
        
        _logger.LogInformation("Updating transaction with id {id}", id);
        return _transactions.UpdateTransaction(id, requestingUser, newTr);
    }
}