using QuizMaker.Application.Contracts.Results;

namespace QuizMaker.Application.Interfaces.Services;

public interface IExporterService {
    /// <summary>
    /// Returns a list of available export formats (e.g., ["pdf", "json"]).
    /// </summary>
    IEnumerable<string> GetSupportedFormats();

    /// <summary>
    /// Exports a specific quiz to the requested format.
    /// </summary>
    Task<ExportResult> ExportQuizAsync(Guid quizId, string format, CancellationToken cancellationToken);
}
