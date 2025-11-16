namespace DiffErgoSum.Infrastructure.Repositories;

using System.Collections.Concurrent;

using DiffErgoSum.Core.Repositories;

public class InMemoryDiffRepository : IDiffRepository
{
    private readonly ConcurrentDictionary<int, (string? Left, string? Right)> _store = new();

    public Task SaveLeftAsync(int id, string base64Data)
    {
        _store.AddOrUpdate(
            id,
            _ => (base64Data, null),
            (_, existing) => (base64Data, existing.Right)
        );
        return Task.CompletedTask;
    }

    public Task SaveRightAsync(int id, string base64Data)
    {
        _store.AddOrUpdate(
            id,
            _ => (null, base64Data),
            (_, existing) => (existing.Left, base64Data)
        );
        return Task.CompletedTask;
    }

    public Task<(string? Left, string? Right)?> GetAsync(int id)
    {
        var ok = _store.TryGetValue(id, out var pair);
        return Task.FromResult<(string?, string?)?>(ok ? pair : null);
    }
}
