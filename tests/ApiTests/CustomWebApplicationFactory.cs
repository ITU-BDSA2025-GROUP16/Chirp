using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Repositories;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using MyChat.Razor.Tests;
using Microsoft.Data.Sqlite;

namespace Chirp.Web;
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    //In memory instance of our website.
    private SqliteConnection? _connection;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ChatDBContext>));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);

            var dbContextServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ChatDBContext));
            if (dbContextServiceDescriptor != null)
                services.Remove(dbContextServiceDescriptor);

            var facadeDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(CheepRepository));
            if (facadeDescriptor != null)
                services.Remove(facadeDescriptor);

            // Create and keep the connection alive for the lifetime of the factory
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            services.AddDbContext<ChatDBContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // uses TestAuthHandler to check if users are logged in during tests
            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme, options => { });

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ChatDBContext>();

                db.Database.EnsureCreated();

            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}