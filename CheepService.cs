public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int pageNumber = 1);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber = 1);
}

public class CheepService : ICheepService
{
    private readonly DBFacade _db;

    public CheepService(DBFacade db)
    {
        _db = db;
    }
    
    
    
    
    // These would normally be loaded from a database for example
    public List<CheepViewModel> GetCheeps(int pageNumber = 1)
    {
        return _db.GetCheeps(pageNumber);
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber = 1)
    {
        return _db.GetCheepsFromAuthor(author, pageNumber);
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
