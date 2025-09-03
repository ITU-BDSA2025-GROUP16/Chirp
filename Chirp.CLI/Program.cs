using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;

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
        try
        {
            using var parser = new TextFieldParser("data/chirp_cli_db.csv");
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.ReadLine();

            while (!parser.EndOfData)
            {
                var text = parser.ReadFields();
                
                if (text.Length != 3)
                {
                    continue;
                }
                
                string digitsOnly = Regex.Replace(text[2], @"\D", "");
                long time = long.Parse(digitsOnly);

                DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(time).ToLocalTime();
                Console.WriteLine($"{text[0]} @ {dto:dd/MM/yy HH:mm:ss}: {text[1]}");
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
        }
    }
    
    static void AppendCheep(string[] message) //Indsætter ny linje i csv
    {
        string filePath = "data/chirp_cli_db.csv";
        using StreamWriter writer = File.AppendText(filePath);
        
        String username = Environment.UserName;
        long unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        DateTimeOffset dto = DateTimeOffset.FromUnixTimeSeconds(unixTime).ToLocalTime();

        
        string fullMessage = string.Join(" ", message);
        string fullMessageFinal = $"{username},\"{fullMessage}\",{unixTime}";

        
        writer.WriteLine(fullMessageFinal); 
        Console.WriteLine(fullMessageFinal + " added to csv");
        
        
    }
}



