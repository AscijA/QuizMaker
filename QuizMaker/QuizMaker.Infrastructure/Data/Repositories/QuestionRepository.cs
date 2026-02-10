using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Interfaces.Repositories;
using QuizMaker.Domain.Entities;

namespace QuizMaker.Infrastructure.Data.Repositories;
public class QuestionRepository : IQuestionRepository {

    private readonly QuizDbContext _context;
    public QuestionRepository(QuizDbContext context) {
        _context = context;
    }

    public async Task<IEnumerable<Question>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken) {

        return await _context.Questions
            .AsNoTracking()
            .Where(q => ids.Contains(q.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<CursorResult<Question>> SearchAsync(string? searchText, DateTime? cursor, int pageSize, CancellationToken cancellationToken) {
        var query = _context.Questions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchText)) {
            query = query.Where(q => EF.Functions.ILike(q.Text, $"%{searchText}%") ||
                EF.Functions.ILike(q.Answer, $"%{searchText}%"));
        }
        if (cursor.HasValue) {
            query = query.Where(a => a.CreatedAt < cursor.Value);
        }

        var items = await query
            .OrderByDescending(q => q.CreatedAt)
            .ThenBy(q => q.Id).Take(pageSize + 1)
            .ToListAsync(cancellationToken);

        var result = new CursorResult<Question>();

        if (items.Count > pageSize) {
            result.HasNextPage = true;
            items.RemoveAt(items.Count - 1);
        }

        result.Items = items;

        if (items.Count > 0) {
            result.NextCursor = items.Last().CreatedAt.ToString("o");
        }

        return result;
    }
}
