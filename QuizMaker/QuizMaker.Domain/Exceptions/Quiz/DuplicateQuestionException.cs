using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Domain.Exceptions.Quiz;
internal class DuplicateQuestionException: DomainException {

    internal DuplicateQuestionException(Guid quizId, Guid questionId) :
        base($"Quiz {quizId} already contains question {questionId}") { }
}
