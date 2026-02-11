using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizMaker.Application.Interfaces.Repositories;
using QuizMaker.Application.Interfaces.UnitOfWork;
using QuizMaker.Infrastructure.Data;
using QuizMaker.Infrastructure.Data.Repositories;

namespace QuizMaker.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config) {

        services.AddOptions<DatabaseSettings>()
            .Bind(config.GetSection("ConnectionStrings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<QuizDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
