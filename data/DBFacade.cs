using Microsoft.Data.Sqlite;

public class DBFacade
{
    private readonly string _dbPath;

    public DBFacade(string dbPath)
    {
        _dbPath = dbPath;
    }

    public List<CheepViewModel> GetCheeps(int pageNumber = 1)
    {
        var cheeps = new List<CheepViewModel>();
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
            SELECT u.username, m.text, m.pub_date
            FROM message m
            JOIN user u ON m.author_id = u.user_id
            ORDER BY m.pub_date DESC 
            LIMIT $limit OFFSET $offset
        ";
        
        int limit = 32;
        int offset = (pageNumber - 1) * limit;
        command.Parameters.AddWithValue("$limit", limit);
        command.Parameters.AddWithValue("$offset", offset);
        
        Console.WriteLine($"Page: {pageNumber}, Offset: {offset}, Limit: {limit}");

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            cheeps.Add(new CheepViewModel(
                reader.GetString(0),
                reader.GetString(1),
                UnixTimeStampToDateTimeString(reader.GetInt64(2)) // convert pub_date
            ));
        }

        return cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNumber = 1)
    {
        var cheeps = new List<CheepViewModel>();
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
            SELECT u.username, m.text, m.pub_date
            FROM message m
            JOIN user u ON m.author_id = u.user_id
            WHERE u.username = $author
            ORDER BY m.pub_date DESC 
            LIMIT $limit OFFSET $offset
        ";
        
        int limit = 32;
        int offset = (pageNumber - 1) * limit;
        command.Parameters.Add("$author", SqliteType.Text).Value = author;
        command.Parameters.Add("$limit", SqliteType.Integer).Value = limit;
        command.Parameters.Add("$offset", SqliteType.Integer).Value = offset;

        Console.WriteLine($"Author: {author}, Page: {pageNumber}, Offset: {offset}, Limit: {limit}");


        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            cheeps.Add(new CheepViewModel(
                reader.GetString(0),
                reader.GetString(1),
                UnixTimeStampToDateTimeString(reader.GetInt64(2))
            ));
        }

        return cheeps;
    }

    private static string UnixTimeStampToDateTimeString(long unixTimeStamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
}
