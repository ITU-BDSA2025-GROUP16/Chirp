using System.Diagnostics;
using Xunit;

public class E2ETest1
{
    private (int ExitCode, string Output) RunCommand(string command, string args, string workingDir)
    {
        var psi = new ProcessStartInfo
        {
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = workingDir
        };

        using var process = Process.Start(psi);
        string output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
        process.WaitForExit();
        return (process.ExitCode, output);
    }

    [Fact]
    public void ReadCommand_ShowsExpectedCheeps()
    {
        // Path to test CSV inside the test project
        var testCsvRelative = Path.Combine("TestData", "test_data_small.csv");
        var testCsvFullPath = Path.GetFullPath(testCsvRelative);

        // Determine CLI project path relative to the repo
        var repoRoot = Directory.GetParent(Directory.GetCurrentDirectory())
            .Parent.Parent.Parent.FullName; // Navigate from bin/Debug/netX
        var cliProjectPath = Path.Combine(repoRoot, "src", "Chirp.CLI");

        // Run CLI read command using the test CSV
        var result = RunCommand(
            "dotnet",
            $"run -- read 10 --file \"{testCsvFullPath}\"",
            cliProjectPath
        );

        Assert.Equal(0, result.ExitCode);

        var expectedOutput = new[]
        {
            "ropf @ 08/01/23 14:09:20: Hello, BDSA students!",
            "rnie @ 08/02/23 14:19:38: Welcome to the course!",
            "rnie @ 08/02/23 14:37:38: I hope you had a good summer.",
            "ropf @ 08/02/23 15:04:47: Cheeping cheeps on Chirp :)"
        };

        foreach (var line in expectedOutput)
        {
            Assert.Contains(line, result.Output);
        }
    }

}