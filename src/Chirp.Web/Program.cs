using Chirp.Core.Interfaces;
using Chirp.Core.Services;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;

var builder = WebApplication.CreateBuilder(args);

// Determine SQLite DB path
string dbPath = Path.Combine(AppContext.BaseDirectory, "chirp.db");
builder.Services.AddDbContext<ChatDBContext>(
    options => options.UseSqlite($"Data Source={dbPath}"));

// Register services
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
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
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GitHubAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/";
    options.LogoutPath = "/logout";
})
.AddGitHub(options =>
{
    options.ClientId = builder.Configuration["authentication:github:clientId"];
    options.ClientSecret = builder.Configuration["authentication:github:clientSecret"];
    options.CallbackPath = "/signin-github";
    
    //EVEN MORE DEBUG!!
    options.Events = new OAuthEvents
    {
        OnRedirectToAuthorizationEndpoint = context =>
        {
            Console.WriteLine($"Redirecting to GitHub: {context.RedirectUri}");
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }
    };
});

//MORE DEBUG

builder.Services.Configure<GitHubAuthenticationOptions>("GitHub", options =>
{
    options.Events.OnCreatingTicket = context =>
    {
        Console.WriteLine("=== OnCreatingTicket SUCCESS ===");
        Console.WriteLine($"Access Token: {context.AccessToken?.Substring(0, 10)}...");
        return Task.CompletedTask;
    };
    
    options.Events.OnRemoteFailure = context =>
    {
        Console.WriteLine("=== OnRemoteFailure ===");
        Console.WriteLine($"Error: {context.Failure?.Message}");
        Console.WriteLine($"Stack: {context.Failure?.StackTrace}");
        context.Response.Redirect("/");
        context.HandleResponse();
        return Task.CompletedTask;
    };
});


var app = builder.Build();

// Database initialization
using (var scope = app.Services.CreateScope())
{
    var chirpContext = scope.ServiceProvider.GetRequiredService<ChatDBContext>();
    if (chirpContext.Database.EnsureCreated())
    {
        DbInitializer.SeedDatabase(chirpContext);
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

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapRazorPages();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Path: {context.Request.Path}");
    Console.WriteLine($"Request Method: {context.Request.Method}");
    await next();
});

app.Run();

public partial class Program { }