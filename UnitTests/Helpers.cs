using BudgetModel.Models;

namespace UnitTests;

public static class Helpers
{
    public static string GetDbPath(bool makeNew = false)
    {
        string documentsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string codeDir = @"Code/dotnet/Files/Budget/db";

        if (!Path.Exists(Path.Combine(documentsDir, codeDir)))
        {
            Directory.CreateDirectory(Path.Combine(documentsDir, codeDir));
        }
        
        string dbFile = @"database.db";
        string dbPath = Path.Combine(documentsDir, codeDir, dbFile);
        if (File.Exists(dbPath) && makeNew)
        {
            File.Delete(dbPath);
        }

        return dbPath;
    }

    public static BudgetModel.Context GetContext(bool makeNew = false)
    {
        BudgetModel.Context context = new(GetDbPath(makeNew));
        context.Database.EnsureCreated();
        return context;
    }
}