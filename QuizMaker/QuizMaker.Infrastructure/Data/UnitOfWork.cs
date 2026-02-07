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

    public UnitOfWork(ILogger<UnitOfWork> logger) {
        _logger = logger;
    }
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) {
        throw new NotImplementedException();
    }
}
