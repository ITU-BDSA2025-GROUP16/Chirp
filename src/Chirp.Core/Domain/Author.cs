using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.Domain;

public class Author : IdentityUser<int>
{
    // This will now act as the PK instead of Id
    public override int Id
    {
        get => AuthorId;
        set => AuthorId = value;
    }
    public int AuthorId { get; set; }  // EF Core will consider this the primary key by convention
    public string Name { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();


}