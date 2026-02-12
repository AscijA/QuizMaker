using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using QuizMaker.Infrastructure.Data;

namespace QuizMaker.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class {
    protected override void ConfigureWebHost(IWebHostBuilder builder) {
        builder.ConfigureAppConfiguration((context, config) => {
            var settings = new Dictionary<string, string>
            {
        {"ConnectionStrings:Default", "DataSource=:memory:"},
        {"Authentication:ApiKey", "TestSecretKey123"},
    };
            config.AddInMemoryCollection(settings!);
        });

        builder.ConfigureServices(services => {
            try {
                services.Remove(
                    services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<QuizDbContext>))
                );
            } catch (Exception ex) {
                Console.WriteLine($"Error removing existing DbContext configuration: {ex.Message}");
            }

            services.AddDbContext<QuizDbContext>(options => {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
        });
    }
}