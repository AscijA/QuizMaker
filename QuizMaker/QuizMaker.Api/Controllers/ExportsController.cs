using Microsoft.AspNetCore.Mvc;
using QuizMaker.Application.Interfaces.Services;
using System.Net.Mime;

namespace QuizMaker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportsController : ControllerBase {
    private readonly IExporterService _exporterService;

    public ExportsController(IExporterService exporterService) {
        _exporterService = exporterService;
    }

    /// <summary>
    /// Exports a quiz to a specific file format (PDF, CSV, JSON).
    /// </summary>
    /// <param name="id">The unique identifier of the quiz to export.</param>
    /// <param name="format">The target format (e.g., 'pdf', 'csv', 'json'). Case-insensitive.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A downloadable file containing the quiz data.</returns>
    /// <response code="200">Returns the file content.</response>
    /// <response code="400">If the requested format is not supported.</response>
    /// <response code="404">If the quiz with the specified ID is not found.</response>
    [HttpGet("{id}/export")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportQuiz(Guid id, [FromQuery] string format, CancellationToken cancellationToken) {
        var result = await _exporterService.ExportQuizAsync(id, format, cancellationToken);
        return File(result.Data, result.ContentType, result.FileName);
    }

    /// <summary>
    /// Retrieves a list of supported export formats.
    /// </summary>
    /// <returns>A list of strings representing supported extensions (e.g., ["pdf", "csv"]).</returns>
    /// <response code="200">Returns the list of formats.</response>
    [HttpGet("export-formats")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public IActionResult GetExportFormats() {
        return Ok(_exporterService.GetSupportedFormats());
    }
}