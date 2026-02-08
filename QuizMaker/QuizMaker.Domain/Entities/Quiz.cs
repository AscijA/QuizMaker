
using QuizMaker.Domain.Exceptions.Quiz;

namespace QuizMaker.Domain.Entities;
/// <summary>
/// Represents a Quiz 
/// Aggregate Root.
/// </summary>
public class Quiz {
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
    //public bool IsDeleted { get; private set; } // soft delete
    public DateTime CreatedAt { get; private set; }

    public Quiz() { }
    public Quiz(string name) {

        if (string.IsNullOrWhiteSpace(name))
            throw new QuizValidationException("Quiz Name is required.");

        Name = name;
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
