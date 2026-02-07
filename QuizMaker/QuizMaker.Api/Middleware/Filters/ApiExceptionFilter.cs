using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuizMaker.Application.Exceptions;
using QuizMaker.Domain.Exceptions;

namespace QuizMaker.Api.Middleware.Filters;

public class ApiExceptionFilter {

    public void OnException(ExceptionContext exceptionContext) {

        if (exceptionContext.Exception is NotFoundException) {
            exceptionContext.Result = new NotFoundObjectResult(new { error = exceptionContext.Exception.Message });
            exceptionContext.ExceptionHandled = true;
        }
        else if (exceptionContext.Exception is DomainException) {
            exceptionContext.Result = new BadRequestObjectResult(new { error = exceptionContext.Exception.Message });
            exceptionContext.ExceptionHandled = true;
        }
    }
}
