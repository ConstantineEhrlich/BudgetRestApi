using AuthService.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace AuthService;

public class Startup
{
    private IConfiguration Configuration { get; init; }
    
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(CorsPolicies);
        services.AddControllers();
        services.Configure<Models.UsersDatabaseSettings>(Configuration.GetSection("UsersDatabase"));
        services.AddSingleton<Models.UsersDatabase>();
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        
    }

    private static void CorsPolicies(CorsOptions options)
    {
        options.AddPolicy("BasicCors", policy =>
        {
            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(origin => true);
        });
    }
}