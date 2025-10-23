using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using MyChat.Razor.data;

namespace MyChat.Tests;
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    //In memory instance of our website.
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
                d => d.ServiceType == typeof(DBFacade));
            if (facadeDescriptor != null)
                services.Remove(facadeDescriptor);

            services.AddDbContext<ChatDBContext>(options =>
            {
                options.UseSqlite("Data Source=:memory:");
            });

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ChatDBContext>();
                
                db.Database.OpenConnection(); 
                db.Database.EnsureCreated();
                
            }
        });
    }
}