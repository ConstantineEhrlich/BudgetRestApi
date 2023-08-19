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
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
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
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        
        app.UseEndpoints(ep => ep.MapControllers());
    }
}