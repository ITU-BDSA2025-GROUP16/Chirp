using System.Net.Http.Json;
using System.Runtime.CompilerServices;


var client = new HttpClient { BaseAddress = new Uri("http://localhost:5080") };

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
            await Cheep();
            break;
        case "Cheeps":
            await AddUser();
            break;
        default:
            Console.WriteLine("Invalid Choise... Try again!");
            break;
    }
}

async Task Cheep()
{
    try
    {
        var cheep = new Cheep
            {
                Author = Environment.UserName,
                Message = await.,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            //_database.Store(cheep);
            //Client.UserInterface.ShowCheepAdded(cheep);
    }
    catch (Exception e)
    {
        Console.WriteLine("ERROR: " + e.Message);
    }
}

async Task AddUser()
{
    Console.WriteLine("Enter user ID: ");
    var inputUserID = Console.ReadLine();
    Console.WriteLine("Enter user name");
    var inputUserName = Console.ReadLine();

    if (!int.TryParse(inputUserID, out var id) || string.IsNullOrWhiteSpace(inputUserName))
    {
        Console.WriteLine("Invalid entry");
        return;
    }

    var newUser = new User { Id = id, Name = inputUserName };

    try
    {
        var response = await client.PostAsJsonAsync("/users", newUser);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("User added to database successfully");
        }
        else
        {
            Console.WriteLine("Failed: " + response.StatusCode);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine("ERROR: " + e.Message);
    }

}
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}
localhost



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