using BudgetModel;
namespace BudgetDataLoader;

public static class Program
{
    public static void Main(string[] args)
    {
        Config config;
        try
        {
            config = new Config();
        }
        catch (Exception e)
        {
            Console.WriteLine("The following error occured:");
            Console.WriteLine(e.Message);
            return;
        }
        
        new SampleDataCreator(config).Create();
    }
}