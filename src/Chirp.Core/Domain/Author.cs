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
            UserName = value; // Automatically set UserName when Email is set
        } 
    }
    

    // Navigation property
    public ICollection<Cheep> Cheeps { get; set; } = new List<Cheep>();


}