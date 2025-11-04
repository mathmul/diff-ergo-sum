namespace DiffErgoSum.Infrastructure;

using DiffErgoSum.Domain;
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

    public void SaveLeft(int id, string base64Data)
    {
        var entity = _context.Diffs.Find(id);
        if (entity == null)
        {
            entity = new DiffEntity { Id = id, Left = base64Data };
            _context.Diffs.Add(entity);
        }
        else
        {
            entity.Left = base64Data;
            _context.Diffs.Update(entity);
        }

        _context.SaveChanges();
    }

    public void SaveRight(int id, string base64Data)
    {
        var entity = _context.Diffs.Find(id);
        if (entity == null)
        {
            entity = new DiffEntity { Id = id, Right = base64Data };
            _context.Diffs.Add(entity);
        }
        else
        {
            entity.Right = base64Data;
            _context.Diffs.Update(entity);
        }

        _context.SaveChanges();
    }

    public (string? Left, string? Right)? Get(int id)
    {
        var entity = _context.Diffs.AsNoTracking().FirstOrDefault(e => e.Id == id);
        if (entity is null)
            return null;

        return (entity.Left, entity.Right);
    }
}
