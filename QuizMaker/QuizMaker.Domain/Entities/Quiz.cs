
namespace QuizMaker.Domain.Entities;
/// <summary>
/// Represents a Quiz 
/// Aggregate Root.
/// </summary>
public class Quiz {
    public Guid Id { get; private set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Quiz() {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
