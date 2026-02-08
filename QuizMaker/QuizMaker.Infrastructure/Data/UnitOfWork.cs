using Microsoft.Extensions.Logging;
using QuizMaker.Application.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizMaker.Infrastructure.Data;
public class UnitOfWork : IUnitOfWork {
    private readonly ILogger<UnitOfWork> _logger;

    private readonly QuizDbContext _context;

    public UnitOfWork(QuizDbContext context, ILogger<UnitOfWork> logger) {
        _context = context;
        _logger = logger;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) {
        var numberOfChanges = await _context.SaveChangesAsync(cancellationToken);
        _logger.LogDebug("UnitOfWork commited {Count} of  changes", numberOfChanges);
    }

    public void Dispose() {
        _context.Dispose();
    }
}
