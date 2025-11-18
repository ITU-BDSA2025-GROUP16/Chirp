using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Chirp.Core.Services;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Chirp.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    private readonly IFollowService _serviceA;

    private readonly UserManager<Author> _userManager;
    private Author? _currentUser;
    
    public string? Author { get; set; } = string.Empty;
    public List<CheepViewModel> Cheeps { get; set; } = new();
    public int CurrentPage { get; set; } = 1;

    public PublicModel(ICheepService service, IFollowService serviceA, UserManager<Author> userManager)
    {
        _service = service;
        _userManager = userManager;

        _serviceA = serviceA;
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

    //FOLLOW AND UNFOLLOW LOGIC:
    [BindProperty]
    public string? AuthorName { get; set; }

   public async Task<IActionResult> OnPostFollowAsync()
    {
        Console.WriteLine("=== OnPostAsync Called ===");
        
        //My id:
        var userIdString = _userManager.GetUserId(User);
        Console.WriteLine("id is: " + userIdString);
        if (string.IsNullOrEmpty(userIdString))
        {
            return Page();
        }
        int followerId = int.Parse(userIdString);



        //Other id:
       var authorToFollow = await _userManager.FindByNameAsync(AuthorName);
       Console.WriteLine($"Looking for author with name: '{AuthorName}'");
        if (authorToFollow == null)
        {
            Console.WriteLine("Author to follow was null");
            return Page();
        }
        int followedId = authorToFollow.Id;



        Console.WriteLine($"Follower ID: {followerId}, Following: {AuthorName} (ID: {followedId})");

        //await _serviceA.Follow(followerId, followedId);
        //Console.WriteLine("User:" + currentUser);
        return Redirect("/");
    }
   
}