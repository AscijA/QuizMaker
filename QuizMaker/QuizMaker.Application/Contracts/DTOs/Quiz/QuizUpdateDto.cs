using QuizMaker.Application.Contracts.DTOs.Question;

namespace QuizMaker.Application.Contracts.DTOs.Quiz;
public record QuizUpdateDto(Guid Id, string Name, IEnumerable<QuestionDetailDto> Questions);
