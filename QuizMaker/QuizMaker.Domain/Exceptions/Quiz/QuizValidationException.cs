namespace QuizMaker.Domain.Exceptions.Quiz;
public class QuizValidationException : DomainException {
    internal QuizValidationException(string? message) : base(message) { }
}
