

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Detect if running in Azure
var isAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID"));

// Use /home for Azure, current directory for localhost
var dataPath = isAzure 
    ? Path.Combine("/home", "data", "chirp_cli_db.csv")
    : Path.Combine(Directory.GetCurrentDirectory(), "data", "chirp_cli_db.csv");

if (!File.Exists(dataPath))
{
    Directory.CreateDirectory(Path.GetDirectoryName(dataPath)!);
    File.WriteAllText(dataPath, "Author,Message,Timestamp" + Environment.NewLine);
}

app.MapPost("/cheeps", (Cheep newCheep) =>
{
    // No spaces after commas, and add newline at the end
    File.AppendAllText(dataPath, $"{newCheep.Author},{newCheep.Message},{newCheep.Timestamp}{Environment.NewLine}");
    return Results.Created($"/users/{newCheep.Timestamp}", newCheep);
});

app.MapGet("/cheeps", () =>
{
    var lines = File.ReadAllLines(dataPath);
    return lines
        .Skip(1)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => 
        {
            var lastCommaIndex = line.LastIndexOf(',');
            var secondLastCommaIndex = line.LastIndexOf(',', lastCommaIndex - 1);
            
            return new Cheep
            {
                Author = line.Substring(0, secondLastCommaIndex).Trim(),
                Message = line.Substring(secondLastCommaIndex + 1, lastCommaIndex - secondLastCommaIndex - 1).Trim(),
                Timestamp = long.Parse(line.Substring(lastCommaIndex + 1).Trim())
            };
        })
        .ToList();
});
app.MapGet("/", () => "Chirp API is running!. Cheeps kommer senere, vi magtede ikke lige mere idag");

app.Run();

public class Cheep
{
    public required string Author { get; set; }
    public required string Message { get; set; }
    public long Timestamp { get; set; }
}
