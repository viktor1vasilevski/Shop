namespace Shop.Domain.Interfaces;

public interface IEfUnitOfWork : IDisposable
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    void SaveChanges();
}
