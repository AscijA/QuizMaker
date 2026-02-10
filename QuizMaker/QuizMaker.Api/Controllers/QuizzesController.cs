using Microsoft.AspNetCore.Mvc;
using QuizMaker.Application.Common.Results;
using QuizMaker.Application.Contracts.DTOs.Quiz;
using QuizMaker.Application.Interfaces.Services;


namespace QuizMaker.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class QuizzesController : ControllerBase {

    private readonly IQuizService _quizService;

    private readonly ILogger<QuizzesController> _logger;

    public QuizzesController(IQuizService quizService, ILogger<QuizzesController> logger) {
        _quizService = quizService;
        _logger = logger;
    }

    // GET: api/<QuizzesController>
    [HttpGet]
    public async Task<CursorResult<QuizListDto>> GetAllAsync(string? cursor, CancellationToken cancellationToken, int pageSize = 10) {

        return await _quizService.GetAllAsync(cursor, pageSize, cancellationToken);
    }

    // GET api/<QuizzesController>/5
    [HttpGet("{id}")]
    public async Task<QuizDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken) {
        return await _quizService.GetByIdAsync(id, cancellationToken);
    }

    // POST api/<QuizzesController>
    [HttpPost]
    public async Task<QuizDetailDto> AddAsync([FromBody] QuizCreateDto dto, CancellationToken cancellationToken) {
        return await _quizService.AddAsync(dto, cancellationToken);
    }

    // PUT api/<QuizzesController>/5
    [HttpPut]
    public async Task<QuizDetailDto> Update([FromBody] QuizUpdateDto dto, CancellationToken cancellationToken) {
        return await _quizService.UpdateAsync(dto, cancellationToken);
    }

    [HttpPatch]
    public async Task<QuizDetailDto> UpdatePartial([FromBody] QuizUpdateDto dto, CancellationToken cancellationToken) {
        return await _quizService.UpdatePartialAsync(dto, cancellationToken);
    }

    // DELETE api/<QuizzesController>/5
    [HttpDelete("{id}")]
    public async void Delete(Guid id, CancellationToken cancellationToken) {
        await _quizService.DeleteAsync(id, cancellationToken);
    }

    [HttpGet]
    [Route("/export")]
    public async Task<byte[]> Export(Guid id) {
        return null;
    }
}
