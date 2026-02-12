using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Contracts.DTOs.Quiz;
using QuizMaker.Application.Interfaces.Services;

namespace QuizMaker.Api.Controllers;

/// <summary>
/// Manages Quiz resources. Allows creating, retrieving, updating, and deleting quizzes.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Authorize]
//[Authorize(Roles = "Admin")]
public class QuizzesController : ControllerBase {
    private readonly IQuizService _quizService;

    public QuizzesController(IQuizService quizService) {
        _quizService = quizService;
    }

    /// <summary>
    /// Retrieves a paginated list of quizzes using cursor-based pagination.
    /// </summary>
    /// <param name="cursor">
    /// The UTC date cursor from the previous page's response. 
    /// <br/>If <b>null</b>, returns the first page of results.
    /// </param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <param name="pageSize">
    /// The number of items to return per page. 
    /// <br/>Default is <b>10</b>. Max recommended is 50.
    /// </param>
    /// <returns>A cursor result containing a list of quizzes and the next cursor.</returns>
    /// <response code="200">Returns the list of quizzes.</response>
    [HttpGet]
    [ProducesResponseType(typeof(CursorResult<QuizListDto>), StatusCodes.Status200OK)]
    public async Task<CursorResult<QuizListDto>> GetAllAsync(
        [FromQuery] string? cursor,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 10) {
        return await _quizService.GetAllAsync(cursor, pageSize, cancellationToken);
    }

    /// <summary>
    /// Retrieves a specific quiz by its unique identifier.
    /// </summary>
    /// <param name="id">The GUID of the quiz to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The detailed quiz information including questions.</returns>
    /// <response code="200">Returns the requested quiz.</response>
    /// <response code="404">If the quiz with the specified ID was not found.</response>
    [HttpGet("{id}", Name = "GetQuizById")]
    [ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuizDetailDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken) {
        var result = await _quizService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new quiz.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/quizzes
    ///     {
    ///        "name": "Geography 101",
    ///        "questions": [
    ///           { "text": "Capital of France?", "answer": "Paris" }
    ///        ]
    ///     }
    /// 
    /// </remarks>
    /// <param name="dto">The quiz creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created quiz.</returns>
    /// <response code="201">Returns the created quiz.</response>
    /// <response code="400">If the input data is invalid (e.g., missing name).</response>
    [HttpPost]
    [ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<QuizDetailDto>> AddAsync([FromBody] QuizCreateDto dto, CancellationToken cancellationToken) {
        var result = await _quizService.AddAsync(dto, cancellationToken);
        return CreatedAtRoute("GetQuizById", new { id = result.Id }, result);
    }

    /// <summary>
    /// Fully updates an existing quiz.
    /// </summary>
    /// <remarks>
    /// <b>Warning:</b> This replaces the entire question list. Any questions not included in the payload will be removed from the quiz.
    /// </remarks>
    /// <param name="dto">The updated quiz data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated quiz details.</returns>
    /// <response code="200">If the update was successful.</response>
    /// <response code="404">If the quiz ID was not found.</response>
    /// <response code="400">If the input data is invalid (e.g., missing name).</response>
    [HttpPut]
    [ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<QuizDetailDto> Update([FromBody] QuizUpdateDto dto, CancellationToken cancellationToken) {
        return await _quizService.UpdateAsync(dto, cancellationToken);
    }

    /// <summary>
    /// Partially updates a quiz (e.g., rename only, or add questions without deleting old ones).
    /// </summary>
    /// <param name="dto">The partial update data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated quiz details.</returns>
    /// <response code="400">If the input data is invalid (e.g., missing name).</response>
    [HttpPatch]
    [ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<QuizDetailDto> UpdatePartial([FromBody] QuizUpdateDto dto, CancellationToken cancellationToken) {
        return await _quizService.UpdatePartialAsync(dto, cancellationToken);
    }

    /// <summary>
    /// Deletes a quiz by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the quiz to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Indicates the quiz was successfully deleted.</response>
    /// <response code="404">If the quiz was not found.</response>
    /// <response code="400">If the input data is invalid (e.g., missing name).</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken) {
        await _quizService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

}