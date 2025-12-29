using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Chirp.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Web;

namespace MyChat.Razor.Tests;

public class FollowE2ETests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public FollowE2ETests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AuthenticatedUser_CanFollowAnotherAuthor_ViaPrivateTimeline()
    {
        // Arrange
        var username = "TestUser_ForFollow"; // arbitrary test username
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true, AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-Test-User", username);

        // Pick a target author from the seeded DB
        int targetAuthorId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ChatDBContext>();
            var author = db.Authors.OrderBy(a => a.Id).FirstOrDefault();
            Assert.NotNull(author);
            targetAuthorId = author.Id;
        }

        // Act: GET the private page to obtain antiforgery token and cookies
        var privateUrl = "/private/test"; // route param is not used by handler for auth checks
        var getResp = await client.GetAsync(privateUrl);
        getResp.EnsureSuccessStatusCode();
        var html = await getResp.Content.ReadAsStringAsync();

        // Extract antiforgery token from the HTML
        var tokenMatch = Regex.Match(html, @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""(?<token>[^""]+)"" />", RegexOptions.Singleline);
        if (!tokenMatch.Success)
        {
        // Try a slightly different self-closing tag format
            tokenMatch = Regex.Match(html, @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""(?<token>[^""]+)"">", RegexOptions.Singleline);
        }

        Assert.True(tokenMatch.Success, "Antiforgery token not found in private page HTML.");
        var token = tokenMatch.Groups["token"].Value;

        var form = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("followedId", targetAuthorId.ToString()),
            new KeyValuePair<string, string>("__RequestVerificationToken", token)
        });

        var postResp = await client.PostAsync(privateUrl + "?handler=Follow", form);

        // Assert redirect (Razor page typically redirects after POST)
        Assert.True(postResp.StatusCode == HttpStatusCode.Redirect || postResp.StatusCode == HttpStatusCode.Found,
            $"Expected redirect status after POST, got {postResp.StatusCode}");

        // Compute expected follower id the TestAuthHandler will produce
        int expectedFollowerId = Math.Abs(username.GetHashCode() % 1000000);

        // Verify the follow record was created in the test database
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ChatDBContext>();
            var exists = db.Follows.Any(f => f.FollowerId == expectedFollowerId && f.FollowedId == targetAuthorId);
            Assert.True(exists, "Follow record was not created in the database.");
        }
    }
}
