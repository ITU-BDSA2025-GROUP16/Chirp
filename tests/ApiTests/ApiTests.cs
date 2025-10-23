using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MyChat.Razor.Tests;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>> 
{
    //Tests for website using temporary version
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
        var response = await _client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PublicTimeline_ContainsHelgeCheep()
    {
        var response = await _client.GetAsync("/Roger%20Histand");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("understand", content);
    }

    [Fact]
    public async Task UserTimeline_Adrian_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/Adrian");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UserTimeline_Adrian_ContainsAdrianCheep()
    {
        var response = await _client.GetAsync("/Adrian");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Adrian", content);
        Assert.Contains("Hej, velkommen til kurset.", content);
    }

    [Fact]
    public async Task PublicTimeline_ContainsPublicTimelineHeader()
    {
        var response = await _client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Public Timeline", content);
    }

    [Fact]
    public async Task UserTimeline_ContainsUsernameInHeader()
    {
        var response = await _client.GetAsync("/Adrian");
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Hej, velkommen til kurset.", content);
    }

    [Fact]
    public async Task PublicTimeline_WithPageParameter_ReturnsCorrectPage()
    {
        var response = await _client.GetAsync("/?page=2");
        var content = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
        Assert.Contains("Public Timeline - Page 2", content);
    }
}   