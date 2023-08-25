using System.Text.Json.Serialization;
using BudgetServices;
using Microsoft.EntityFrameworkCore;

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
        // Cors allow requesting the API from the domain that does not equal to API (browser safety measure)
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });
        });
        
        // Controller automatically converts objects into json, here we apply the options:
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
        
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddScoped<UserService>();
        services.AddScoped<BudgetFileService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<TransactionService>();
        services.AddDbContext<BudgetModel.Context>(options =>
            options.UseSqlite(Configuration.GetConnectionString("Default")));

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
        app.UseAuthorization();
        
        app.UseEndpoints(ep => ep.MapControllers());
    }
}