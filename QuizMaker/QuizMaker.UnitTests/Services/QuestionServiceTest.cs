using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using QuizMaker.Application.Services;
using QuizMaker.Application.Interfaces.Repositories;
using QuizMaker.Application.Contracts.DTOs.Question;
using QuizMaker.Application.Common.Results;
using QuizMaker.Domain.Entities;
using System.Globalization;

namespace QuizMaker.UnitTests.Services;

public class QuestionServiceTests {
    private readonly Mock<IQuestionRepository> _questionRepoMock;
    private readonly Mock<ILogger<QuestionService>> _loggerMock;
    private readonly QuestionService _sut;

    public QuestionServiceTests() {
        _questionRepoMock = new Mock<IQuestionRepository>();
        _loggerMock = new Mock<ILogger<QuestionService>>();

        _sut = new QuestionService(
            _questionRepoMock.Object,
            _loggerMock.Object);
    }

    // GET BY IDS

    [Fact]
    public async Task GetByIDsAsync_WhenIdsListIsNull_ThrowsArgumentException() {
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.GetByIDsAsync(null!, CancellationToken.None));

        Assert.Equal("No Question IDs passed", ex.Message);
    }

    [Fact]
    public async Task GetByIDsAsync_WhenIdsListIsEmpty_ThrowsArgumentException() {
        var emptyList = new List<Guid>();

        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.GetByIDsAsync(emptyList, CancellationToken.None));

        Assert.Equal("No Question IDs passed", ex.Message);
    }

    [Fact]
    public async Task GetByIDsAsync_WhenRepoReturnsEmpty_ReturnsEmptyEnumerable() {
        var ids = new List<Guid> { Guid.NewGuid() };

        _questionRepoMock.Setup(x => x.GetByIdsAsync(ids, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Question>());

        var result = await _sut.GetByIDsAsync(ids, CancellationToken.None);

        Assert.Empty(result);

    }

    [Fact]
    public async Task GetByIDsAsync_WhenRepoReturnsItems_ReturnsMappedDtos() {
        var id1 = Guid.NewGuid();
        var ids = new List<Guid> { id1 };
        var questions = new List<Question> { new Question("Text", "Answer") };

        _questionRepoMock.Setup(x => x.GetByIdsAsync(ids, It.IsAny<CancellationToken>()))
            .ReturnsAsync(questions);

        var result = await _sut.GetByIDsAsync(ids, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Text", result.First().Text);
        Assert.Equal("Answer", result.First().Answer);
    }

    // SEARCH

    [Fact]
    public async Task SearchAsync_WhenRepoReturnsEmpty_ReturnsEmptyResult() {
        var emptyRepoResult = new CursorResult<Question> {
            Items = new List<Question>(),
            HasNextPage = false
        };

        _questionRepoMock.Setup(x => x.SearchAsync(
                It.IsAny<string?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyRepoResult);

        var result = await _sut.SearchAsync("test", null, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task SearchAsync_WithValidCursor_ParsesDateAndPassesToRepo() {
        var cursorStr = "08.02.2025";
        var parsedDate = DateTime.ParseExact(cursorStr, "dd.MM.yyyy", CultureInfo.InvariantCulture);

        var searchText = "math";

        var repoResult = new CursorResult<Question> {
            Items = new List<Question> { new Question("Math Q", "42") },
            HasNextPage = true
        };

        _questionRepoMock.Setup(x => x.SearchAsync(
                searchText,
                parsedDate,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(repoResult);

        var result = await _sut.SearchAsync(searchText, cursorStr, 10, CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal("Math Q", result.Items.First().Text);

        Assert.Null(result.NextCursor);
    }

    [Fact]
    public async Task SearchAsync_WithInvalidCursor_PassesNullDateToRepo() {
        // Arrange
        var invalidCursor = "invalid-date";

        var repoResult = new CursorResult<Question> {
            Items = new List<Question> { new Question("Q", "A") }
        };

        _questionRepoMock.Setup(x => x.SearchAsync(
                It.IsAny<string?>(),
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(repoResult);

        await _sut.SearchAsync("test", invalidCursor, 10, CancellationToken.None);

        _questionRepoMock.Verify(x => x.SearchAsync(
            It.IsAny<string?>(),
            null,
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // GET ALL

    [Fact]
    public async Task GetAllAsync_WhenRepoReturnsEmpty_ReturnsEmptyResult() {
        var emptyRepoResult = new CursorResult<Question> {
            Items = new List<Question>(),
            HasNextPage = false
        };

        _questionRepoMock.Setup(x => x.SearchAsync(
                It.IsAny<string?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyRepoResult);

        var result = await _sut.GetAllAsync(null, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetAllAsync_WithValidCursor_ParsesDateAndPassesToRepo() {
        var cursorStr = "08.02.2025";
        var parsedDate = DateTime.ParseExact(cursorStr, "dd.MM.yyyy", CultureInfo.InvariantCulture);


        var repoResult = new CursorResult<Question> {
            Items = new List<Question> { new Question("Math Q", "42") },
            HasNextPage = true
        };

        _questionRepoMock.Setup(x => x.SearchAsync(
                null,
                parsedDate,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(repoResult);

        var result = await _sut.GetAllAsync(cursorStr, 10, CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal("Math Q", result.Items.First().Text);

        Assert.Null(result.NextCursor);
    }

    [Fact]
    public async Task GetAllAsync_WithInvalidCursor_PassesNullDateToRepo() {
        // Arrange
        var invalidCursor = "invalid-date";

        var repoResult = new CursorResult<Question> {
            Items = new List<Question> { new Question("Q", "A") }
        };

        _questionRepoMock.Setup(x => x.SearchAsync(
                null,
                null,
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(repoResult);

        await _sut.GetAllAsync(invalidCursor, 10, CancellationToken.None);

        _questionRepoMock.Verify(x => x.SearchAsync(
            null,
            null,
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}