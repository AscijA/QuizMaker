using QuizMaker.Application.Common.Results;
using QuizMaker.Domain.Entities;

namespace QuizMaker.Application.Interfaces.Repositories;

/// <summary>
/// Defines methods for querying and retrieving question entities from a data source.
/// </summary>
public interface IQuestionRepository {

    /// <summary>
    /// Retrieves a cursor-based paginated list of <c>Question</c> entities
    /// </summary>
    /// <param name="searchText">Text to search for. Pass null for all questions.</param>
    /// <param name="cursor">Text string-cursor of the last item in the previous page. Pass <c>null</c> for the first page.</param>
    /// <param name="pageSize">Number of items to return</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A cursor result containing the <c>Question</c> entites and the next cursor.</returns>
    Task<CursorResult<Question>> SearchAsync(string? searchText, string? cursor, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a collection of <c>Question</c> entites matching the specified IDs.
    /// </summary>
    /// <param name="ids">A collection of <c>Question</c> entity IDs to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the collection of matching <c>Question</c> entities.</returns>
    Task<IEnumerable<Question>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
}
