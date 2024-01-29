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
        services.AddScoped<Services.UserService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors("BasicCors");
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
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