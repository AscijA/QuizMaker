using QuizMaker.Application.Contracts.DTOs.Question;
using QuizMaker.Domain.Entities;

namespace QuizMaker.Application.Mappings;
public static class QuestionMappingExtension {

    public static QuestionDetailDto? ToDetailDto(this Question question) {

        if (question == null)
            return null;

        return new QuestionDetailDto(
            question.Id,
            question.Text,
            question.Answer
        );
    }

}
