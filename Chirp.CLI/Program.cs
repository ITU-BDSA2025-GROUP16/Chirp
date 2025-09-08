using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;
using CommandLine;
using CsvHelper;
using System.Globalization;



class Cheep //CsvHelper 
{
    public string? Author { get; set; }
    public string? Message { get; set; }
    public long Timestamp { get; set; }
}

//system.commandline
[Verb("read", HelpText = "Read all cheeps from csv")]
class ReadOptions { }
[Verb("cheep", HelpText = "Add new cheep")]
class CheepOptions
{
    [Value(0, Min = 1, HelpText = "Message text", Required = true)]
    public IEnumerable<string> Message { get; set; } = Enumerable.Empty<string>();
}


class Program
{ 
    static int Main(string[] args)
    {
        var result = Parser.Default.ParseArguments<ReadOptions, CheepOptions>(args);
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
            errs =>
            {
                exitCode = 1;
                return exitCode;
            });

        return exitCode;
    }


    static void ReadCsv() //printer alt i csv som følger format
    {
        using var reader = new StreamReader("data/chirp_cli_db.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<Cheep>();
        foreach (var record in records)
        {
            var dto = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp).ToLocalTime();
            Console.WriteLine($"{record.Author} @ {dto:dd/MM/yy HH:mm:ss}: {record.Message}");
        }
    }
    
    static void AppendCheep(IEnumerable<string> message) //Indsætter ny linje i csv (skal følge format)
    {
        var filePath = "data/chirp_cli_db.csv";
        var username = Environment.UserName;
        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var fullMessage = string.Join(" ", message);

        var record = new Cheep { Author = username, Message = fullMessage, Timestamp = unixTime };

        bool fileExists = File.Exists(filePath);

        using var stream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
        using var writer = new StreamWriter(stream);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        if (!fileExists || new FileInfo(filePath).Length == 0)
        {
            csv.WriteHeader<Cheep>();
            csv.NextRecord();
        }

        csv.WriteRecord(record);
        csv.NextRecord();

        Console.WriteLine($"{username}, \"{fullMessage}\", {unixTime} added to csv");
    }   
}



