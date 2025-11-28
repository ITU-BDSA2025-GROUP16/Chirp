using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Chirp.Core.Domain;
using Chirp.Core.Services;

namespace Chirp.Web.Pages;

[Authorize]
public class InformationPageModel : PageModel
{
        private readonly ICheepService _service;

    private readonly UserManager<Author> _userManager;
    
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    public string? Author { get; set; } = string.Empty;

    public int CurrentPage { get; set; } = 1;

    public List<CheepViewModel> Cheeps { get; set; } = new();

    public InformationPageModel(ICheepService service, UserManager<Author> userManager)
    {
         _service = service;
        _userManager = userManager;
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
    }
}