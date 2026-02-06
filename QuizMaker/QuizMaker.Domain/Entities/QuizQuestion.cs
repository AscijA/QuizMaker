namespace QuizMaker.Domain.Entities;

/// <summary>
/// Association between <c>Quiz</c> and <c>Question</c>
/// Join Entity
/// </summary>
public class QuizQuestion {

    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public int OrderIndex { get; set; }
}
