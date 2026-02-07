using Microsoft.Extensions.Logging;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Interfaces.Repositories;
using QuizMaker.Domain.Entities;

namespace QuizMaker.Infrastructure.Data.Repositories;
public class QuestionRepository : IQuestionRepository {

    private readonly ILogger<QuestionRepository> _logger;

    public QuestionRepository(ILogger<QuestionRepository> logger) {
        _logger = logger;
    }

    public async Task<IEnumerable<Question>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

    public async Task<CursorResult<Question>> SearchAsync(string? searchText, string? cursor, int pageSize, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
