using System.Net;
using System.Net.Http.Json;
using System.Text;
using QuizMaker.Application.Contracts.DTOs.Question;
using QuizMaker.Application.Contracts.DTOs.Quiz;
using Xunit;

namespace QuizMaker.IntegrationTests.Controllers;

public class ExportsControllerTests : IntegrationTestBase {
    public ExportsControllerTests(CustomWebApplicationFactory<Api.Program> factory)
        : base(factory) { }

    [Fact]
    public async Task GetExportFormats_ReturnsCsvFormat() {
        var response = await _client.GetAsync("/api/exports/export-formats");

        response.EnsureSuccessStatusCode();
        var formats = await response.Content.ReadFromJsonAsync<IEnumerable<string>>();

        Assert.NotNull(formats);
        Assert.Contains(formats, f => f.Equals("csv", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ExportQuiz_ReturnsValidCsvFile() {
        var quizName = "CSV Test Quiz";
        var questionText = "What is, specifically, the \"answer\"?";

        var quiz = new QuizCreateDto(
                quizName,
                new List<QuestionDetailDto>
                    {   
                         new QuestionDetailDto (Guid.Empty, questionText, "42"),
                         new QuestionDetailDto (Guid.Empty, "Simple Question", "24")
                    }
                );

        var createRes = await _client.PostAsJsonAsync("/api/quizzes", quiz);
        createRes.EnsureSuccessStatusCode();
        var createdQuiz = await createRes.Content.ReadFromJsonAsync<QuizDetailDto>();

        var response = await _client.GetAsync($"/api/exports/{createdQuiz!.Id}/export?format=csv");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal("text/csv", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains(".csv", response.Content.Headers.ContentDisposition?.FileName);

        var fileBytes = await response.Content.ReadAsByteArrayAsync();

        var preamble = Encoding.UTF8.GetPreamble();
        Assert.Equal(preamble[0], fileBytes[0]);
        Assert.Equal(preamble[1], fileBytes[1]);
        Assert.Equal(preamble[2], fileBytes[2]);

        var csvContent = Encoding.UTF8.GetString(fileBytes, preamble.Length, fileBytes.Length - preamble.Length);

        Assert.Contains("Order,Question Text", csvContent);

        Assert.Contains($"1,\"{questionText.Replace("\"", "\"\"")}\"", csvContent);

        Assert.Contains("2,Simple Question", csvContent);
    }

    [Fact]
    public async Task ExportQuiz_ReturnsBadRequest_WhenFormatIsUnsupported() {
        var dummyId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/exports/{dummyId}/export?format=json");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}