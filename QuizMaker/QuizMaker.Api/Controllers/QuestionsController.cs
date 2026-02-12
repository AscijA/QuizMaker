using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Contracts.DTOs.Question;
using QuizMaker.Application.Interfaces.Services;

namespace QuizMaker.Api.Controllers;

/// <summary>
/// Manages Question resources independently of Quizzes.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize]
//[Authorize(Roles = "Admin")]
public class QuestionsController : ControllerBase {
    private readonly IQuestionService _questionService;
    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(IQuestionService questionService, ILogger<QuestionsController> logger) {
        _questionService = questionService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a batch of questions by their IDs.
    /// </summary>
    /// <param name="ids">A list of GUIDs to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of question details.</returns>
    /// <response code="200">Returns the matching questions.</response>
    [HttpGet("batch")]
    [ProducesResponseType(typeof(IEnumerable<QuestionDetailDto>), StatusCodes.Status200OK)]
    public async Task<IEnumerable<QuestionDetailDto>> GetByIdsAsync(
        [FromQuery] IEnumerable<Guid> ids,
        CancellationToken cancellationToken) {
        return await _questionService.GetByIDsAsync(ids, cancellationToken);
    }

    /// <summary>
    /// Searches for questions containing specific text.
    /// </summary>
    /// <param name="searchText">The text to search for within question text or answers.</param>
    /// <param name="cursor">The UTC cursor from the previous page.</param>
    /// <param name="pageSize">Number of results per page (Default: 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of matching questions.</returns>
    /// <response code="200">Returns the search results.</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(CursorResult<QuestionDetailDto>), StatusCodes.Status200OK)]
    public async Task<CursorResult<QuestionDetailDto>> SearchAsync(
        [FromQuery] string? searchText,
        [FromQuery] string? cursor,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default) {
        return await _questionService.SearchAsync(searchText, cursor, pageSize, cancellationToken);
    }

    /// <summary>
    /// Get a batch of questions ordered by creation date, starting from a given cursor.
    /// </summary>
    /// <param name="cursor">The UTC cursor from the previous page.</param>
    /// <param name="pageSize">Number of results per page (Default: 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of questions.</returns>
    /// <response code="200">Returns the search results.</response>
    [HttpGet]
    [ProducesResponseType(typeof(CursorResult<QuestionDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CursorResult<QuestionDetailDto>), StatusCodes.Status200OK)]
    public async Task<CursorResult<QuestionDetailDto>> GetAllAsync(
        [FromQuery] string? cursor,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default) {
        return await _questionService.GetAllAsync(cursor, pageSize, cancellationToken);
    }
}