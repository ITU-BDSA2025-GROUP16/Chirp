using Chirp.Core.Interfaces;
using Chirp.Core.Services;
using Chirp.Core.Domain;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;




var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ChatDBContextConnection") ?? throw new InvalidOperationException("Connection string 'ChatDBContextConnection' not found.");;

// Determine SQLite DB path
string dbPath = Path.Combine(AppContext.BaseDirectory, "chirp.db");
builder.Services.AddDbContext<ChatDBContext>(
    options => options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddDefaultIdentity<Author>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ChatDBContext>();

// Register services
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IFollowService, FollowService>();    
builder.Services.AddScoped<IFollowRepository, FollowRepository>(); 

builder.Services.AddRazorPages();

// Session configuration
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// Data Protection
builder.Services.AddDataProtection();

//DEBUG

Console.WriteLine("=== Configuration Debug ===");
Console.WriteLine($"authentication:github:clientId = {builder.Configuration["authentication:github:clientId"] ?? "NULL"}");
Console.WriteLine($"authentication:github:clientSecret = {builder.Configuration["authentication:github:clientSecret"] ?? "NULL"}");
Console.WriteLine("All config keys:");
foreach (var kvp in builder.Configuration.AsEnumerable())
{
    if (kvp.Key.Contains("GitHub", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine($"  {kvp.Key} = {kvp.Value}");
    }
}
Console.WriteLine("===========================");

// Authentication
builder.Services.AddAuthentication()
    .AddGitHub(options =>
    {
        var clientId = builder.Configuration["authentication:github:clientId"];
        var clientSecret = builder.Configuration["authentication:github:clientSecret"];
        
        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new InvalidOperationException(
                $"GitHub OAuth not configured. ClientId: {(clientId == null ? "null" : "set")}, " +
                $"ClientSecret: {(clientSecret == null ? "null" : "set")}");
        }
        
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;
        options.CallbackPath = "/signin-github";
        options.SaveTokens = true;
        
        // Add SignInScheme to use Identity's external scheme
        options.SignInScheme = IdentityConstants.ExternalScheme;
        
        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var gitHubId = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = context.Principal.FindFirst(ClaimTypes.Name)?.Value;
                var email = context.Principal.FindFirst(ClaimTypes.Email)?.Value;

                userName = string.IsNullOrWhiteSpace(userName) ? $"GitHubUser_{gitHubId}" : userName.Trim();
                email = string.IsNullOrWhiteSpace(email) ? $"{userName}@github.user" : email.Trim();

                Console.WriteLine($"=== GitHub OAuth - User: {userName} (ID: {gitHubId}) ===");
            },
            
            OnRemoteFailure = context =>
            {
                Console.WriteLine("=== OnRemoteFailure ===");
                Console.WriteLine($"Error: {context.Failure?.Message}");
                context.Response.Redirect("/");
                context.HandleResponse();
                return Task.CompletedTask;
            }
        };
    });


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

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();    // for Identity
app.UseAuthorization();     // for [Authorize] attributes
app.MapRazorPages();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Path: {context.Request.Path}");
    Console.WriteLine($"Request Method: {context.Request.Method}");
    await next();
});

app.Run();

public partial class Program { }