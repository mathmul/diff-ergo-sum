namespace DiffErgoSum.Infrastructure;

using DiffErgoSum.Domain;
using DiffErgoSum.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;

public class PostgresDiffRepository : IDiffRepository
{
    private readonly DiffDbContext _context;

    public PostgresDiffRepository(DiffDbContext context)
    {
        _context = context;

        // _context.Database.EnsureCreated();
        try
        {
            _context.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect to PostgreSQL: {ex.Message}");
        }
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
        return entity is null ? null : (entity.Left, entity.Right);
    }
}
