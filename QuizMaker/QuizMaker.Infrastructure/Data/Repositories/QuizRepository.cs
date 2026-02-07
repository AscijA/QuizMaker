using Microsoft.Extensions.Logging;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Interfaces.Repositories;
using QuizMaker.Domain.Entities;

namespace QuizMaker.Infrastructure.Data.Repositories;
public class QuizRepository : IQuizRepository {
    private readonly ILogger<QuizRepository> _logger;

    public QuizRepository(ILogger<QuizRepository> logger) {
        _logger = logger;
    }

    public async Task AddAsync(Quiz quiz, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

    public async Task<CursorResult<Quiz>> GetAllAsync(DateTime? cursor, int pageSize, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

    public async Task<Quiz?> GetByIdAsync(Guid id, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(Quiz quiz, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
