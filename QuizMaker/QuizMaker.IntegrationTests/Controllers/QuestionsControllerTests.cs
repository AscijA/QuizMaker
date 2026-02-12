using System.Net;
using System.Net.Http.Json;
using QuizMaker.Api;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Contracts.DTOs.Question;
using QuizMaker.Application.Contracts.DTOs.Quiz;
using Xunit;

namespace QuizMaker.IntegrationTests.Controllers;

public class QuestionsControllerTests : IntegrationTestBase {
    public QuestionsControllerTests(CustomWebApplicationFactory<Program> factory)
        : base(factory) { }

    [Fact (Skip = "DB function mismatch, InMemory doesnt support ILike")]
    public async Task Search_ReturnsMatchingQuestions_WhenTextExists() {
        var uniqueTerm = Guid.NewGuid().ToString().Substring(0, 8);
        var expectedText = $"What is the secret code {uniqueTerm}?";

        var quiz = new QuizCreateDto(
            "Searchable Quiz",
            new List<QuestionDetailDto>
            {
                new QuestionDetailDto (Guid.Empty, expectedText, "42"),
                new QuestionDetailDto (Guid.Empty, "Just another question", "24")
            }
        );

        var createRes = await _client.PostAsJsonAsync("/api/quizzes", quiz);
        createRes.EnsureSuccessStatusCode();

        var response = await _client.GetAsync($"/api/questions/search?searchText={uniqueTerm}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CursorResult<QuestionDetailDto>>();
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(expectedText, result.Items.First().Text);
    }

    [Fact]
    public async Task GetAll_ReturnsPaginatedList() {
        var quiz = new QuizCreateDto(
            "Pagination Quiz",
            new List<QuestionDetailDto>
            {
                new QuestionDetailDto (Guid.Empty, "Q1", "A1"),
                new QuestionDetailDto (Guid.Empty, "Q2", "A2")
            }
        );
        await _client.PostAsJsonAsync("/api/quizzes", quiz);

        var response = await _client.GetAsync("/api/questions?pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<CursorResult<QuestionDetailDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task GetBatch_ReturnsSpecificQuestions_WhenIdsProvided() {
        var quiz = new QuizCreateDto(
            "Batch Test Quiz",
            new List<QuestionDetailDto>
            {
                new QuestionDetailDto (Guid.Empty, "Batch Q1", "A1"),
                new QuestionDetailDto (Guid.Empty, "Batch Q2", "A2")
            }
        );

        var createRes = await _client.PostAsJsonAsync("/api/quizzes", quiz);
        var createdQuiz = await createRes.Content.ReadFromJsonAsync<QuizDetailDto>();

        var questionIds = createdQuiz!.Questions.Select(q => q.Id).ToList();

        var queryString = string.Join("&", questionIds.Select(id => $"ids={id}"));

        var response = await _client.GetAsync($"/api/questions/batch?{queryString}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<QuestionDetailDto>>();
        Assert.NotNull(result);
        Assert.Equal(questionIds.Count, result.Count);
        Assert.All(result, q => Assert.Contains(q.Id, questionIds));
    }
}