using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MyChat.Tests;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>> //Large pats of these tests where made in collaboration with LLM
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PublicTimeline_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PublicTimeline_ContainsHelgeCheep()
    {
        // Act
        var response = await _client.GetAsync("/Roger%20Histand");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("understand", content);
    }

    [Fact]
    public async Task UserTimeline_Adrian_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/Adrian");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UserTimeline_Adrian_ContainsAdrianCheep()
    {
        // Act
        var response = await _client.GetAsync("/Adrian");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Adrian", content);
        Assert.Contains("Hej, velkommen til kurset.", content);
    }

    [Fact]
    public async Task PublicTimeline_ContainsPublicTimelineHeader()
    {
        // Act
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Public Timeline", content);
    }

    [Fact]
    public async Task UserTimeline_ContainsUsernameInHeader()
    {
        // Act
        var response = await _client.GetAsync("/Adrian");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("Hej, velkommen til kurset.", content);
    }

    [Fact]
    public async Task PublicTimeline_WithPageParameter_ReturnsCorrectPage()
    {
        var response = await _client.GetAsync("/?page=2");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Contains("Public Timeline - Page 2", content);
    }
}   