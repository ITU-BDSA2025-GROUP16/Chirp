using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Chirp.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Web;

namespace MyChat.Razor.Tests;

public class LikeE2ETests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public LikeE2ETests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AuthenticatedUser_CanLikeCheep_ViaPrivateTimeline()
    {
        // Arrange
        var username = "TestUser_ForLike";
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true, AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add("X-Test-User", username);

        int targetCheepId;
        int expectedLikerId = Math.Abs(username.GetHashCode() % 1000000);

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ChatDBContext>();
            // Pick a cheep not authored by the test user to like
            var cheep = db.Cheeps.OrderBy(c => c.CheepId).FirstOrDefault(c => c.AuthorId != expectedLikerId);
            Assert.NotNull(cheep);
            targetCheepId = cheep.CheepId;
        }

        // Act: GET the private page to obtain antiforgery token and cookies
        var privateUrl = "/private/test";
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
            new KeyValuePair<string, string>("CheepId", targetCheepId.ToString()),
            new KeyValuePair<string, string>("__RequestVerificationToken", token)
        });

        var postResp = await client.PostAsync(privateUrl + "?handler=Like", form);

        // Assert redirect (Razor page typically redirects after POST)
        Assert.True(postResp.StatusCode == HttpStatusCode.Redirect || postResp.StatusCode == HttpStatusCode.Found,
            $"Expected redirect status after POST, got {postResp.StatusCode}");

        // Verify the like record was created in the test database
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ChatDBContext>();
            var exists = db.Likes.Any(l => l.LikerId == expectedLikerId && l.LikedCheepId == targetCheepId);
            Assert.True(exists, "Like record was not created in the database.");
        }
    }
}
