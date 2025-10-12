namespace MyChat.Razor.Model;

public class Author
{
    public int AuthorId { get; set; }  // EF Core will consider this the primary key by convention
    public string Name { get; set; }
    public string Email { get; set; }

    // Navigation property
    public ICollection<Cheep> Cheeps { get; set; }
}