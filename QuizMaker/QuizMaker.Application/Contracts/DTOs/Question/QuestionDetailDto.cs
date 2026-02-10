using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Application.Contracts.DTOs.Question;
/// <summary>
/// Represents the details of a specific question.
/// Used for both retrieving data and binding questions during Quiz creation/updates.
/// </summary>
/// <param name="Id">
/// The unique identifier of the question. 
/// <br/>If <b>Empty</b> (Guid.Empty) during creation/update, it is treated as a <b>new</b> question.
/// <br/>If a valid GUID is provided, the system attempts to link an <b>existing</b> question.
/// </param>
/// <param name="Text">The actual text or prompt of the question.</param>
/// <param name="Answer">The correct answer for the question.</param>
public record QuestionDetailDto(Guid Id, string Text, string Answer);
