using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MyChat.Razor.Tests;
//Fake login system
//Simple authentication handler for testign purposes.
//Allows tests to authenticate as any user by setting a claim.

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "TestScheme";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        
        if (!Request.Headers.ContainsKey("X-Test-User"))
        {
            return Task.FromResult(AuthenticateResult.NoResult()); // No auth header = not logged in
        }

        var username = Request.Headers["X-Test-User"].ToString();

        // Generate a numeric ID for the test user
        // Use a simple hash of the username to get a consistent numeric ID
        var userId = Math.Abs(username.GetHashCode() % 1000000).ToString();

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),  // Must be numeric for your app
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Email, $"{username}@test.com")
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}