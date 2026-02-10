using QuizMaker.Domain.Entities;
using QuizMaker.Domain.Exceptions.Quiz;

namespace QuizMaker.UnitTests.Domain;
public class QuizTests {

    public QuizTests() { }

    [Fact]
    public void Constructor_ValidName_CreatesInstance() {
        Quiz quiz = new Quiz("Test");

        Assert.NotNull(quiz);
        Assert.Equal("Test", quiz.Name);
        Assert.NotEqual(Guid.Empty, quiz.Id);
        Assert.NotEqual(DateTime.MinValue, quiz.CreatedAt);

    }


    [Theory]
    [MemberData(nameof(MemberDataForTests.InvalidStrings), MemberType = typeof(MemberDataForTests))]
    public void Constructor_InvalidName_ThrowsQuizValidationException(string? name) {

        Assert.Throws<QuizValidationException>(() => { new Quiz(name); });

    }

    [Fact]
    public void UpdateName_ValidName_RenamesInstance() {
        var quiz = new Quiz("Test");
        quiz.UpdateName("NewName");
        Assert.Equal("NewName", quiz.Name);
    }

    [Theory]
    [MemberData(nameof(MemberDataForTests.InvalidStrings), MemberType = typeof(MemberDataForTests))]
    public void UpdateName_InvalidName_ThrowsQuizValidationException(string? name) {
        var quiz = new Quiz("Test");
        Assert.Throws<QuizValidationException>(() => { quiz.UpdateName(name); });
    }

    [Fact]
    public void AddQuestion_ShouldAddQuestion_WhenItDoesNotExist() {
        var quiz = new Quiz("Test Quiz");
        var question = new Question("Q1", "A1");

        // Act
        quiz.AddQuestion(question, 0);

        // Assert
        Assert.Single(quiz.QuizQuestions);
        Assert.Contains(quiz.QuizQuestions, qq => qq.QuestionId == question.Id);
    }

    [Fact]
    public void AddQuestion_ShouldNotAddDuplicate_WhenQuestionAlreadyExists() {
        var quiz = new Quiz("Test Quiz");
        var question = new Question("Q1", "A1");

        quiz.AddQuestion(question, 0);
        quiz.AddQuestion(question, 1);

        Assert.Single(quiz.QuizQuestions); 
    }

    [Fact]
    public void RemoveQuestion_ShouldRemoveLink_WhenQuestionExists() {
        var quiz = new Quiz("Test Quiz");
        var question = new Question("Q1", "A1");
        quiz.AddQuestion(question, 0);

        quiz.RemoveQuestion(question.Id);

        Assert.Empty(quiz.QuizQuestions);
    }

    [Fact]
    public void RemoveQuestion_ShouldDoNothing_WhenQuestionIdDoesNotExist() {
        var quiz = new Quiz("Test Quiz");
        var question = new Question("Q1", "A1");
        quiz.AddQuestion(question, 0);

        quiz.RemoveQuestion(Guid.NewGuid());

        Assert.Single(quiz.QuizQuestions); 
    }

    [Fact]
    public void ReorderQuestions_ShouldNormalizeIndexes_ToSequentialZeroBased() {
        // Arrange
        var quiz = new Quiz("Test Quiz");
        var q1 = new Question("Q1", "A");
        var q2 = new Question("Q2", "A");
        var q3 = new Question("Q3", "A");

        quiz.AddQuestion(q1, 10);  
        quiz.AddQuestion(q2, 5);   
        quiz.AddQuestion(q3, 100); 

        quiz.ReorderQuestions();

        var result = quiz.QuizQuestions.OrderBy(x => x.OrderIndex).ToList();

        Assert.Equal(3, result.Count);

        Assert.Equal(q2.Id, result[0].QuestionId);
        Assert.Equal(0, result[0].OrderIndex);

        Assert.Equal(q1.Id, result[1].QuestionId);
        Assert.Equal(1, result[1].OrderIndex);

        Assert.Equal(q3.Id, result[2].QuestionId);
        Assert.Equal(2, result[2].OrderIndex);
    }

}
