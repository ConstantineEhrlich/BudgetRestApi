using System.Text;
using AuthService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

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
        services.AddCors(StartupExtensions.CorsPolicies);
        services.AddControllers(opts => opts.Filters.Add<Controllers.ExceptionFilter>());
        services.ConfigureJwtAuthentication(new(Encoding.ASCII.GetBytes(Configuration["JWT_KEY"])));
        services.AddScoped<Services.JwtGenerator>();
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        services.Configure<Services.UsersDatabaseSettings>(Configuration.GetSection("UsersDatabase"));
        services.AddSingleton<Services.UsersDatabase>();
        services.AddScoped<Services.UserService>();
        services.AddScoped<Services.ProfileService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors("BasicCors");
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}

public static class StartupExtensions
{
    public static void CorsPolicies(CorsOptions options)
    {
        options.AddPolicy("BasicCors", policy =>
        {
            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(origin => true);
        });
    }
    
    public static void ConfigureJwtAuthentication(this IServiceCollection services, SymmetricSecurityKey key)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            ConfigureJwtBearerOptions(options, key);
        });
    }

    private static void ConfigureJwtBearerOptions(JwtBearerOptions options, SymmetricSecurityKey key)
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false
        };
        // Custom event handling for the JwtBearer middleware
        options.Events = new JwtBearerEvents
        {
            // This event is invoked when the middleware receives a message (in this case, an HTTP request)
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("access_token", out string? token))
                {
                    // If the token is found in the cookie, use it for authentication
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };
    }
}