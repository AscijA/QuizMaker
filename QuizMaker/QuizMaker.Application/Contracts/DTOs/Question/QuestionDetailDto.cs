using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Application.Contracts.DTOs.Question;
public record QuestionDetailDto (Guid Id, string Text, string Answer);
