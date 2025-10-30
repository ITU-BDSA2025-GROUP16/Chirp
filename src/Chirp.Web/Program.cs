using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Repositories;
using Chirp.Core.Interfaces;
using Chirp.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Determine SQLite DB path
string dbPath = Path.Combine(AppContext.BaseDirectory, "chirp.db");
builder.Services.AddDbContext<ChatDBContext>(
    options => options.UseSqlite($"Data Source={dbPath}"));

// Register CheepService
builder.Services.AddScoped<ICheepService, CheepService>();

builder.Services.AddScoped<ICheepRepository, CheepRepository>();


// Add services to the container.
builder.Services.AddRazorPages();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var chirpContext = scope.ServiceProvider.GetRequiredService<ChatDBContext>();
    if (chirpContext.Database.EnsureCreated())
    {
        DbInitializer.SeedDatabase(chirpContext);    
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

app.MapRazorPages();

app.Run();


public partial class Program { }