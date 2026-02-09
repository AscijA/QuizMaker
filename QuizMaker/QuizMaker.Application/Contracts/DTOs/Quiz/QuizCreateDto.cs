using QuizMaker.Application.Contracts.DTOs.Question;

namespace QuizMaker.Application.Contracts.DTOs.Quiz;
public record QuizCreateDto(string Name, IEnumerable<QuestionDetailDto> Questions);
