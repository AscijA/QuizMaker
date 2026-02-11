using Microsoft.Extensions.Logging;
using QuizMaker.Application.Contracts.Results;
using QuizMaker.Application.Interfaces;
using QuizMaker.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuizMaker.Application.Services;

public class ExporterService : IExporterService, IDisposable {
    private readonly IQuizService _quizService;
    private readonly ILogger<ExporterService> _logger;
    private CompositionContainer _container;

    [ImportMany(typeof(IQuizExporter))]
    public IEnumerable<IQuizExporter> Exporters { get; set; }

    public ExporterService(IQuizService quizService, ILogger<ExporterService> logger) {
        _quizService = quizService;
        _logger = logger;
        InitializeExporters();
    }

    /// <summary>
    /// Initializes the MEF composition container by loading exporter assemblies from the Exporters directory and the
    /// current application domain, and composes the current instance.
    /// </summary>
    private void InitializeExporters() {
        try {
            var aggregateCatalog = new AggregateCatalog();
            var exportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exporters");

            if (Directory.Exists(exportPath)) {
                aggregateCatalog.Catalogs.Add(new DirectoryCatalog(exportPath));
            }

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName != null && a.FullName.StartsWith("QuizMaker"))
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                .Where(a => !a.Location.StartsWith(exportPath, StringComparison.OrdinalIgnoreCase));

            foreach (var assembly in loadedAssemblies) {
                aggregateCatalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }

            _container = new CompositionContainer(aggregateCatalog);
            _container.ComposeParts(this);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to initialize MEF exporters.");
            Exporters = Enumerable.Empty<IQuizExporter>();
        }
    }
    public IEnumerable<string> GetSupportedFormats() {
        if (Exporters == null)
            return [];
        return Exporters.Select(e => e.Format).Distinct();
    }

    public async Task<ExportResult> ExportQuizAsync(Guid quizId, string format, CancellationToken cancellationToken) {
        if (string.IsNullOrEmpty(format)) {
            var formats = GetSupportedFormats();
            throw new ArgumentException($"Format is required. Available formats: {string.Join(", ", formats)}");
        }

        var exporter = Exporters?.FirstOrDefault(e =>
            string.Equals(e.Format, format, StringComparison.OrdinalIgnoreCase));

        if (exporter == null) {
            throw new ArgumentException($"Export format '{format}' is not supported or no exporter plugin found.");
        }

        var quizDto = await _quizService.GetByIdAsync(quizId, cancellationToken);

        var fileBytes = await exporter.ExportAsync(quizDto);

        var sanitizedName = string.Join("_", quizDto.Name.Split(Path.GetInvalidFileNameChars()));
        var fileName = $"{sanitizedName}_{DateTime.UtcNow:ddMMyyyy}.{format.ToLower()}";

        return new ExportResult(fileName, exporter.ContentType, fileBytes);
    }

    public void Dispose() {
        _container?.Dispose();
    }
}