using System.Linq.Expressions;

namespace Shop.Domain.Interfaces;

public interface IEfRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>>? predicate = null, bool asNoTracking = true, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? predicate = null, bool asNoTracking = true, CancellationToken cancellationToken = default);
}
