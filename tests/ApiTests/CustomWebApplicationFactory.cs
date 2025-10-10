using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;  
using Microsoft.Extensions.DependencyInjection;

namespace MyChat.Tests;

// Custom factory to configure test database
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DBFacade registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DBFacade));
            if (descriptor != null)
                services.Remove(descriptor);

            // Point to your database with test data (relative path from test project)
            string testDbPath = Path.Combine("..", "..", "src", "api", "data", "chirp.db");
            services.AddSingleton(new DBFacade(testDbPath));
        });
    }
}