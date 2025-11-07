using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class LoginModel : PageModel
{
    public async Task OnGetAsync()
    {
        await HttpContext.ChallengeAsync("GitHub", new AuthenticationProperties 
        { 
            RedirectUri = "/" 
        });
    }
}