List<string> cheeps = new() {"Hello, BDSA students!", "Welcome to the course!", "I hope you had a good summer.", "Test2", "test3"};

foreach (var cheep in cheeps)
{
    Console.WriteLine(cheep);
    Thread.Sleep(1000);
}
