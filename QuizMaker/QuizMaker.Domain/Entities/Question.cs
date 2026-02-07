using QuizMaker.Domain.Exceptions.Question;

namespace QuizMaker.Domain.Entities;
/// <summary>
/// Represents a reusable Question
/// </summary>
public class Question {
    public Guid Id { get; private set; }
    public string Text { get; set; }
    public string Answer { get; set; }

    public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();

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
    }
}
