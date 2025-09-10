namespace Chirp.CLI.Client;

using System;

public static class UserInterface
{
    public static void ShowHelp()
    {
        Console.WriteLine("'dotnet run read' to read csv");
        Console.WriteLine("'dotnet run cheep \"text\"' to add new line to csv");
    }

    public static void ShowInvalidArgument()
    {
        Console.WriteLine("invalid argument");
    }

    public static void PrintCheep(Cheep cheep)
    {
        var dto = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).ToLocalTime();
        Console.WriteLine($"{cheep.Author} @ {dto:dd/MM/yy HH:mm:ss}: {cheep.Message}");
    }

    public static void ShowCheepAdded(Cheep cheep)
    {
        Console.WriteLine($"{cheep.Author}, \"{cheep.Message}\", {cheep.Timestamp} added to csv");
    }
}