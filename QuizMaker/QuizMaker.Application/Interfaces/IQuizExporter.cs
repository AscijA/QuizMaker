using QuizMaker.Application.Contracts.DTOs.Quiz;

namespace QuizMaker.Application.Interfaces;

/// <summary>
/// Contract for dynamic quiz exporters loaded via MEF.
/// </summary>
public interface IQuizExporter {
    /// <summary>
    /// The short code for the format (e.g., "pdf", "json").
    /// This is what the API client will request.
    /// </summary>
    string Format { get; }

    /// <summary>
    /// The MIME type for the HTTP response (e.g., "application/pdf").
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// Generates the file content for the given quiz.
    /// </summary>
    Task<byte[]> ExportAsync(QuizDetailDto quiz);
}