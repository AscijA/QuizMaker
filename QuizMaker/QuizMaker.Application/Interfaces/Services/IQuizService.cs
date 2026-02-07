using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Contracts.DTOs.Quiz;

namespace QuizMaker.Application.Interfaces.Services;
/// <summary>
/// Provides methods for managing quizzes, including retrieval, creation, updating, and deletion.
/// </summary>
public interface IQuizService {

    /// <summary>
    /// Retrieves a pagination list of <c>QuizListDto</c> entities using cursor-based pagination
    /// </summary>
    /// <param name="cursor">CreatedAt UTC DateTime-String of the last item in the previous page. Pass <c>null</c> for the first page.</param>
    /// <param name="pageSize">Number of items to return</param>
    /// <returns>A cursor result containing the <c>QuizListDto</c> entities and the next cursor.</returns>
    Task<CursorResult<QuizListDto>> GetAllAsync(string? cursor, int? pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves a <c>Quiz</c> entity as <c>QuizDetailDto</c> by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the <c>Quiz</c> entity to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <c>QuizDetailDto</c> if found; otherwise, null.</returns>
    Task<QuizDetailDto> GetById(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously adds a <c>Quiz</c> entity to the data store.
    /// </summary>
    /// <param name="quizCreateDto">The <c>QuizCreateDto</c> used to add the <c>Quiz</c> entity.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <c>ID</c> of the created <c>Quiz</c></returns>
    Task<Guid> AddAsync(QuizCreateDto quizCreateDto, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously updates the specified <c>Quiz</c>.
    /// </summary>
    /// <param name="quizDetailDto">The <c>QuizCreateDto</c> used to update the <c>Quiz</c> entity.</param>
    /// <returns></returns>
    Task UpdateAsync(QuizDetailDto quizDetailDto, CancellationToken cancellationToken);


    /// <summary>
    /// Asynchronously deletes the <c>Quiz</c> entity with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the <c>Quiz</c> entity to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

}
