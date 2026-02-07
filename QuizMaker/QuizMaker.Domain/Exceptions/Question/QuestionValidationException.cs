using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Domain.Exceptions.Question;
internal class QuestionValidationException : DomainException {

    internal QuestionValidationException(string? message) : base(message) { }
}
