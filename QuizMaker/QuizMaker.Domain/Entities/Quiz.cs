using QuizMaker.Domain.Exceptions.Quiz;

namespace QuizMaker.Domain.Entities;

public class Quiz {
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private readonly List<QuizQuestion> _quizQuestions = new();
    public virtual IReadOnlyCollection<QuizQuestion> QuizQuestions => _quizQuestions.AsReadOnly();

    public bool IsDeleted { get; private set; }
    private Quiz() { }

    public Quiz(string name) {
        if (string.IsNullOrWhiteSpace(name))
            throw new QuizValidationException("Quiz Name is required.");

        Name = name;
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string name) {
        if (string.IsNullOrWhiteSpace(name))
            throw new QuizValidationException("Quiz Name is required.");
        Name = name;
    }

    public void Delete() {
        IsDeleted = true;
    }


    public void AddQuestion(Question question, int orderIndex) {
        if (_quizQuestions.Any(qq => qq.QuestionId == question.Id)) {
            return;
        }
        var link = new QuizQuestion(this, question, orderIndex);
        _quizQuestions.Add(link);
    }

    public void RemoveQuestion(Guid questionId) {
        var link = _quizQuestions.FirstOrDefault(qq => qq.QuestionId == questionId);
        if (link != null) {
            _quizQuestions.Remove(link);
        }
    }
    public void ClearQuestions() {
        _quizQuestions.Clear();
    }

    public void ReorderQuestions() {
        var ordered = _quizQuestions.OrderBy(x => x.OrderIndex).ToList();
        for (int i = 0; i < ordered.Count; i++) {
            ordered[i].UpdateOrder(i);
        }
    }
}