using Microsoft.Extensions.Logging;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Contracts.DTOs.Question;
using QuizMaker.Application.Contracts.DTOs.Quiz;
using QuizMaker.Application.Exceptions;
using QuizMaker.Application.Interfaces.Repositories;
using QuizMaker.Application.Interfaces.Services;
using QuizMaker.Application.Interfaces.UnitOfWork;
using QuizMaker.Application.Mappings;
using QuizMaker.Domain.Entities;
using Serilog;

namespace QuizMaker.Application.Services;
public class QuizService : IQuizService {

    private readonly IQuizRepository _quizRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<QuizService> _logger;

    public QuizService(IQuizRepository quizRepository, IQuestionRepository questionRepository, IUnitOfWork unitOfWork, ILogger<QuizService> logger) {
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<QuizDetailDto> AddAsync(QuizCreateDto quizCreateDto, CancellationToken cancellationToken) {

        _logger.LogInformation("Creating new quiz: {QuizName}", quizCreateDto.Name);

        var quiz = new Quiz(quizCreateDto.Name);

        await BindQuestionsToQuizAsync(quiz, quizCreateDto.Questions, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return quiz.ToDetailDto()!;

    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken) {

        _logger.LogInformation("Attempting to delete Quiz with Id {ID}", id);
        var quiz = await _quizRepository.GetByIdAsync(id, cancellationToken);

        if (quiz == null) {
            _logger.LogWarning("Quiz with Id {ID} not found. Deletion Failed!", id);
            throw new NotFoundException("Quiz with Id {ID} not found.", id);
        }

        _quizRepository.Delete(quiz);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Quiz with ID {ID} successfully deleted.", id);

    }

    public async Task<CursorResult<QuizListDto>> GetAllAsync(string? cursor, int pageSize, CancellationToken cancellationToken) {

        DateTime? cursorDT = DateTime.TryParse(cursor, out var dt) ? dt : (DateTime?)null;
        var quizzes = await _quizRepository.GetAllAsync(cursorDT, pageSize, cancellationToken);

        if (!quizzes.Items.Any()) {
            _logger.LogWarning("No Quizzes found");
            return new CursorResult<QuizListDto>() { Items = [] };
        }

        var items = quizzes.Items
            .Select(q => q.ToListDto()!)
            .ToList();

        _logger.LogDebug("Found {Count} Quizzes", items.Count);

        return new CursorResult<QuizListDto>() {
            HasNextPage = quizzes.HasNextPage,
            NextCursor = quizzes.NextCursor,
            Items = items
        };

    }

    public async Task<QuizDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken) {
        var quiz = await _quizRepository.GetFullByIdAsync(id, cancellationToken, shouldTrack: false);

        if (quiz == null) {
            _logger.LogWarning("Quiz with Id {ID} not found.", id);
            throw new NotFoundException("Quiz with Id {ID} not found.", id);
        }

        return quiz.ToDetailDto()!;
    }

    public async Task<QuizDetailDto> UpdateAsync(QuizUpdateDto quizUpdateDto, CancellationToken cancellationToken) {

        if (quizUpdateDto.Id == Guid.Empty) {
            throw new ArgumentException("Quiz ID is required for update.");
        }

        var quiz = await _quizRepository.GetFullByIdAsync(quizUpdateDto.Id, cancellationToken, shouldTrack: true);

        if (quiz == null) {
            throw new NotFoundException($"Quiz {quizUpdateDto.Id} not found");
        }

        quiz.UpdateName(quizUpdateDto.Name);

        quiz.ClearQuestions();

        await BindQuestionsToQuizAsync(quiz, quizUpdateDto.Questions, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return quiz.ToDetailDto()!;
    }

    public async Task<QuizDetailDto> UpdatePartialAsync(QuizUpdateDto quizUpdateDto, CancellationToken cancellationToken) {
        if (quizUpdateDto.Id == Guid.Empty) {
            throw new ArgumentException("Quiz ID is required for update.");
        }

        var quiz = await _quizRepository.GetFullByIdAsync(quizUpdateDto.Id, cancellationToken, shouldTrack: true);

        if (quiz == null) {
            throw new NotFoundException($"Quiz with ID {quizUpdateDto.Id} not found.");
        }

        _logger.LogInformation("Updating Quiz {QuizId}. Partial update.", quiz.Id);

        if (!string.IsNullOrEmpty(quizUpdateDto.Name)) {
            quiz.UpdateName(quizUpdateDto.Name);
        }

        if (quizUpdateDto.Questions != null) {
            quiz.ClearQuestions();

            await BindQuestionsToQuizAsync(quiz, quizUpdateDto.Questions, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return quiz.ToDetailDto()!;
    }


    /// <summary>
    /// Associates a collection of questions with a quiz, reusing existing questions where possible and creating new
    /// ones as needed.
    /// </summary>
    /// <param name="quiz">The quiz entity to which questions will be bound.</param>
    /// <param name="questionDtos">A collection of question details to be associated with the quiz.</param>
    /// <param name="cancellationToken">Token for cancelling the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task BindQuestionsToQuizAsync(Quiz quiz, IEnumerable<QuestionDetailDto> questionDtos, CancellationToken cancellationToken) {

        if (questionDtos == null || !questionDtos.Any()) {
            return;
        }

        var existingIds = questionDtos
            .Where(q => q.Id != Guid.Empty)
            .Select(q => q.Id)
            .Distinct()
            .ToList();

        var existingQuestions = new List<Question>();
        if (existingIds.Count > 0) {
            _logger.LogDebug("Fetching {Count} existing questions for reuse.", existingIds.Count);
            var fetched = await _questionRepository.GetByIdsAsync(existingIds, cancellationToken);
            existingQuestions.AddRange(fetched);
        }

        int orderIndex = 0;
        foreach (var qDto in questionDtos) {
            Question? questionEntity = null;

            if (qDto.Id != Guid.Empty) {
                questionEntity = existingQuestions.FirstOrDefault(q => q.Id == qDto.Id);

                if (questionEntity == null) {
                    _logger.LogWarning("Question ID {Id} provided but not found. Creating as new.", qDto.Id);
                    questionEntity = new Question(qDto.Text, qDto.Answer);
                }
            }
            else {
                questionEntity = new Question(qDto.Text, qDto.Answer);
            }

            quiz.AddQuestion(questionEntity, orderIndex++);
        }
    }
}
