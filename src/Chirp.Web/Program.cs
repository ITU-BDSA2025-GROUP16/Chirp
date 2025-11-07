using Chirp.Core.Interfaces;
using Chirp.Core.Services;
using Chirp.Core.Domain;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ChatDBContextConnection") ?? throw new InvalidOperationException("Connection string 'ChatDBContextConnection' not found.");;

// Determine SQLite DB path
string dbPath = Path.Combine(AppContext.BaseDirectory, "chirp.db");
builder.Services.AddDbContext<ChatDBContext>(
    options => options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddDefaultIdentity<Author>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ChatDBContext>();

// Register CheepService
builder.Services.AddScoped<ICheepService, CheepService>();

builder.Services.AddScoped<ICheepRepository, CheepRepository>();


// Add services to the container.
builder.Services.AddRazorPages();


var app = builder.Build();

// Ensure DB is created and seeded
using (var scope = app.Services.CreateScope())
{
    var chirpContext = scope.ServiceProvider.GetRequiredService<ChatDBContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Author>>();

    // Delete old DB in development if corrupted
    if (!File.Exists(dbPath) || chirpContext.Database.CanConnect() == false)
    {
        if (File.Exists(dbPath)) File.Delete(dbPath);

        chirpContext.Database.EnsureCreated();
        DbInitializer.SeedDatabase(chirpContext, userManager);
    }
    else
    {
        // Optional: just seed if empty
        DbInitializer.SeedDatabase(chirpContext, userManager);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();    // for Identity
app.UseAuthorization();     // for [Authorize] attributes
app.MapRazorPages();
app.Run();


public partial class Program { }
