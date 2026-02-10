using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using QuizMaker.Application.Services;
using QuizMaker.Application.Interfaces.Repositories;
using QuizMaker.Application.Interfaces.UnitOfWork;
using QuizMaker.Application.Contracts.DTOs.Quiz;
using QuizMaker.Application.Contracts.DTOs.Question;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Exceptions;
using QuizMaker.Domain.Entities;
using System.Globalization;

namespace QuizMaker.UnitTests.Services;

public class QuizServiceTests {
    private readonly Mock<IQuizRepository> _quizRepoMock;
    private readonly Mock<IQuestionRepository> _questionRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<QuizService>> _loggerMock;
    private readonly QuizService _sut;

    public QuizServiceTests() {
        _quizRepoMock = new Mock<IQuizRepository>();
        _questionRepoMock = new Mock<IQuestionRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<QuizService>>();

        _sut = new QuizService(
            _quizRepoMock.Object,
            _questionRepoMock.Object,
            _uowMock.Object,
            _loggerMock.Object);
    }

    // GET BY ID

    [Fact]
    public async Task GetByIdAsync_WhenQuizExists_ReturnsDetailDto() {
        var quizId = Guid.NewGuid();
        var quiz = new Quiz("General Knowledge");

        _quizRepoMock.Setup(x => x.GetFullByIdAsync(quizId, It.IsAny<CancellationToken>(), false))
            .ReturnsAsync(quiz);

        var result = await _sut.GetByIdAsync(quizId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("General Knowledge", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ThrowsNotFoundException() {
        var quizId = Guid.NewGuid();
        _quizRepoMock.Setup(x => x.GetFullByIdAsync(quizId, It.IsAny<CancellationToken>(), false))
            .ReturnsAsync((Quiz?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.GetByIdAsync(quizId, CancellationToken.None));
    }

    // GET ALL

    [Fact]
    public async Task GetAllAsync_WhenNoItems_ReturnsEmptyResult() {
        var repoResult = new CursorResult<Quiz> {
            Items = new List<Quiz>(),
            HasNextPage = false
        };

        _quizRepoMock.Setup(x => x.GetAllAsync(It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(repoResult);

        var result = await _sut.GetAllAsync(null, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public async Task GetAllAsync_WithValidCursor_ParsesDateAndReturnsMappedDtos() {
        var cursorStr = "08.02.2025";
        var parsedDate = DateTime.ParseExact(cursorStr, "dd.MM.yyyy", CultureInfo.InvariantCulture);

        var quizzes = new List<Quiz> { new Quiz("Q1"), new Quiz("Q2") };

        var repoResult = new CursorResult<Quiz> {
            Items = quizzes,
            HasNextPage = true,
            NextCursor = "07.02.2025"
        };

        _quizRepoMock
            .Setup(x => x.GetAllAsync(parsedDate, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(repoResult);

        var result = await _sut.GetAllAsync(cursorStr, 10, CancellationToken.None);

        Assert.Equal(2, result.Items.Count());
        Assert.Equal("Q1", result.Items.First().Name);
        Assert.True(result.HasNextPage);
        Assert.Equal("07.02.2025", result.NextCursor);

        _quizRepoMock.Verify(
            x => x.GetAllAsync(parsedDate, 10, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    // ADD 
    [Fact]
    public async Task AddAsync_WithNewQuestions_SavesAndReturnsDto() {
        var questionDto = new QuestionDetailDto(Guid.Empty, "2+2", "4");

        var createDto = new QuizCreateDto("Math Quiz", new List<QuestionDetailDto> { questionDto });

        var result = await _sut.AddAsync(createDto, CancellationToken.None);

        Assert.Equal("Math Quiz", result.Name);
        Assert.Single(result.Questions);
        Assert.Equal("2+2", result.Questions.First().Text);

        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WithExistingQuestionId_ReusesExistingEntity() {
        var existingId = Guid.NewGuid();

        var existingQDto = new QuestionDetailDto(existingId, "Existing Q", "A");
        var newQDto = new QuestionDetailDto(Guid.Empty, "New Q", "B");
        var createDto = new QuizCreateDto("Hybrid Quiz", new List<QuestionDetailDto> { existingQDto, newQDto });

        var existingEntity = new Question("Existing Q", "A");
        typeof(Question).GetProperty("Id")?.SetValue(existingEntity, existingId);

        _questionRepoMock.Setup(x => x.GetByIdsAsync(
                It.Is<List<Guid>>(ids => ids.Contains(existingId)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Question> { existingEntity });

        var result = await _sut.AddAsync(createDto, CancellationToken.None);

        _questionRepoMock.Verify(x => x.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(2, result.Questions.Count());
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenQuestionsListIsEmpty_ReturnsEarlyAndDoesNotCallRepo() {
        var createDto = new QuizCreateDto("Empty Quiz", new List<QuestionDetailDto>());

        var result = await _sut.AddAsync(createDto, CancellationToken.None);

        Assert.Empty(result.Questions);

        _questionRepoMock.Verify(x => x.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);

        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenQuestionsListIsNull_ReturnsEarly() {
        var createDto = new QuizCreateDto("Null List Quiz", null!);

        var result = await _sut.AddAsync(createDto, CancellationToken.None);

        Assert.Empty(result.Questions);
        _questionRepoMock.Verify(x => x.GetByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_WhenQuestionIdNotFoundInDb_LogsWarningAndCreatesNewQuestion() {
        var nonExistentId = Guid.NewGuid();
        var qText = "Ghost Question";
        var qAnswer = "Ghost Answer";

        var qDto = new QuestionDetailDto(nonExistentId, qText, qAnswer);
        var createDto = new QuizCreateDto("Fallback Quiz", new List<QuestionDetailDto> { qDto });

        _questionRepoMock.Setup(x => x.GetByIdsAsync(
                It.Is<List<Guid>>(ids => ids.Contains(nonExistentId)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Question>());

        var result = await _sut.AddAsync(createDto, CancellationToken.None);

        Assert.Single(result.Questions);
        var addedQuestion = result.Questions.First();
        Assert.Equal(qText, addedQuestion.Text);
        Assert.Equal(qAnswer, addedQuestion.Answer);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(nonExistentId.ToString())),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // DELETE

    [Fact]
    public async Task DeleteAsync_WhenFound_DeletesAndSaves() {
        var id = Guid.NewGuid();
        var quiz = new Quiz("Delete Me");
        _quizRepoMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(quiz);

        await _sut.DeleteAsync(id, CancellationToken.None);

        _quizRepoMock.Verify(x => x.Delete(quiz), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ThrowsNotFoundException() {
        var id = Guid.NewGuid();
        _quizRepoMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Quiz)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
           _sut.DeleteAsync(id, CancellationToken.None));

        _quizRepoMock.Verify(x => x.Delete((Quiz)null), Times.Never);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }


    // UPDATE

    [Fact]
    public async Task UpdateAsync_WhenIdEmpty_ThrowsArgumentException() {
        var updateDto = new QuizUpdateDto(Guid.Empty, "Name", new List<QuestionDetailDto>());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.UpdateAsync(updateDto, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateAsync_WhenQuizNotFound_ThrowsNotFoundException() {
        var quizId = Guid.NewGuid();

        var updateDto = new QuizUpdateDto(quizId, "Name", new List<QuestionDetailDto>());

        _quizRepoMock.Setup(x => x.GetByIdAsync(updateDto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Quiz)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
          _sut.UpdateAsync(updateDto, CancellationToken.None));

    }

    [Fact]
    public async Task UpdateAsync_Valid_UpdatesNameAndQuestions() {
        var quizId = Guid.NewGuid();
        var existingQuiz = new Quiz("Old Name");

        var qDto = new QuestionDetailDto(Guid.Empty, "New Q", "A");
        var updateDto = new QuizUpdateDto(quizId, "Updated Name", new List<QuestionDetailDto> { qDto });

        _quizRepoMock.Setup(x => x.GetFullByIdAsync(quizId, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(existingQuiz);

        var result = await _sut.UpdateAsync(updateDto, CancellationToken.None);

        Assert.Equal("Updated Name", existingQuiz.Name);
        Assert.Single(existingQuiz.QuizQuestions);
        Assert.Equal("New Q", existingQuiz.QuizQuestions.First().Question.Text);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePartialAsync_NameOnly_DoesNotTouchQuestions() {
        var quizId = Guid.NewGuid();
        var existingQuiz = new Quiz("Old Name");
        existingQuiz.AddQuestion(new Question("Keep Me", "A"), 0);

        var updateDto = new QuizUpdateDto(quizId, "Partial Name", null);

        _quizRepoMock.Setup(x => x.GetFullByIdAsync(quizId, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(existingQuiz);

        await _sut.UpdatePartialAsync(updateDto, CancellationToken.None);

        Assert.Equal("Partial Name", existingQuiz.Name);
        Assert.Single(existingQuiz.QuizQuestions);
        Assert.Equal("Keep Me", existingQuiz.QuizQuestions.First().Question.Text);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePartialAsync_QuestionsOnly_DoesNotTouchName() {
        var quizId = Guid.NewGuid();
        var existingQuiz = new Quiz("Keep Name");

        var qDto = new QuestionDetailDto(Guid.Empty, "New Q", "A");
        var updateDto = new QuizUpdateDto(quizId, null, new List<QuestionDetailDto> { qDto });

        _quizRepoMock.Setup(x => x.GetFullByIdAsync(quizId, It.IsAny<CancellationToken>(), true))
            .ReturnsAsync(existingQuiz);

        await _sut.UpdatePartialAsync(updateDto, CancellationToken.None);

        Assert.Equal("Keep Name", existingQuiz.Name);
        Assert.Single(existingQuiz.QuizQuestions);
        Assert.Equal("New Q", existingQuiz.QuizQuestions.First().Question.Text);
    }

    [Fact]
    public async Task UpdatePartialAsync_WhenIdEmpty_ThrowsArgumentException() {
        var updateDto = new QuizUpdateDto(Guid.Empty, "Name", new List<QuestionDetailDto>());

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.UpdatePartialAsync(updateDto, CancellationToken.None));
    }

    [Fact]
    public async Task UpdatePartialAsync_WhenQuizNotFound_ThrowsNotFoundException() {
        var quizId = Guid.NewGuid();

        var updateDto = new QuizUpdateDto(quizId, "Name", new List<QuestionDetailDto>());

        _quizRepoMock.Setup(x => x.GetByIdAsync(updateDto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Quiz)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
          _sut.UpdatePartialAsync(updateDto, CancellationToken.None));
    }
}