using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Chirp.Web;
using System.Threading.Tasks;

namespace MyChat.Razor.Tests;

public class CheepBoxVisibilityTests : IClassFixture<CustomWebApplicationFactory>
{
    // Tests to verify that the cheep box only appears when user is logged in
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CheepBoxVisibilityTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(); // Unauthenticated client by default
    }

    [Fact]
    public async Task PublicTimeline_UnauthenticatedUser_NoCheepBox()
    {
        // Act
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.DoesNotContain("cheepbox", content);
    }

    [Fact]
    public async Task PrivateTimeline_UnauthenticatedUser_NoCheepBox()
    {
        // Act
        var response = await _client.GetAsync("/private/Helge"); //some random private timeline
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.DoesNotContain("cheepbox", content);
        
    }
    [Fact]
    public async Task PrivateTimeline_AuthenticatedUser_ShowCheepBox()
    {
        // Arrange
        var authenticatedClient = _factory.CreateAuthenticatedClient("TestUser");
    
        // Act
        var response = await authenticatedClient.GetAsync("/private/TestUser");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("cheepbox", content);
    }
    [Fact]
    public async Task PrivateTimeline_AuthenticatedUser_ShowCheepBoxForms()
    {
        // Arrange
        var authenticatedClient = _factory.CreateAuthenticatedClient("TestUser");
    
        // Act
        var response = await authenticatedClient.GetAsync("/private/TestUser");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        
        Assert.Contains("What's on your mind", content);
        Assert.Contains("type=\"text\"", content);
        Assert.Contains("type=\"submit\"", content);
        Assert.Contains("value=\"Share\"", content);
    }
    [Fact]
    public async Task PublicTimeline_AuthenticatedUser_NoCheepBox()
    {
        // Arrange
        var authenticatedClient = _factory.CreateAuthenticatedClient("TestUser");
    
        // Act
        var response = await authenticatedClient.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        
        Assert.DoesNotContain("cheepbox", content);
        
        
    }


}
