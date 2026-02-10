using Microsoft.AspNetCore.Authentication.Negotiate;
using QuizMaker.Application;
using QuizMaker.Infrastructure;
using Serilog;

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
                });

                builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                    .AddNegotiate();

                builder.Services.AddApplication();
                builder.Services.AddInfrastructure(builder.Configuration);

                //builder.Services.AddAuthorization(options => {
                //    options.FallbackPolicy = options.DefaultPolicy;
                //});

                var app = builder.Build();

                app.UseSerilogRequestLogging(options => {
                    options.EnrichDiagnosticContext = (diag, http) => {
                        diag.Set("TraceId", http.TraceIdentifier);
                        diag.Set("RemoteIP", http.Connection.RemoteIpAddress?.ToString());
                        diag.Set("User", http.User?.Identity?.Name);
                    };

                    //options.GetLevel = (http, elapsed, ex) =>
                    //    http.Request.Path.StartsWithSegments("/health") ? LogEventLevel.Verbose : LogEventLevel.Information;
                });

                if (app.Environment.IsDevelopment()) {
                    app.UseSwagger();
                    app.UseSwaggerUI(options => options.EnableTryItOutByDefault());
                }

                app.UseHttpsRedirection();

                app.UseAuthentication();
                //app.UseAuthorization();

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
