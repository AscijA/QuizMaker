using System.ComponentModel.Composition;
using System.Text;
using QuizMaker.Application.Contracts.DTOs.Quiz;
using QuizMaker.Application.Interfaces;

namespace QuizMaker.Infrastructure.Exporters;

[Export(typeof(IQuizExporter))]
public class CsvExporter : IQuizExporter {
    public string Format { get; set; } = "csv";
    public string ContentType { get; set; } = "text/csv";

    public Task<byte[]> ExportAsync(QuizDetailDto quiz) {
        var sb = new StringBuilder();

        sb.AppendLine($"Quiz Name: {EscapeCsv(quiz.Name)}");
        sb.AppendLine("Order,Question Text");

        if (quiz.Questions != null) {
            int order = 1;
            foreach (var question in quiz.Questions) {
                var text = EscapeCsv(question.Text);

                sb.AppendLine($"{order++},{text}");
            }
        }

        var preamble = Encoding.UTF8.GetPreamble();
        var body = Encoding.UTF8.GetBytes(sb.ToString());

        var combined = new byte[preamble.Length + body.Length];
        Array.Copy(preamble, combined, preamble.Length);
        Array.Copy(body, 0, combined, preamble.Length, body.Length);

        return Task.FromResult(combined);
    }

    /// <summary>
    /// Escapes fields containing commas, quotes, or newlines by wrapping them in quotes
    /// and doubling existing quotes (standard CSV rule).
    /// </summary>
    private string EscapeCsv(string? field) {
        if (string.IsNullOrEmpty(field))
            return "";

        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r')) {
            field = field.Replace("\"", "\"\"");

            return $"\"{field}\"";
        }

        return field;
    }
}