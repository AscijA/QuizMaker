
using QuizMaker.Domain.Common;
using QuizMaker.Domain.Entities;

namespace QuizMaker.Domain.Interfaces;

/// <summary>
/// Defines methods for querying and retrieving question entities from a data source.
/// </summary>
internal interface IQuestionRepository {

    /// <summary>
    /// Retrieves a cursor-based paginated list of Question entities
    /// </summary>
    /// <param name="searchText">Text to search for. Pass null for all questions.</param>
    /// <param name="cursor">Text string-cursor of the last item in the previous page. Pass <c>null</c> for the first page.</param>
    /// <param name="pageSize">Number of items to return</param>
    /// <returns>A cursor result containing the items and the next cursor.</returns>
    Task<CursorResult<Question>> SearchAsync(string? searchText, string? cursor, int pageSize);

    /// <summary>
    /// Asynchronously retrieves a collection of Questions entites matching the specified IDs.
    /// </summary>
    /// <param name="ids">A collection of Question entity IDs to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, containing the collection of matching questions.</returns>
    Task<IEnumerable<Question>> GetByIdsAsync(IEnumerable<Guid> ids);
}
