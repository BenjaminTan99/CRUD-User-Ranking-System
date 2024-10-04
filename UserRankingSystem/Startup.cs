/// <summary>
/// Class Startup deals with the configuration of services and middleware for the application.
/// </summary>
public class Startup {
    // Configures services like controllers and database context for the application.
    public void ConfigureServices(IServiceCollection services) {
        services.AddDbContext<UserRankingContext>(options =>
            options.UseSqlite("Data Source=userrankings.db"));
        
        services.AddControllers();
    }

    // Configures request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });
    }
}