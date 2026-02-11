using QuizMaker.Domain.Exceptions.Question;

namespace QuizMaker.Domain.Entities;
/// <summary>
/// Represents a reusable Question
/// </summary>
public class Question {
    public Guid Id { get; private set; }
    public string Text { get; set; } = null!;
    public string Answer { get; set; } = null!;
    public DateTime CreatedAt { get; private set; }
    private readonly List<QuizQuestion> _quizQuestions = new();

    public IReadOnlyCollection<QuizQuestion> QuizQuestions => _quizQuestions.AsReadOnly();
    private Question() { }
    public Question(string text, string answer) {

        if (string.IsNullOrWhiteSpace(text)) {
            throw new QuestionValidationException("Question text is required.");
        }
        if (string.IsNullOrWhiteSpace(answer)) {
            throw new QuestionValidationException("Question answer is required.");
        }
        Text = text;
        Answer = answer;
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
