using System.Net;
using System.Net.Http.Json;
using QuizMaker.Api;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Contracts.DTOs.Question;
using QuizMaker.Application.Contracts.DTOs.Quiz;

namespace QuizMaker.IntegrationTests.Controllers;

public class QuizzesControllerTests : IntegrationTestBase {
    public QuizzesControllerTests(CustomWebApplicationFactory<Program> factory)
        : base(factory) { }

    [Fact]
    public async Task Lifecycle_Create_Get_Delete_WorksCorrectly() {
        var newQuiz = new QuizCreateDto(
          "Integration Test Quiz",
          new List<QuestionDetailDto>
            {
                new QuestionDetailDto(Guid.Empty,"Is this a test?", "Yes")
            }
        );

        var createResponse = await _client.PostAsJsonAsync("/api/quizzes", newQuiz);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createdQuiz = await createResponse.Content.ReadFromJsonAsync<QuizDetailDto>();
        Assert.NotNull(createdQuiz);
        Assert.Equal("Integration Test Quiz", createdQuiz.Name);
        Assert.Single(createdQuiz.Questions);

        var getResponse = await _client.GetAsync($"/api/quizzes/{createdQuiz.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetchedQuiz = await getResponse.Content.ReadFromJsonAsync<QuizDetailDto>();
        Assert.Equal(createdQuiz.Id, fetchedQuiz!.Id);

        var listResponse = await _client.GetAsync("/api/quizzes?pageSize=10");

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var cursorResult = await listResponse.Content.ReadFromJsonAsync<CursorResult<QuizListDto>>();
        Assert.NotNull(cursorResult);
        Assert.NotEmpty(cursorResult.Items);
        Assert.Contains(cursorResult.Items, q => q.Id == createdQuiz.Id);

        var deleteResponse = await _client.DeleteAsync($"/api/quizzes/{createdQuiz.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getAgainResponse = await _client.GetAsync($"/api/quizzes/{createdQuiz.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAgainResponse.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenNameIsMissing() {
        var invalidQuiz = new QuizCreateDto("", []);

        var response = await _client.PostAsJsonAsync("/api/quizzes", invalidQuiz);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}