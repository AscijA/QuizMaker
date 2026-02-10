using Microsoft.Extensions.Logging;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Contracts.DTOs.Question;
using QuizMaker.Application.Interfaces.Repositories;
using QuizMaker.Application.Interfaces.Services;
using QuizMaker.Application.Mappings;

namespace QuizMaker.Application.Services;
public class QuestionService : IQuestionService {

    private readonly IQuestionRepository _questionRepository;
    private readonly ILogger<QuestionService> _logger;

    public QuestionService(IQuestionRepository questionRepository, ILogger<QuestionService> logger) {
        _questionRepository = questionRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<QuestionDetailDto>> GetByIDsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken) {
        var idList = ids?.ToList();

        if (idList == null || idList.Count == 0) {
            _logger.LogWarning("GetByIDsAsync called with empty ID list.");
            throw new ArgumentException("No Question IDs passed");
        }

        var items = await _questionRepository
            .GetByIdsAsync(idList, cancellationToken);

        if (!items.Any()) {
            _logger.LogWarning("Requested {Count} IDs, but found 0 questions.", idList.Count);
            return Enumerable.Empty<QuestionDetailDto>();
        }

        _logger.LogDebug("Requested {NumberOfRequestedIDs} IDs, got {ResultNumber}", idList.Count, items.Count());

        return items.Select(q => q.ToDetailDto()!);
    }

    public async Task<CursorResult<QuestionDetailDto>> SearchAsync(string? searchText, string? cursor, int pageSize, CancellationToken cancellationToken) {

        DateTime? cursorDT = DateTime.TryParse(cursor, out var dt) ? dt : (DateTime?)null;

        var questions = await _questionRepository.SearchAsync(searchText, cursorDT, pageSize, cancellationToken);

        if (!questions.Items.Any()) {
            _logger.LogWarning("No Questions found");
            return new CursorResult<QuestionDetailDto>() { Items = [] };
        }

        var items = questions.Items
            .Select(q => q.ToDetailDto()!)
            .ToList();

        return new CursorResult<QuestionDetailDto>() {
            HasNextPage = questions.HasNextPage,
            NextCursor = questions.NextCursor,
            Items = items
        };

    }
}
