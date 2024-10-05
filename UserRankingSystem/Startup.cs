using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using UserRankingSystem.Data;

/// <summary>
/// Class Startup deals with the configuration of services and middleware for the application.
/// </summary>
public class Startup {
    // Configures services like controllers and database context for the application.
    public void ConfigureServices(IServiceCollection services) {
        services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });
        Console.WriteLine("Controllers configured.");

        services.AddDbContext<UserRankingContext>(options => {
            options.UseSqlite("Data Source=userrankings.db");
            Console.WriteLine("Configuring database");
        });

        Console.WriteLine("DbContext Initialised!");

        services.AddSwaggerGen();
    }

    // Configures request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserRanking API V1");
            c.RoutePrefix = string.Empty;
        });

        app.UseRouting();
        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });
    }
}