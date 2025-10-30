using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core.Services;
using Chirp.Core.Interfaces;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; } = new();
    public int CurrentPage { get; set; } = 1;
    public string? Author { get; set; } = string.Empty;
    public UserTimelineModel(ICheepService service)
    {
        _service = service;
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
}
