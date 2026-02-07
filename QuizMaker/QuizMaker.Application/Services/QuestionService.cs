using Microsoft.Extensions.Logging;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Contracts.DTOs.Question;
using QuizMaker.Application.Interfaces.Services;

namespace QuizMaker.Application.Services;
public class QuestionService : IQuestionService {

    private readonly ILogger<QuestionService> _logger;

    public QuestionService(ILogger<QuestionService> logger) {
        _logger = logger;
    }
    public async Task<IEnumerable<QuestionDetailDto>> GetByIDsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

    public async Task<CursorResult<QuestionDetailDto>> SearchAsync(string? searchText, string? cursor, int? pageSize, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
