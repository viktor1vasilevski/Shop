using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Models;
using Shop.Domain.Models.Base;
using Shop.Infrastructure.Configurations;

namespace Shop.Infrastructure.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor _httpContextAccessor) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var username = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

        var addedEntries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableBaseEntity && e.State == EntityState.Added);

        foreach (var entry in addedEntries)
        {
            var entity = (AuditableBaseEntity)entry.Entity;
            entity.Created = DateTime.UtcNow;
            entity.CreatedBy = username;
        }

        var modifiedEntries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableBaseEntity && e.State == EntityState.Modified);

        foreach (var entry in modifiedEntries)
        {
            var entity = (AuditableBaseEntity)entry.Entity;

            entry.Property(nameof(entity.Created)).IsModified = false;
            entry.Property(nameof(entity.CreatedBy)).IsModified = false;

            entity.LastModified = DateTime.UtcNow;
            entity.LastModifiedBy = username;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
