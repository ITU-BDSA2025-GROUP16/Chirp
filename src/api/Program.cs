using Microsoft.EntityFrameworkCore;
using MyChat.Razor.data;

var builder = WebApplication.CreateBuilder(args);
// Determine SQLite DB path
string dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") 
                ?? Path.Combine(Path.GetTempPath(), "..", "..", "data", "Chat.db");
// Register DBFacade
builder.Services.AddSingleton(new DBFacade(dbPath));
// Register CheepService
builder.Services.AddScoped<ICheepService, CheepService>();

// Load database connection via configuration
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChatDBContext>(options => options.UseSqlite(connectionString));

// Add services to the container.
builder.Services.AddRazorPages();


var app = builder.Build();

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

