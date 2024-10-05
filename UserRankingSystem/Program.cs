using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Main program to run the UserRankingSystem.
/// </summary>
public class Program {
    public static void Main(string[] args) {
        // Build and run the Host.
        CreateHostBuilder(args).Build().Run();
    }

    // Create a Host Builder with specified IP address
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://*:5043", "http://localhost:5043");
            });
}