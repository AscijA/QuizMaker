using QuizMaker.Application.Contracts.DTOs.Question;

namespace QuizMaker.Application.Contracts.DTOs.Quiz;
public record QuizDetailDto(Guid Id, string Name, IEnumerable<QuestionDetailDto> Questions);
