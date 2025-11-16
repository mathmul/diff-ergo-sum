namespace DiffErgoSum.Infrastructure;

using DiffErgoSum.Core.Repositories;
using DiffErgoSum.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;

public class SqliteDiffRepository : IDiffRepository
{
    private readonly DiffDbContext _context;

    public SqliteDiffRepository(DiffDbContext context)
    {
        _context = context;
        _context.Database.EnsureCreated();
    }

    public async Task SaveLeftAsync(int id, string base64Data)
    {
        var entity = await _context.Diffs.FindAsync(id);
        if (entity is null)
        {
            entity = new DiffEntity { Id = id, Left = base64Data };
            await _context.Diffs.AddAsync(entity);
        }
        else
        {
            entity.Left = base64Data;
        }

        await _context.SaveChangesAsync();
    }

    public async Task SaveRightAsync(int id, string base64Data)
    {
        var entity = await _context.Diffs.FindAsync(id);
        if (entity is null)
        {
            entity = new DiffEntity { Id = id, Right = base64Data };
            await _context.Diffs.AddAsync(entity);
        }
        else
        {
            entity.Right = base64Data;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<(string? Left, string? Right)?> GetAsync(int id)
    {
        var entity = await _context.Diffs.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return entity is null ? null : (entity.Left, entity.Right);
    }
}
