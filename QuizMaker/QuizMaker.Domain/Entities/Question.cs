namespace QuizMaker.Domain.Entities;
/// <summary>
/// Represents a reusable Question
/// </summary>
public class Question {
    public Guid Id { get; private set; }
    public string Text { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;

    public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();

    public Question() {
        Id = Guid.NewGuid();
    }
}
