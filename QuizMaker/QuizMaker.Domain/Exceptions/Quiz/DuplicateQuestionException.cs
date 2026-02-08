namespace QuizMaker.Domain.Exceptions.Quiz;
internal class DuplicateQuestionException: DomainException {

    internal DuplicateQuestionException(Guid quizId, Guid questionId) :
        base($"Quiz {quizId} already contains question {questionId}") { }
}
