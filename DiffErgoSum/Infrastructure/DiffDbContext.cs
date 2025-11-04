namespace DiffErgoSum.Infrastructure;

using DiffErgoSum.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;

public class DiffDbContext : DbContext
{
    public DiffDbContext(DbContextOptions<DiffDbContext> options)
        : base(options) { }

    public DbSet<DiffEntity> Diffs => Set<DiffEntity>();
}
