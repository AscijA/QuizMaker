using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Contracts.DTOs.Question;

namespace QuizMaker.Application.Interfaces.Services;

/// <summary>
/// Provides operations for searching and retrieving question details.
/// </summary>
public interface IQuestionService {

    /// <summary>
    /// Retrieves a cursor-based paginated list of <c>QuestionDetailDto</c> entities
    /// </summary>
    /// <param name="searchText">Text to search for. Pass null for all questions.</param>
    /// <param name="cursor">Text string-cursor of the last item in the previous page. Pass <c>null</c> for the first page.</param>
    /// <param name="pageSize">Number of items to return</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A cursor result containing the <c>QuestionDetailDto</c> entites and the next cursor.</returns>
    Task<CursorResult<QuestionDetailDto>> SearchAsync(string? searchText, string? cursor, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a collection of <c>QuestionDetailDto</c> entites matching the specified IDs.
    /// </summary>
    /// <param name="ids">A collection of <c>Question</c> entity IDs to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation, containing the collection of matching <c>QuestionDetailDto</c> entities.</returns>
    Task<IEnumerable<QuestionDetailDto>> GetByIDsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken );
}
