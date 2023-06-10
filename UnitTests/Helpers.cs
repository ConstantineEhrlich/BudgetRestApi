using BudgetModel.Models;

namespace UnitTests;

public static class Helpers
{
    public static string GetDbPath(bool makeNew = false)
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string dbName = "database.db";
        string dbPath = Path.Combine(baseDirectory, dbName);
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