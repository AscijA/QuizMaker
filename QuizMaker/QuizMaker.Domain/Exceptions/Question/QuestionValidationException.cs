namespace QuizMaker.Domain.Exceptions.Question;
public class QuestionValidationException : DomainException {

    public QuestionValidationException(string? message) : base(message) { }
}
