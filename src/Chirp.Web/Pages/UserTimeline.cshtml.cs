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
    private readonly UserManager<Author> _userManager;
    public List<CheepViewModel> Cheeps { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public string? Author { get; set; } = string.Empty;
    [BindProperty]
    public string NewCheepText { get; set; } = string.Empty;
    public UserTimelineModel(ICheepService service, UserManager<Author> userManager)
    {
        _service = service;
        _userManager = userManager;
    }

    public void OnGet()
    {

        Author = RouteData.Values["author"]?.ToString();
        int pageNumber = 1;
        string? pageQuery = HttpContext.Request.Query["page"];
        if (!string.IsNullOrEmpty(pageQuery) && int.TryParse(pageQuery, out int parsedPage))
        {
            pageNumber = parsedPage > 0 ? parsedPage : 1;
        }

        CurrentPage = pageNumber;

        Console.WriteLine(pageNumber);

        if (string.IsNullOrEmpty(Author))
        {
            Cheeps = new();
            return;
        }

        Cheeps = _service.GetCheepsFromAuthor(Author, pageNumber);
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

        return Redirect("/");
    }
}
