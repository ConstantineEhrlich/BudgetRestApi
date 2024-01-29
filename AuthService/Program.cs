namespace AuthService;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        builder.Configuration["JWT_KEY"] = System.Environment.GetEnvironmentVariable("JWT_KEY")
            ?? throw new KeyNotFoundException("JWT_KEY variable not set");
        
        Startup startup = new(builder.Configuration);
        startup.ConfigureServices(builder.Services);

        WebApplication app = builder.Build();
        startup.Configure(app, app.Environment);
        
        app.Run();
    }
}
