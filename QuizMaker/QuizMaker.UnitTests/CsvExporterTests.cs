using System.Text;
using QuizMaker.Application.Contracts.DTOs.Question;
using QuizMaker.Application.Contracts.DTOs.Quiz;
using QuizMaker.Infrastructure.Exporters;
using Xunit;

namespace QuizMaker.UnitTests.Exporters;

public class CsvExporterTests {

    [Fact]
    public async Task ExportAsync_ShouldGenerateValidCsvHeader() {
        var exporter = new CsvExporter();
        var quizGuid = Guid.NewGuid();

        var quiz = new QuizDetailDto(quizGuid, "Simple Quiz", new List<QuestionDetailDto>());

        var result = await exporter.ExportAsync(quiz);
        var csvString = Encoding.UTF8.GetString(result);

        Assert.Contains("Quiz Name: Simple Quiz", csvString);
        Assert.Contains("Order,Question Text", csvString);
    }

    [Fact]
    public async Task ExportAsync_ShouldEscapeSpecialCharacters_InCsv() {
        var exporter = new CsvExporter();
        var quizGuid = Guid.NewGuid();
        var q1Id = Guid.NewGuid();
        var q2Id = Guid.NewGuid();
        var quiz = new QuizDetailDto(quizGuid, "Tricky Quiz",
            new List<QuestionDetailDto> {
                new QuestionDetailDto (q1Id,"What is 1, 2, 3?", "Numbers" ),
                new QuestionDetailDto (q2Id,"Say \"Hello\"","Hi" )
            }
        );

        var result = await exporter.ExportAsync(quiz);
        var csvString = Encoding.UTF8.GetString(result);

        Assert.Contains("\"What is 1, 2, 3?\"", csvString);

        Assert.Contains("\"Say \"\"Hello\"\"\"", csvString);
    }
}