namespace QuizMaker.Application.Interfaces.UnitOfWork;
public interface IUnitOfWork : IDisposable {

    /// <summary>
    /// Commits Changes to the DataStore
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous Save operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
