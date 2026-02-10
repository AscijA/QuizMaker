using QuizMaker.Application.Contracts.DTOs.Question;

namespace QuizMaker.Application.Contracts.DTOs.Quiz;
/// <summary>
/// Data required to create a new Quiz.
/// </summary>
/// <param name="Name">The display name or title of the quiz.</param>
/// <param name="Questions">
/// An optional list of questions to associate with the quiz immediately upon creation.
/// Can contain a mix of new questions (Empty ID) and existing questions (Valid ID).
/// </param>
public record QuizCreateDto(string Name, IEnumerable<QuestionDetailDto> Questions);
