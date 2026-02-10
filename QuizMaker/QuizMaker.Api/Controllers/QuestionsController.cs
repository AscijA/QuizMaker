using Microsoft.AspNetCore.Mvc;
using QuizMaker.Application.Interfaces.Services;


namespace QuizMaker.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class QuestionsController : ControllerBase {
    private readonly IQuestionService _questionService;
    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(IQuestionService questionService, ILogger<QuestionsController> logger) {
        _questionService = questionService;
        _logger = logger;
    }

    // GET: api/<QuestionesController>
    [HttpGet]
    public IEnumerable<string> Get() {
        return new string[] { "value1", "value2" };
    }

    // GET api/<QuestionesController>/5
    [HttpGet("{id}")]
    public string Get(int id) {
        return "value";
    }

    // POST api/<QuestionesController>
    [HttpPost]
    public void Post([FromBody] string value) {
    }

    // PUT api/<QuestionesController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value) {
    }

    // DELETE api/<QuestionesController>/5
    [HttpDelete("{id}")]
    public void Delete(int id) {
    }
}
