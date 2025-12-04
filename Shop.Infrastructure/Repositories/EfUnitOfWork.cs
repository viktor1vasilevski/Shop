using Shop.Domain.Interfaces;
using Shop.Infrastructure.Context;

namespace Shop.Infrastructure.Repositories;

public class EfUnitOfWork(AppDbContext _context) : IEfUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) => await _context.SaveChangesAsync(cancellationToken);
    public void SaveChanges() => _context.SaveChanges();
    public void Dispose() => _context.Dispose();
}
