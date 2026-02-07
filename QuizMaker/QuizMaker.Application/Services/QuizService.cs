using Microsoft.Extensions.Logging;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Contracts.DTOs.Quiz;
using QuizMaker.Application.Interfaces.Services;
using Serilog;

namespace QuizMaker.Application.Services;
public class QuizService : IQuizService {

    private readonly ILogger<QuizService> _logger;

    public QuizService(ILogger<QuizService> logger) {
        _logger = logger;
    }

    public async Task<Guid> AddAsync(QuizCreateDto quizCreateDto, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

    public async Task<CursorResult<QuizListDto>> GetAllAsync(string? cursor, int? pageSize, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

    public async Task<QuizDetailDto> GetById(Guid id, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }

    public async Task UpdateAsync(QuizDetailDto quizDetailDto, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}
