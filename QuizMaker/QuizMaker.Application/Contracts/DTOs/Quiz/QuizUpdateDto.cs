using QuizMaker.Application.Contracts.DTOs.Question;

namespace QuizMaker.Application.Contracts.DTOs.Quiz;
/// <summary>
/// Data required to update an existing Quiz.
/// </summary>
/// <param name="Id">The unique identifier of the quiz to update.</param>
/// <param name="Name">The new name for the quiz.</param>
/// <param name="Questions">
/// The <b>new</b> list of questions for the quiz. 
/// <br/><b>Note:</b> This usually replaces the entire existing collection of questions for this quiz.
/// </param>
public record QuizUpdateDto(Guid Id, string Name, IEnumerable<QuestionDetailDto> Questions);
