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
    public string Get(int id) {
        return "value";
    }

    // POST api/<QuizzesController>
    [HttpPost]
    public void Post([FromBody] string value) {
    }

    // PUT api/<QuizzesController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value) {
    }

    // DELETE api/<QuizzesController>/5
    [HttpDelete("{id}")]
    public void Delete(int id) {
    }

    [HttpGet]
    public async Task<byte[]> Export(Guid id) {
        return null;
    }
}
