

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var dataPath = Path.Combine(AppContext.BaseDirectory, "data", "chirp_cli_db.csv");

if (!File.Exists(dataPath))
{
    Directory.CreateDirectory(Path.GetDirectoryName(dataPath)!);
    File.WriteAllText(dataPath, "Author,Message,Timestamp" + Environment.NewLine);
}


app.MapGet("/cheeps", () =>
{
    var lines = File.ReadAllLines(dataPath);

    return lines
        .Skip(1)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => line.Split(","))
        .Select(parts => new Cheep
        {
            Author = parts[0].Trim(),
            Message = parts[1].Trim(),
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        })
        .ToList();
});

app.MapPost("/cheeps", (Cheep newCheep) =>
{
    File.AppendAllText(dataPath, $"{newCheep.Author}, {newCheep.Message}, {newCheep.Timestamp}");
    return Results.Created($"/users/{newCheep.Timestamp}", newCheep);
});
app.MapGet("/", () => "Chirp API is running!. Cheeps kommer senere, vi magtede ikke lige mere idag");

app.Run();

public class Cheep
{
    public required string Author { get; set; }
    public required string Message { get; set; }
    public long Timestamp { get; set; }
}
