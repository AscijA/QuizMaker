using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Interfaces.Repositories;
using QuizMaker.Domain.Entities;

namespace QuizMaker.Infrastructure.Data.Repositories;
public class QuizRepository : IQuizRepository {
    private readonly QuizDbContext _context;

    public QuizRepository(QuizDbContext context) {
        _context = context;
    }

    public async Task<Quiz> AddAsync(Quiz quiz, CancellationToken cancellationToken) {
        var entity = await _context.Quizzes.AddAsync(quiz, cancellationToken);
        return entity.Entity;
    }

    public void Delete(Quiz quiz) {
        _context.Quizzes.Remove(quiz);
    }

    public async Task<CursorResult<Quiz>> GetAllAsync(DateTime? cursor, int pageSize, CancellationToken cancellationToken) {
        IQueryable<Quiz> query = _context.Quizzes
            .AsNoTracking()
            .Include(q => q.QuizQuestions);


        if (cursor.HasValue) {
            query = query.Where(a => a.CreatedAt < cursor.Value);
        }

        var items = await query
            .OrderByDescending(q => q.CreatedAt)
            .ThenBy(q => q.Id)
            .Take(pageSize + 1)
            .ToListAsync(cancellationToken);

        CursorResult<Quiz> result = new CursorResult<Quiz>();

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

    public async Task<Quiz?> GetByIdAsync(Guid id, CancellationToken cancellationToken) {
        return await _context.Quizzes.FindAsync(new object?[] { id }, cancellationToken: cancellationToken);

    }

    public async Task<Quiz?> GetFullByIdAsync(Guid id, CancellationToken cancellationToken, bool shouldTrack) {
        IQueryable<Quiz> query = _context.Quizzes;

        if (!shouldTrack) {
            query = query.AsNoTracking();
        }
        return await query
            .Where(q => q.Id == id)
            .Include(q => q.QuizQuestions)
            .ThenInclude(qq => qq.Question)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
