using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core.Services;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Chirp.Core.Domain;

namespace Chirp.Web.Pages;

public class PrivateTimelineModel : PageModel
{
    private readonly ICheepService _service;
    private readonly IFollowService _followService;
    private readonly ILikeService _likeService;
    private readonly UserManager<Author> _userManager;
    
    public List<CheepViewModel> Cheeps { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public string? Author { get; set; } = string.Empty;

    public HashSet<int> FollowedAuthorIds { get; set; } = new();
    public HashSet<int> LikedCheepIds { get; set; } = new();
    
    [BindProperty]
    public string NewCheepText { get; set; } = string.Empty;
    
    [BindProperty]
    public int FollowedId { get; set; } 

    [BindProperty]
    public int CheepId { get; set; }

    [BindProperty]
    public int AuthorId { get; set; }

    [BindProperty]
    public required string Timestamp { get; set; }

    public PrivateTimelineModel(ICheepService service, IFollowService followService, ILikeService likeService, UserManager<Author> userManager) 
    {
        _service = service;
        _followService = followService;
        _likeService = likeService;
        _userManager = userManager;
    }

    public async Task OnGetAsync() 
{
    int pageNumber = 1;
    string? pageQuery = HttpContext.Request.Query["page"];
    if (!string.IsNullOrEmpty(pageQuery) && int.TryParse(pageQuery, out int parsedPage))
    {
        pageNumber = parsedPage > 0 ? parsedPage : 1;
    }

    CurrentPage = pageNumber;

    var userIdString = _userManager.GetUserId(User);
    if (string.IsNullOrEmpty(userIdString))
    {
        Cheeps = new();
        return;
    }

    int userId = int.Parse(userIdString);
    
    FollowedAuthorIds = await _followService.GetFollowedIds(userId);
    LikedCheepIds = await _likeService.GetLikedCheepIds(userId);
    var authorIdsToShow = new List<int>(FollowedAuthorIds) { userId };
    Cheeps = _service.GetCheepsFromFollowedAuthors(authorIdsToShow.ToList(), pageNumber);
    
    var currentUser = await _userManager.GetUserAsync(User);
    Author = currentUser?.Name;
}
    
    public async Task<IActionResult> OnPostAsync()
    {
        Console.WriteLine("=== OnPostAsync Called ===");
        Author = User.Identity?.Name;

        if (string.IsNullOrEmpty(Author))
        {
            Console.WriteLine("Author null");
            return Page();
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (string.IsNullOrWhiteSpace(NewCheepText) || NewCheepText.Length > 160)
        {
            ModelState.AddModelError(string.Empty, "Cheep text must be between 1 and 160 characters.");
            return Page();
        }
        
        if (currentUser == null)
            return Forbid();
        
        await _service.CreateCheep(currentUser, NewCheepText);
        return Redirect("/private/{author}");
    }


    public async Task<IActionResult> OnPostFollowAsync()
    {
        Console.WriteLine("=== OnPostFollowAsync Called ===");
        
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
        
        return Redirect("/private/{author}");
    }

    public async Task<IActionResult> OnPostLikeAsync()
    {
        if (!User.Identity.IsAuthenticated)
            return Forbid();

        var userIdString = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userIdString))
        {
            return Page();
        }
        int userId = int.Parse(userIdString);

        bool isLiking = await _likeService.IsLiking(userId, CheepId);

        if (isLiking)
        {
            await _likeService.UnLike(userId, CheepId);
            Console.WriteLine("Unliked!");
        }
        else
        {
            await _likeService.Like(userId, CheepId);
            Console.WriteLine("Liked!");
        }

        Console.WriteLine($"User {User.Identity.Name} liked cheep by {AuthorId} at {Timestamp}");
        return Redirect("/private/{author}");
    }
}