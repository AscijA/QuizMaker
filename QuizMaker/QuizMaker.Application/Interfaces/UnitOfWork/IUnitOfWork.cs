namespace QuizMaker.Application.Interfaces.UnitOfWork;
public interface IUnitOfWork {

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
