using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using QuizMaker.Api.Extensions;
using QuizMaker.Api.Middleware.Filters;
using QuizMaker.Application;
using QuizMaker.Infrastructure;
using Serilog;
using Serilog.Events;

namespace QuizMaker.Api {
    public class Program {
        public static void Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            try {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((context, services, loggerConfig) => loggerConfig
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                );

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();

                builder.Services.AddSwaggerGen(c => {
                    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);

                    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme {
                        Description = "Enter your API Key in the text input below.",
                        Type = SecuritySchemeType.ApiKey,
                        Name = "X-Api-Key",
                        In = ParameterLocation.Header,
                        Scheme = "ApiKey"
                    });

                    c.AddSecurityRequirement(
                        new OpenApiSecurityRequirement{
                                {new OpenApiSecurityScheme
                                    {
                                        Reference = new OpenApiReference
                                        {
                                            Type = ReferenceType.SecurityScheme,
                                            Id = "ApiKey"
                                        },
                                        In = ParameterLocation.Header
                                    },
                                    new List<string>()
                            }
                            });
                    //                c.AddSecurityDefinition(
                    //                    "Bearer",
                    //                    new OpenApiSecurityScheme {
                    //                        Name = "Authorization",
                    //                        Type = SecuritySchemeType.Http,
                    //                        Scheme = "Bearer",
                    //                        BearerFormat = "JWT",
                    //                        In = ParameterLocation.Header,
                    //                        Description = "Enter your valid token in the text input below.\n\nExample: `eyJhbGciOi...`"
                    //                    });

                    //                c.AddSecurityRequirement(
                    //                    new OpenApiSecurityRequirement {
                    //                       {
                    //                           new OpenApiSecurityScheme {
                    //                               Reference = new OpenApiReference {
                    //                                   Type = ReferenceType.SecurityScheme,
                    //                                   Id = "Bearer"
                    //                               }
                    //                           },
                    //                           Array.Empty<string>()
                    //                       }
                    //});
                });

                builder.Services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy())
                    .AddNpgSql(
                        connectionString: builder.Configuration.GetConnectionString("Default")!,
                        name: "postgresql",
                        tags: new[] { "db", "data" });

                //builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                //    .AddNegotiate();

                builder.Services.AddApplication();
                builder.Services.AddInfrastructure(builder.Configuration);

                builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
                builder.Services.AddProblemDetails();

                var app = builder.Build();

                app.ApplyMigrations();
                app.UseExceptionHandler();

                if (!app.Environment.IsDevelopment()) {
                    app.UseHsts();
                }

                app.UseHttpsRedirection();

                app.UseSerilogRequestLogging(options => {
                    options.EnrichDiagnosticContext = (diag, http) => {
                        diag.Set("TraceId", http.TraceIdentifier);
                        diag.Set("RemoteIP", http.Connection.RemoteIpAddress?.ToString());
                        diag.Set("User", http.User?.Identity?.Name);
                    };

                    options.GetLevel = (http, elapsed, ex) =>
                        http.Request.Path.StartsWithSegments("/health") ? LogEventLevel.Verbose : LogEventLevel.Information;
                });

                if (app.Environment.IsDevelopment()) {
                    app.UseSwagger();
                    app.UseSwaggerUI(options => options.EnableTryItOutByDefault());
                }
                app.MapHealthChecks("/health");

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();
                app.Run();
            }
            catch (Exception ex) {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally {
                Log.CloseAndFlush();
            }
        }
    }
}
