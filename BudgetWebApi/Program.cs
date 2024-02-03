namespace BudgetWebApi;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        Startup startup = new(builder.Configuration);
        startup.ConfigureServices(builder.Services);

        WebApplication app = builder.Build();
        startup.Configure(app, app.Environment);
        
        app.Run();
        ;
    }
}
