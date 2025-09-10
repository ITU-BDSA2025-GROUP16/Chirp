    using SimpleDB;
    using CommandLine;

    namespace Chirp.CLI;

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
        private static readonly IDatabaseRepository<Cheep> _database =
            new CSVDatabase<Cheep>(@"..\..\data\chirp_cli_db.csv");

        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Client.UserInterface.ShowHelp();
                return 1;
            }

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
    }