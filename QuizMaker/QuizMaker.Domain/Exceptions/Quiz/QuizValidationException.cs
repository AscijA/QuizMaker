using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Domain.Exceptions.Quiz;
internal class QuizValidationException : DomainException {
    internal QuizValidationException(string? message) : base(message) { }
}
