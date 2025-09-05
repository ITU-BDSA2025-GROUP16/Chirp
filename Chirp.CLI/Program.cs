using System.Globalization;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;
using CsvHelper;


class Cheep
{
    public string? Author { get; set; }
    public string? Message { get; set; }
    public long Timestamp { get; set; }
}
class Program
{
    static void Main(string[] args)
    {

        //Parse input for at beslutte hvilken metode køres
        
        if (args.Length == 0)
        {
            Console.WriteLine("'dotnet run read' to read csv");
            Console.WriteLine("'dotnet run cheep \"text\"' to add new line to csv");

            return;
        }

        if (args[0].ToLower() == "read")
        {
            ReadCsv();
        }
        else if (args[0].ToLower() == "cheep" && args.Length > 1)
        {
            args = args[1..];
            AppendCheep(args);
        }
        else
        {
            Console.WriteLine("invalid argument");
        }


        
    }

    static void ReadCsv() //printer alt i csv som følger format
    {
        Console.WriteLine("ReacCsv reached");
        using var reader = new StreamReader("data/chirp_cli_db.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<Cheep>();
        foreach (var record in records)
        {
            var dto = DateTimeOffset.FromUnixTimeSeconds(record.Timestamp).ToLocalTime();
            Console.WriteLine($"{record.Author} @ {dto:dd/MM/yy HH:mm:ss}: {record.Message}");
        }
    }
    
    static void AppendCheep(string[] message) //Indsætter ny linje i csv
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



