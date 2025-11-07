using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Chirp.Core.Services;
using Chirp.Core.Interfaces;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; } = new();
    public int CurrentPage { get; set; } = 1;

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public void OnGet()
    {
        int pageNumber = 1;
        string? pageQuery = HttpContext.Request.Query["page"];
        if (!string.IsNullOrEmpty(pageQuery) && int.TryParse(pageQuery, out int parsedPage))
        {
            pageNumber = parsedPage > 0 ? parsedPage : 1;
        }

        CurrentPage = pageNumber;
        Cheeps = _service.GetCheeps(pageNumber);
    }
    [IgnoreAntiforgeryToken]
    
    public IActionResult OnPostGitHubLogin()
    {
        Console.WriteLine("=== GitHub Login Handler Called ===");
        Console.WriteLine($"User authenticated: {User.Identity?.IsAuthenticated}");
        Console.WriteLine($"Request path: {Request.Path}");
        Console.WriteLine($"Request method: {Request.Method}");
        
        try 
        {
            var result = Challenge(new AuthenticationProperties 
            { 
                RedirectUri = "/" 
            }, "GitHub");
            
            Console.WriteLine("Challenge created successfully");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating challenge: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }
    
    
    

}

