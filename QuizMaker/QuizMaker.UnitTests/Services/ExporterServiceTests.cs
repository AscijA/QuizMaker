using Microsoft.Extensions.Logging;
using Moq;
using QuizMaker.Application.Contracts.DTOs.Quiz;
using QuizMaker.Application.Interfaces;
using QuizMaker.Application.Interfaces.Services;
using QuizMaker.Application.Services;
using Xunit;

namespace QuizMaker.UnitTests.Services;

public class ExporterServiceTests {
    private readonly Mock<IQuizService> _quizServiceMock;
    private readonly Mock<ILogger<ExporterService>> _loggerMock;
    private readonly ExporterService _sut;

    public ExporterServiceTests() {
        _quizServiceMock = new Mock<IQuizService>();
        _loggerMock = new Mock<ILogger<ExporterService>>();

        _sut = new ExporterService(_quizServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void GetSupportedFormats_ShouldReturnFormats_FromInjectedExporters() {
        var exporterMock = new Mock<IQuizExporter>();
        exporterMock.Setup(x => x.Format).Returns("mockformat");

        _sut.Exporters = new List<IQuizExporter> { exporterMock.Object };

        var result = _sut.GetSupportedFormats().ToList();

        Assert.Single(result);
        Assert.Contains("mockformat", result);
    }

    [Fact]
    public async Task ExportQuizAsync_ShouldThrowException_WhenFormatNotSupported() {
        _sut.Exporters = [];

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.ExportQuizAsync(Guid.NewGuid(), "invalid-format", CancellationToken.None));

        Assert.Contains("not supported", exception.Message);
    }

    [Fact]
    public async Task ExportQuizAsync_ShouldCallCorrectExporter_WhenFormatIsValid() {
        var quizId = Guid.NewGuid();
        var quizDto = new QuizDetailDto(quizId, "Test Quiz", []);
        var expectedBytes = new byte[] { 1, 2, 3 };

        _quizServiceMock.Setup(s => s.GetByIdAsync(quizId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(quizDto);

        var exporterMock = new Mock<IQuizExporter>();
        exporterMock.Setup(x => x.Format).Returns("test");
        exporterMock.Setup(x => x.ContentType).Returns("application/test");
        exporterMock.Setup(x => x.ExportAsync(quizDto))
            .ReturnsAsync(expectedBytes);

        _sut.Exporters = new List<IQuizExporter> { exporterMock.Object };

        var result = await _sut.ExportQuizAsync(quizId, "test", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(expectedBytes, result.Data);
        Assert.Equal("application/test", result.ContentType);
        Assert.Contains("Test Quiz", result.FileName);
    }

    [Fact]
    public async Task ExportQuizAsync_ShouldShowEmptyAvailableFormats_WhenInitializationFailed() {
        _sut.Exporters = new List<IQuizExporter>();

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.ExportQuizAsync(Guid.NewGuid(), "pdf", CancellationToken.None));

        Assert.Contains("Export format 'pdf'", exception.Message);
    }
}