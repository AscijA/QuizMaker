using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizMaker.Application.Interfaces.Repositories;
using QuizMaker.Application.Interfaces.UnitOfWork;
using QuizMaker.Infrastructure.Data;
using QuizMaker.Infrastructure.Data.Repositories;
using QuizMaker.Infrastructure.Security;
using QuizMaker.Infrastructure.Settings;

namespace QuizMaker.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config) {

        services.AddOptions<DatabaseSettings>()
            .Bind(config.GetSection("ConnectionStrings"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        //services.AddOptions<JwtSettings>()
        //    .Bind(config.GetSection(JwtSettings.SectionName))
        //    .ValidateDataAnnotations()
        //    .ValidateOnStart();

        //var jwtSettings = config.GetSection(JwtSettings.SectionName).Get<JwtSettings>();

        services.AddDbContext<QuizDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("Default")));

        //services.AddIdentityCore<IdentityUser>()
        //    .AddRoles<IdentityRole>()
        //    .AddEntityFrameworkStores<QuizDbContext>();

        services.AddAuthentication("ApiKey")
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", null);

        //if (jwtSettings != null) {
        //    services.AddAuthentication(options => {
        //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    })
        //    .AddJwtBearer(options => {
        //        options.TokenValidationParameters = new TokenValidationParameters {
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ValidateIssuerSigningKey = true,
        //            ValidIssuer = jwtSettings.Issuer,
        //            ValidAudience = jwtSettings.Audience,
        //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        //        };
        //    });
        //}

        services.AddAuthorization(options => {
            options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("ApiKey")
                .RequireAuthenticatedUser()
                .Build();
        });

        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();


        return services;
    }
}