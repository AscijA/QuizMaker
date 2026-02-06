
using QuizMaker.Domain.Entities;
using QuizMaker.Domain.Common;

namespace QuizMaker.Domain.Interfaces;

/// <summary>
/// Defines methods for managing Quiz entities in a data store,
/// including retrieval, addition, update, and deletion operations.
/// </summary>
internal interface IQuizRepository {

    /// <summary>
    /// Retrieves a pagination list of <c>Quiz</c> entities using cursor-based pagination
    /// </summary>
    /// <param name="cursor">CreatedAt UTC DateTime of the last item in the previous page. Pass <c>null</c> for the first page.</param>
    /// <param name="pageSize">Number of items to return</param>
    /// <returns>A cursor result containing the items and the next cursor.</returns>
    Task<CursorResult<Quiz>> GetAllAsync(DateTime? cursor, int pageSize);

    /// <summary>
    /// Asynchronously retrieves a Quiz entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the Quiz entity to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Quiz if found; otherwise, null.</returns>
    Task<Quiz?> GetByIdAsync(Guid id);

    /// <summary>
    /// Asynchronously adds a Quiz entity to the data store.
    /// </summary>
    /// <param name="quiz">The Quiz entity to add.</param>
    /// <returns>A task that represents the asynchronous add operation.</returns>
    Task AddAsync(Quiz quiz);

    /// <summary>
    /// Asynchronously updates the specified quiz.
    /// </summary>
    /// <param name="quiz">The Quiz entity to update.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateAsync(Quiz quiz);

    /// <summary>
    /// Asynchronously deletes the Quiz entity with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(Guid id);
}
