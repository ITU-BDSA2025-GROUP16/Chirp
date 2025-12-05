using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Chirp.Core.Services;
using Chirp.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Chirp.Core.Domain;

namespace Chirp.Web.Pages;

public class PopularModel : PageModel
{
    private readonly ICheepService _service;
    private readonly IFollowService _followService;
    private readonly ILikeService _likeService;
    private readonly UserManager<Author> _userManager;
    
    public List<CheepViewModel> Cheeps { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public string? Author { get; set; }

    public HashSet<int> FollowedAuthorIds { get; set; } = new();
    public HashSet<int> LikedCheepIds { get; set; } = new();
    
    [BindProperty]
    public int FollowedId { get; set; }

    [BindProperty]
    public int CheepId { get; set; }
    
    public PopularModel(ICheepService service, IFollowService followService, ILikeService likeService, UserManager<Author> userManager)
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
        Cheeps = _service.GetCheepsByLikes(pageNumber);
    
        if (User.Identity?.IsAuthenticated ?? false)
        {
            var userIdString = _userManager.GetUserId(User);
            if (!string.IsNullOrEmpty(userIdString))
            {
                int userId = int.Parse(userIdString);
                FollowedAuthorIds = await _followService.GetFollowedIds(userId);
                LikedCheepIds = await _likeService.GetLikedCheepIds(userId);
                
                var currentUser = await _userManager.GetUserAsync(User);
                Author = currentUser?.Name;
            }
        }
    }

    public async Task<IActionResult> OnPostFollowAsync()
    {
        var userIdString = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userIdString))
        {
            return Page();
        }
        int followerId = int.Parse(userIdString);

        bool isFollowing = await _followService.IsFollowing(followerId, FollowedId);

        if (isFollowing)
        {
            await _followService.Unfollow(followerId, FollowedId);
        }
        else
        {
            await _followService.Follow(followerId, FollowedId);
        }

        return Redirect("/popular");
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
        }
        else
        {
            await _likeService.Like(userId, CheepId);
        }

        return Redirect("/popular");
    }
}