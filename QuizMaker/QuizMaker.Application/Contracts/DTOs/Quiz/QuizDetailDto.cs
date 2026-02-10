using QuizMaker.Application.Contracts.DTOs.Question;

namespace QuizMaker.Application.Contracts.DTOs.Quiz;
/// <summary>
/// Represents the full details of a Quiz, including all associated questions.
/// </summary>
/// <param name="Id">The unique identifier of the quiz.</param>
/// <param name="Name">The display name of the quiz.</param>
/// <param name="Questions">The collection of questions currently assigned to this quiz.</param>
public record QuizDetailDto(Guid Id, string Name, IEnumerable<QuestionDetailDto> Questions);
