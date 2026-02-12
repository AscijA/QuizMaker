using System.Net;
using Xunit;
using QuizMaker.Api;

namespace QuizMaker.IntegrationTests.Controllers;

public class SecurityTests : IClassFixture<CustomWebApplicationFactory<Program>> {
    private readonly HttpClient _client;

    public SecurityTests(CustomWebApplicationFactory<Program> factory) {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ProtectedEndpoint_Returns_Unauthorized_Without_ApiKey() {
        var response = await _client.GetAsync("/api/exports/export-formats");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_Returns_Ok_With_Valid_ApiKey() {
        _client.DefaultRequestHeaders.Add("X-Api-Key", "TestSecretKey123");

        var response = await _client.GetAsync("/api/exports/export-formats");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_Returns_Unauthorized_With_Invalid_ApiKey() {
        _client.DefaultRequestHeaders.Add("X-Api-Key", "WRONG_KEY_123");

        var response = await _client.GetAsync("/api/exports/export-formats");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}