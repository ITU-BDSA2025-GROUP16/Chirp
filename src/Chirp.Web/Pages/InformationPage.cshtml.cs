using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Chirp.Core.Domain;

namespace Chirp.Web.Pages;

[Authorize]
public class InformationPageModel : PageModel
{
    private readonly UserManager<Author> _userManager;
    
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    

    public InformationPageModel(UserManager<Author> userManager)
    {
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

    }
}