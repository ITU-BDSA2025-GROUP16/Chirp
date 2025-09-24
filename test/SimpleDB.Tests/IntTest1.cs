namespace Chirp.Tests;
using SimpleDB;
using Xunit;

public class SimpleDBTests
{
    [Fact]
    public void StoreAndRead()
    {
        var tempFile = Path.GetTempFileName();
        var db = new CSVDatabase<Cheep>(tempFile, forTest: true);
        
        var initialRecords = db.Read().ToList();
        Assert.Empty(initialRecords);
        
        var testCheep = new Cheep
        {
            Author = "Tester",
            Message = "Integration test",
            Timestamp = 123456
        };
        
        db.Store(testCheep);

        var records = db.Read().ToList();
        Assert.Single(records);
        Assert.Equal("Tester", records[0].Author);
        Assert.Equal("Integration test", records[0].Message);

        db.DeleteLast();

        records = db.Read().ToList();
        File.Delete(tempFile);
    }
    
    [Fact]
    public void StoreAndDelete()
    {
        var tempFile = Path.GetTempFileName();
        var db = new CSVDatabase<Cheep>(tempFile, forTest: true);
        
        var initialRecords = db.Read().ToList();
        Assert.Empty(initialRecords);
        
        var testCheep = new Cheep
        {
            Author = "Tester",
            Message = "Integration test",
            Timestamp = 123456
        };
        
        db.Store(testCheep);

        var records = db.Read().ToList();
        Assert.Single(records);

        db.DeleteLast();
        records = db.Read().ToList();
        Assert.Empty(records);
        
        records = db.Read().ToList();
        File.Delete(tempFile);
    }
}