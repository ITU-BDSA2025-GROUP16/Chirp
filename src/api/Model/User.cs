namespace MyChat.Razor.Model;

public class Author
{
    public int Id { get; set; }  // EF Core will consider this the primary key by convention
    public string Username { get; set; }
    public string Email { get; set; }

    // Navigation property
    public ICollection<Cheep> cheeps { get; set; }
}