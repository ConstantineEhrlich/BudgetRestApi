using System.Text;
using System.Text.Json.Serialization;
using BudgetModel.Models;
using BudgetServices;
using BudgetWebApi.Sockets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using BudgetWebApi.Sockets;

namespace BudgetWebApi;

public class Startup
{
    public IConfiguration Configuration { get; init; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        Configuration["JWT_KEY"] = System.Environment.GetEnvironmentVariable("JWT_KEY") ?? "ThisIsAVerySecretKeyThatImUsingHere";
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Cors allow requesting the API from the domain that is different than API domain
        services.AddCors(options =>
        {
            options.AddPolicy("DynamicCorsPolicy", builder =>
            {
                builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed(origin => true); // allow any origin
            });
        });
        

        
        services.AddControllers();
        
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
        
        
        // Read JWT key from the config
        string jwtKey = Configuration["JWT_KEY"];
        byte[] byteKey = Encoding.ASCII.GetBytes(jwtKey);
        
        // Authentication service
        services.AddAuthentication(options =>
        {
            // Set the default scheme for authentication to JwtBearer
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
            {
                // Require authentication through https
                options.RequireHttpsMetadata = true;
                // Save the token for later use in the request pipeline
                options.SaveToken = true;
                // Set up the token validation parameters
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    // Resolve the jwtKey that is stored in the config
                    IssuerSigningKey = new SymmetricSecurityKey(byteKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                // Custom event handling for the JwtBearer middleware
                options.Events = new JwtBearerEvents
                {
                    // This event is invoked when the middleware receives a message (in this case, an HTTP request)
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue("access_token", out string token))
                        {
                            // If the token is found in the cookie, use it for authentication
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    }
                };
                
                
                
            });
        
        
        // Password hasher
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

        // Updates socket manager
        services.AddSingleton<BudgetUpdateManager>();
        // Socket cleanup service - runs in background
        services.AddHostedService<CleanupService<BudgetUpdateManager>>();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("DynamicCorsPolicy");
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        
        
        // Add websocket for transaction updates
        app.UseWebSockets(new WebSocketOptions()
        {
            // These are defaults that might be changed
            KeepAliveInterval = TimeSpan.FromMinutes(2),
            // AllowedOrigins = {  }
        });
            
            
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(ep =>
        {
            ep.MapControllers();
        });
    }
}