namespace DiffErgoSum.Infrastructure;

using System.Collections.Concurrent;

using DiffErgoSum.Domain;

public class InMemoryDiffRepository : IDiffRepository
{
    private readonly ConcurrentDictionary<int, (string? Left, string? Right)> _store = new();

    public void SaveLeft(int id, string base64Data)
    {
        _store.AddOrUpdate(
            id,
            _ => (base64Data, null),
            (_, existing) => (base64Data, existing.Right)
        );
    }

    public void SaveRight(int id, string base64Data)
    {
        _store.AddOrUpdate(
            id,
            _ => (null, base64Data),
            (_, existing) => (existing.Left, base64Data)
        );
    }

    public (string? Left, string? Right)? Get(int id)
    {
        return _store.TryGetValue(id, out var pair) ? pair : null;
    }
}
