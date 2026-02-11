namespace QuizMaker.Application.Contracts.Results;
/// <summary>
/// Represents the result of an export operation.
/// </summary>
public record ExportResult(string FileName, string ContentType, byte[] Data);