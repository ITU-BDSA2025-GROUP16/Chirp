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

    public HashSet<int> FollowedAuthorIds { get; set; } = new();
    
    [BindProperty]
    public int FollowedId { get; set; }
    
    public PublicModel(ICheepService service, IFollowService serviceA, UserManager<Author> userManager)
    {
        _service = service;
        _userManager = userManager;

        _serviceA = serviceA;
    }

    public async Task OnGetAsync() {
        int pageNumber = 1;
        string? pageQuery = HttpContext.Request.Query["page"];
        if (!string.IsNullOrEmpty(pageQuery) && int.TryParse(pageQuery, out int parsedPage))
        {
            pageNumber = parsedPage > 0 ? parsedPage : 1;
        }

        CurrentPage = pageNumber;
        Cheeps = _service.GetCheeps(pageNumber);
    
        if (User.Identity?.IsAuthenticated ?? false)
        {
            var userIdString = _userManager.GetUserId(User);
            if (!string.IsNullOrEmpty(userIdString))
            {
                int userId = int.Parse(userIdString);
                FollowedAuthorIds = await _serviceA.GetFollowedIds(userId);
            }
        }
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
        //Their id automatic through repo
        Console.WriteLine($"Follower ID: {followerId} AND ID: {FollowedId})");

        bool isFollowing = await _serviceA.IsFollowing(followerId, FollowedId);
    

        if (isFollowing)
        {
            //Unfollow
            await _serviceA.Unfollow(followerId, FollowedId);
            Console.WriteLine("Unfollowed!");
        }
        else
        {
            //Follow
            await _serviceA.Follow(followerId, FollowedId);
            Console.WriteLine("Followed!");
        }


        return Redirect("/");
    }
   
}