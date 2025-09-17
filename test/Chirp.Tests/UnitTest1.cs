namespace Chirp.Tests;
using Chirp.CLI;
using Xunit;

public class UnitTest1
{
    [Fact]
    public void TimestampTest()
    {
        
        var testTimestamp = new DateTimeOffset(2025, 9, 17, 12, 45, 15, TimeSpan.Zero).ToUnixTimeSeconds();
        var cheep = new Cheep
        {
            Author = "tester",
            Message = "hello world",
            Timestamp = testTimestamp
        };

        var dto = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp);
        var formatted = $"{cheep.Author} @ {dto:dd/MM/yy HH:mm:ss}: {cheep.Message}";


        Assert.Equal("tester @ 17/09/25 12:45:15: hello world", formatted);
    }

    [Fact]
    public void NameTest()
    {
        var Author = Environment.UserName;
        
        var testTimestamp = new DateTimeOffset(2025, 9, 17, 12, 45, 15, TimeSpan.Zero).ToUnixTimeSeconds();
        var cheep = new Cheep
        {
            Author = Author,
            Message = "test",
            Timestamp = testTimestamp
        };
        
        
        var dto = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp);
        var formatted = $"{cheep.Author} @ {dto:dd/MM/yy HH:mm:ss}: {cheep.Message}";
        
        var s = " @ 17/09/25 12:45:15: test";
        s = Author + s;
        
        Assert.Equal(s, formatted);
    }

    
    
}