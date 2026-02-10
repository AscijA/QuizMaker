using QuizMaker.Application.Contracts.DTOs.Question;
using QuizMaker.Application.Contracts.DTOs.Quiz;
using QuizMaker.Domain.Entities;

namespace QuizMaker.Application.Mappings;
public static class QuizMappingExtension {

    public static QuizDetailDto? ToDetailDto(this Quiz? quiz) {

        if (quiz == null)
            return null;

        var questionDetailDtos = quiz.QuizQuestions
            .OrderBy(qq => qq.OrderIndex)
            .Select(qq => qq.Question.ToDetailDto())
            .Where(dto => dto != null)
            .Cast<QuestionDetailDto>()
            .ToList();

        return new QuizDetailDto(quiz.Id, quiz.Name, questionDetailDtos);
    }

    public static QuizListDto? ToListDto(this Quiz? quiz) {

        if (quiz == null)
            return null;

        return new QuizListDto(
            quiz.Id,
            quiz.Name,
            quiz.QuizQuestions?.Count ?? 0
            );
    }
}
