using Microsoft.EntityFrameworkCore;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.Context;
using System.Linq.Expressions;

namespace Shop.Infrastructure.Repositories;

public class EfRepository<TEntity>(AppDbContext _context) : IEfRepository<TEntity> where TEntity : class
{
    protected readonly DbSet<TEntity> _dbSet = _context.Set<TEntity>();

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public void Delete(TEntity entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
            _dbSet.Attach(entity);
        _dbSet.Remove(entity);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? predicate = null, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        if (asNoTracking)
            query = query.AsNoTracking();

        return predicate == null
            ? await query.AnyAsync(cancellationToken)
            : await query.AnyAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>>? predicate = null, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        if (asNoTracking)
            query = query.AsNoTracking();

        if (predicate != null)
            query = query.Where(predicate);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;
        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, bool asNoTracking = true, CancellationToken cancellationToken = default)
        => asNoTracking
            ? await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken)
            : await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    public void Update(TEntity entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
            _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }
}
