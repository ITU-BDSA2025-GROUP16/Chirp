using System.Net.Http.Json;
using System.Runtime.CompilerServices;


var client = new HttpClient { BaseAddress = new Uri("http://localhost:5086") };

while (true)
{
    Console.WriteLine("\nSelect an option:");
    Console.WriteLine("Cheep: Write a cheep");
    Console.WriteLine("Cheeps: Read cheeps");
    Console.WriteLine("Exit: Exit");
    Console.Write("Choice: ");

    var input = Console.ReadLine();

    if (input == "Exit" || input == null)
    {
        break;
    }

    switch (input)
    {
        case "Cheep":
            Console.Write("Enter your message: ");
            await Cheep();
            break;
        case "Cheeps":
            Console.WriteLine("How many cheeps do you want to read? (or press Enter to read all)");
            await ReadCheeps();
            break;
        default:
            Console.WriteLine("Invalid Choise... Try again!");
            break;
    }
}

async Task Cheep()
{
    var CheepMessage = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(CheepMessage)) {
    
        try
        {

            var cheep = new Cheep
                {
                    Author = Environment.UserName,
                    Message = CheepMessage,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };
        
                var response = await client.PostAsJsonAsync("/cheeps", cheep);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to add cheep. Status code: " + response.StatusCode);
                    return;
                }
                Console.WriteLine("Cheep added successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: " + e.Message);
        }
    }
}

async Task ReadCheeps()
{
    var cheepCountInput = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(cheepCountInput) && int.TryParse(cheepCountInput, out int cheepCount))
    {
        try
        {
            var response = await client.GetAsync($"/cheeps?count={cheepCount}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to fetch cheeps. Status code: " + response.StatusCode);
                return;
            }

            var cheeps = await response.Content.ReadFromJsonAsync<List<Cheep>>();
            if (cheeps != null)
            {
                foreach (var cheep in cheeps)
                {
                    Console.WriteLine($"[{DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp)}] {cheep.Author}: {cheep.Message}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: " + e.Message);
        }
    }
    else
    {
        try
        {
            var response = await client.GetAsync("/cheeps");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to fetch cheeps. Status code: " + response.StatusCode);
                return;
            }

            var cheeps = await response.Content.ReadFromJsonAsync<List<Cheep>>();
            if (cheeps != null)
            {
                foreach (var cheep in cheeps)
                {
                    Console.WriteLine($"[{DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp)}] {cheep.Author}: {cheep.Message}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: " + e.Message);
        }
    } 
 }

public class Cheep
{
    public required string Author { get; set; }
    public required string Message { get; set; }
    public long Timestamp { get; set; }
}



/*    
    using SimpleDB;
    using CommandLine;

    namespace Chirp.CLI;

    [Verb("read", HelpText = "Read all cheeps from csv")]
    class ReadOptions { }

    [Verb("cheep", HelpText = "Add new cheep")]
    class CheepOptions
    {        [Value(0, Min = 1, HelpText = "Message text", Required = true)]
        public IEnumerable<string> Message { get; set; } = Enumerable.Empty<string>();
    }

    [Verb("delete-last", HelpText = "Delete the most recent cheep")]
    class DeleteLastOptions { }

    class Program
    {
        private static readonly IDatabaseRepository<Cheep> _database = CreateDatabase();

        private static IDatabaseRepository<Cheep> CreateDatabase() {
            return CSVDatabase<Cheep>.Instance;
        }

        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Client.UserInterface.ShowHelp();
                return 1;
            }

            var result = Parser.Default.ParseArguments<ReadOptions, CheepOptions, DeleteLastOptions>(args);
            int exitCode = 1;

            result.MapResult(
                (ReadOptions opts) =>
                {
                    ReadCsv();
                    exitCode = 0;
                    return exitCode;
                },
                (CheepOptions opts) =>
                {
                    AppendCheep(opts.Message.ToArray());
                    exitCode = 0;
                    return exitCode;
                },
                (DeleteLastOptions opts) =>
                {
                    DeleteLastCheep(); 
                    return 0; 
                    
                },
                errs =>
                {
                    Client.UserInterface.ShowInvalidArgument();
                    exitCode = 1;
                    return exitCode;
                });

            return exitCode;
        }

        private static void ReadCsv()
        {
            var cheeps = _database.Read();
            foreach (var cheep in cheeps)
            {
                Client.UserInterface.PrintCheep(cheep);
            }
        }

        private static void AppendCheep(string[] messageWords)
        {
            var cheep = new Cheep
            {
                Author = Environment.UserName,
                Message = string.Join(" ", messageWords),
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            _database.Store(cheep);
            Client.UserInterface.ShowCheepAdded(cheep);
        }
        
        private static void DeleteLastCheep()
        {
            _database.DeleteLast();
        }
    } */