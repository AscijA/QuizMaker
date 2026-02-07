using Microsoft.Extensions.DependencyInjection;
using QuizMaker.Application.Interfaces.Services;
using QuizMaker.Application.Services;

namespace QuizMaker.Application;

public static class DependencyInjection {
    public static IServiceCollection AddApplication(this IServiceCollection services) {
        services.AddScoped<IQuizService, QuizService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IExporterService, ExporterService>();

        return services;
    }
}
