using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;

namespace MyChat.Razor.Tests;

/// <summary>
/// Extension methods to help create authenticated test clients.
/// </summary>
public static class TestClientExtensions
{
    /// <summary>
    /// Creates an authenticated HttpClient for testing.
    /// The client will be authenticated as the specified username.
    /// </summary>
    /// <param name="factory">The web application factory</param>
    /// <param name="username">The username to authenticate as (e.g., "TestUser")</param>
    /// <param name="allowAutoRedirect">Whether the client should follow redirects automatically</param>
    /// <returns>An HttpClient with authentication headers set</returns>
    public static HttpClient CreateAuthenticatedClient(
        this WebApplicationFactory<Program> factory,
        string username,
        bool allowAutoRedirect = false)
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = allowAutoRedirect
        });

        // Set the test authentication header
        client.DefaultRequestHeaders.Add("X-Test-User", username);

        return client;
    }
}
