using System.Text;
using System.Text.Json.Serialization;
using BudgetModel.Models;
using BudgetServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BudgetWebApi;

public class Startup
{
    public IConfiguration Configuration { get; init; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Cors allow requesting the API from the domain that is different than API domain
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });
        });
        
        
        
        // Json conversion options to be used by the controllers
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.MaxDepth = 2;
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
        
        
        
        
        // Services to support Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        
        
        
        
        // Data access services
        services.AddScoped<UserService>();
        services.AddScoped<BudgetFileService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<TransactionService>();
        services.AddDbContext<BudgetModel.Context>(options =>
            options.UseSqlite(Configuration.GetConnectionString("Default")));
        
        
        // Read JWT key from the environment variables:
        string key = System.Environment.GetEnvironmentVariable("JWT_KEY") ?? "ThisIsAVerySecretKeyThatImUsingHere";
        byte[] byteKey = Encoding.ASCII.GetBytes(key);
        
        // Authentication service
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(byteKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        
        // Password hasher
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();



    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("AllowAll");
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(ep => ep.MapControllers());
    }
}