using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.Domain;

public class Author : IdentityUser<int>
{
    public string Name { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();


}