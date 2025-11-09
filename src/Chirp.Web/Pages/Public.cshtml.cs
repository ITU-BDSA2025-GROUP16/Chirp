using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Chirp.Core.Services;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Chirp.Core.Domain;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    private readonly UserManager<Author> _userManager;
    public string? Author { get; set; } = string.Empty;
    public List<CheepViewModel> Cheeps { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public string NewCheepText { get; set; } = string.Empty;

    public PublicModel(ICheepService service, UserManager<Author> userManager)
    {
        _service = service;
        _userManager = userManager;
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
    
    public async Task<IActionResult> OnPostAsync()
    {
        Author = RouteData.Values["author"]?.ToString();

        if (string.IsNullOrEmpty(Author))
        {
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null || currentUser.Name != Author)
        {
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(NewCheepText) || NewCheepText.Length > 160)
        {
            ModelState.AddModelError(string.Empty, "Cheep text must be between 1 and 160 characters.");
            return Page();
        }

        await _service.CreateCheep(currentUser, NewCheepText);

        return RedirectToPage($"/{currentUser.UserName}");
    }
    

}

