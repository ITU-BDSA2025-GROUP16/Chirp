using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core.Services;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Chirp.Core.Domain;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    private readonly IFollowService _followService;
    private readonly UserManager<Author> _userManager;
    
    public List<CheepViewModel> Cheeps { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public string? Author { get; set; } = string.Empty;
    public int? AuthorId { get; set; }
    public bool IsFollowing { get; set; } = false;
    
    [BindProperty]
    public string NewCheepText { get; set; } = string.Empty;
    
    [BindProperty]
    public int FollowedId { get; set; }

    public UserTimelineModel(ICheepService service, IFollowService followService, UserManager<Author> userManager)
    {
        _service = service;
        _followService = followService;
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        Author = RouteData.Values["author"]?.ToString();
        int pageNumber = 1;
        string? pageQuery = HttpContext.Request.Query["page"];
        if (!string.IsNullOrEmpty(pageQuery) && int.TryParse(pageQuery, out int parsedPage))
        {
            pageNumber = parsedPage > 0 ? parsedPage : 1;
        }

        CurrentPage = pageNumber;

        if (string.IsNullOrEmpty(Author))
        {
            Cheeps = new();
            return;
        }

        Cheeps = _service.GetCheepsFromAuthor(Author, pageNumber);
        
        if (User.Identity?.IsAuthenticated == true)
        {
            var userIdString = _userManager.GetUserId(User);
            if (!string.IsNullOrEmpty(userIdString))
            {
                int currentUserId = int.Parse(userIdString);
                
                if (Cheeps.Any())
                {
                    AuthorId = Cheeps.First().AuthorId;
                    IsFollowing = await _followService.IsFollowing(currentUserId, AuthorId.Value);
                }
            }
        }
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        Console.WriteLine("=== OnPostAsync Called ===");
        Author = RouteData.Values["author"]?.ToString();

        if (string.IsNullOrEmpty(Author))
        {
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);

        if (string.IsNullOrWhiteSpace(NewCheepText) || NewCheepText.Length > 160)
        {
            ModelState.AddModelError(string.Empty, "Cheep text must be between 1 and 160 characters.");
            return Page();
        }

        await _service.CreateCheep(currentUser, NewCheepText);
        Console.WriteLine("User:" + currentUser);
        return Redirect("/");
    }
    
    public async Task<IActionResult> OnPostFollowAsync()
    {
        Console.WriteLine("=== OnPostFollowAsync Called ===");
        
        Author = RouteData.Values["author"]?.ToString();
        
        var userIdString = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userIdString))
        {
            return Page();
        }
        
        int followerId = int.Parse(userIdString);

        Console.WriteLine($"Follower ID: {followerId}, Target ID: {FollowedId}");

        bool isFollowing = await _followService.IsFollowing(followerId, FollowedId);
        
        if (isFollowing)
        {
            await _followService.Unfollow(followerId, FollowedId);
            Console.WriteLine("Unfollowed!");
        }
        else
        {
            await _followService.Follow(followerId, FollowedId);
            Console.WriteLine("Followed!");
        }
        
        return Redirect($"/user/{Author}");
    }
}