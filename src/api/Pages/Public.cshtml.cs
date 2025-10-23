using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyChat.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; } = new();
    public int CurrentPage { get; set; } = 1;

    public PublicModel(ICheepService service)
    {
        _service = service;
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

}
