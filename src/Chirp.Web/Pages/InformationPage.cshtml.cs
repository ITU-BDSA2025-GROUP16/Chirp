using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Chirp.Core.Domain;
using Chirp.Core.Services;
using Chirp.Infrastructure.Repositories;
using Chirp.Core.Interfaces;

namespace Chirp.Web.Pages;

[Authorize]
public class InformationPageModel : PageModel
{
    private readonly ICheepService _service;
    private readonly IFollowService _followService; 

    private readonly ILikeService _likeService;

    private readonly UserManager<Author> _userManager;
    private readonly IAuthorRepository _authorRepository;
    
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public string? Author { get; set; } = string.Empty;

    public int CurrentPage { get; set; } = 1;

    public List<CheepViewModel> Cheeps { get; set; } = new();

    public HashSet<int> FollowedAuthorIds { get; set; } = new();
    public List<Author> FollowedAuthors { get; set; } = new();
    

    public InformationPageModel(ICheepService service, IFollowService followService, ILikeService likeService, UserManager<Author> userManager, IAuthorRepository authorRepository)
    {
         _service = service;
         _followService = followService;
        _userManager = userManager;
        _authorRepository = authorRepository;
        _likeService = likeService;
    }

    public async Task OnGetAsync()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return;
        }

        Name = currentUser.Name ?? string.Empty;
        Username = currentUser.UserName ?? string.Empty;
        Email = currentUser.Email ?? string.Empty;


        //Cheeps
        Author = User.Identity?.Name;

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


        //Follow
        var userIdString = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userIdString))
        {
            Cheeps = new();
            return;
        }
        int userId = int.Parse(userIdString);




        FollowedAuthorIds = await _followService.GetFollowedIds(userId);
        
        foreach (int id in FollowedAuthorIds)
        {
            var author = _authorRepository.GetAuthorFromId(id);
            if (author != null)
                {
                    FollowedAuthors.Add(author);
                }
        }

    }
    public async Task<IActionResult> OnPostForgetMeAsync([FromServices] SignInManager<Author> signInManager)
    {
        Console.WriteLine("=== OnPostForgetMeAsync Called ===");
        
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return RedirectToPage("/Public");
        }

        await _service.DeleteUserData(currentUser);
        await _followService.DeleteFollowersData(currentUser);
        await _likeService.DeleteLikesData(currentUser);

        var result = await _userManager.DeleteAsync(currentUser);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Error deleting user account.");
            return Page();
        }

        await signInManager.SignOutAsync();

        Console.WriteLine("User account and data deleted.");

        return RedirectToPage("/Public");
    }
}