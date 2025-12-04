using Microsoft.EntityFrameworkCore;

namespace Shop.Infrastructure.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}
