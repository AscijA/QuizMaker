namespace QuizMaker.Application.Contracts.DTOs.Quiz;

/// <summary>
/// A lightweight view of a Quiz, intended for list/summary views.
/// Does not include the full question text to save bandwidth.
/// </summary>
/// <param name="Id">The unique identifier of the quiz.</param>
/// <param name="Name">The display name of the quiz.</param>
/// <param name="Count">The total number of questions contained in this quiz.</param>
public record QuizListDto(Guid Id, string Name, int Count);
