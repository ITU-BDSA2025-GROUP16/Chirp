namespace Chirp.Tests;
using Chirp.CLI;
using Xunit;

public class ClientTests
{
    [Fact]
    public void UnitCheepCreationTest()
    {
        var author = "tester";
        var message = "hello world";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        var cheep = new Cheep
        {
            Author = author,
            Message = message,
            Timestamp = timestamp
        };
        
        Assert.Equal(author, cheep.Author);
        Assert.Equal(message, cheep.Message);
        Assert.Equal(timestamp, cheep.Timestamp);
    }
    
    [Fact]
    public void UnitTimestampTest()
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
    public void UnitNameTest()
    {
        var Author = Environment.UserName;
        
        var testTimestamp = new DateTimeOffset(2025, 9, 17, 12, 45, 15, TimeSpan.Zero).ToUnixTimeSeconds();
        var cheep = new Cheep
        {
            Author = Environment.UserName,
            Message = "test",
            Timestamp = testTimestamp
        };
        
        
        var dto = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp);
        var formatted = $"{cheep.Author} @ {dto:dd/MM/yy HH:mm:ss}: {cheep.Message}";
        
        var s = " @ 17/09/25 12:45:15: test";
        s = Author + s;
        
        Assert.Equal(s, formatted);
    }

    [Fact]
    public void UnitMessageTest()
    {
        var Author = Environment.UserName;
        
        var testTimestamp = new DateTimeOffset(2025, 9, 17, 12, 45, 15, TimeSpan.Zero).ToUnixTimeSeconds();
        var cheep = new Cheep
        {
            Author = "tester",
            Message = "test message",
            Timestamp = testTimestamp
        };
        
        
        var dto = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp);
        var formatted = $"{cheep.Author} @ {dto:dd/MM/yy HH:mm:ss}: {cheep.Message}";
        
        Assert.Equal("tester @ 17/09/25 12:45:15: test message", formatted);
    }
    
    [Fact]
    public void UnitMessageJoinTest()
    {
        var words = new[] { "Hello", "world", "this", "is", "a", "test" };
        var expectedMessage = "Hello world this is a test";
        var actualMessage = string.Join(" ", words);
        
        Assert.Equal(expectedMessage, actualMessage);
    }
    
    
    [Fact]
    public void UnitHandleEmptyTest()
    {
        var author = "";
        var message = "";
        var timestamp = 0;
        
        var cheep = new Cheep
        {
            Author = author,
            Message = message,
            Timestamp = timestamp
        };
        
        Assert.Equal(author, cheep.Author);
        Assert.Equal(message, cheep.Message);
        Assert.Equal(timestamp, cheep.Timestamp);
    }

}