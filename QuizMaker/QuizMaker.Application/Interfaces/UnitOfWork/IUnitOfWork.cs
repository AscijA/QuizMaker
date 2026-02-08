namespace QuizMaker.Application.Interfaces.UnitOfWork;
public interface IUnitOfWork : IDisposable {

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
