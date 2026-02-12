using QuizMaker.Api;

[assembly: Xunit.CollectionBehavior(DisableTestParallelization = true)]
namespace QuizMaker.IntegrationTests;

public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory<Program>> {
    protected readonly HttpClient _client;

    public IntegrationTestBase(CustomWebApplicationFactory<QuizMaker.Api.Program> factory) {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-Api-Key", "TestSecretKey123");
    }
}