
using QuizMaker.Domain.Entities;
using QuizMaker.Domain.Exceptions.Question;

namespace QuizMaker.UnitTests.Domain;
public class QuestionTests {


    [Fact]
    public void Constructor_ValidTextAnswer_CreatesInstance() {
        Question question = new Question("Test", "Test");

        Assert.NotNull(question);
        Assert.Equal("Test", question.Text);
        Assert.Equal("Test", question.Answer);
        Assert.NotEqual(DateTime.MinValue, question.CreatedAt);
        Assert.NotEqual(Guid.Empty, question.Id);
    }


    [Theory]
    [MemberData(nameof(MemberDataForTests.InvalidTitleAndMaxQuestions), MemberType = typeof(MemberDataForTests))]
    public void Constructor_InvalidTextAnswer_ThrowsQuestionValidationException(string? text, string? answer) {
        Assert.Throws<QuestionValidationException>(() => { new Question(text, answer); });
    }

}
