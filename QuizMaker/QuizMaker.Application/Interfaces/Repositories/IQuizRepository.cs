
using QuizMaker.Domain.Entities;
using QuizMaker.Application.Common.Results;

namespace QuizMaker.Application.Interfaces.Repositories;

/// <summary>
/// Defines methods for managing Quiz entities in a data store,
/// including retrieval, addition, update, and deletion operations.
/// </summary>
public interface IQuizRepository {

    /// <summary>
    /// Retrieves a pagination list of <c>Quiz</c> entities using cursor-based pagination
    /// </summary>
    /// <param name="cursor">CreatedAt UTC DateTime of the last item in the previous page. Pass <c>null</c> for the first page.</param>
    /// <param name="pageSize">Number of items to return</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A cursor result containing the <c>Quiz</c> entites and the next cursor.</returns>
    Task<CursorResult<Quiz>> GetAllAsync(DateTime? cursor, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a <c>Quiz</c> entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the Quiz entity to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <c>Quiz</c> entity if found; otherwise, null.</returns>
    Task<Quiz?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously adds a <c>Quiz</c> entity to the data store.
    /// </summary>
    /// <param name="quiz">The <c>Quiz</c> entity to add.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous add operation.</returns>
    Task AddAsync(Quiz quiz, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously updates the specified <c>Quiz</c>.
    /// </summary>
    /// <param name="quiz">The Quiz entity to update.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateAsync(Quiz quiz, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously deletes the <c>Quiz</c> entity with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the <c>Quiz</c> entity to delete.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
