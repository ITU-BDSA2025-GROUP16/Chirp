using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.Domain;

public class Author : IdentityUser<int>
{
    public string Name { get; set; } = string.Empty;

    private string _email = string.Empty;
    
    public override string? Email
    {
        get => _email;
        set
        {
            _email = value ?? string.Empty;

            // Set UserName to the email
            UserName = _email;

            // If Name is empty, also set it to the email
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = _email;
            }
        }
    }
    

    // Navigation property
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();


}