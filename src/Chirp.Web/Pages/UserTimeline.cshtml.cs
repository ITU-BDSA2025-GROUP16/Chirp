using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core.Services;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Chirp.Core.Domain;
using Microsoft.AspNetCore.Authentication;


namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    private readonly IFollowService _followService;
    private readonly ILikeService _likeService;
    private readonly UserManager<Author> _userManager;
    
    public List<CheepViewModel> Cheeps { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public string? Author { get; set; } = string.Empty;
    public int? AuthorId { get; set; }
    public bool IsFollowing { get; set; } = false;
    public HashSet<int> LikedCheepIds { get; set; } = new();
    
    [BindProperty]
    public string NewCheepText { get; set; } = string.Empty;
    
    [BindProperty]
    public int FollowedId { get; set; }

    [BindProperty]
    public int CheepId { get; set; }
    

    public UserTimelineModel(ICheepService service, IFollowService followService, ILikeService likeService, UserManager<Author> userManager)
    {
        _service = service;
        _followService = followService;
        _likeService = likeService;
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
                LikedCheepIds = await _likeService.GetLikedCheepIds(currentUserId);
                
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

    public async Task<IActionResult> OnPostLikeAsync()
    {
        if (!User.Identity.IsAuthenticated)
            return Forbid();

        Author = RouteData.Values["author"]?.ToString();

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

        return Redirect($"/user/{Author}");
    }
}