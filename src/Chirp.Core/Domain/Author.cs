namespace Chirp.Core.Domain;
public class Author
{
    public int AuthorId { get; set; }  // EF Core will consider this the primary key by convention
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();

    
}