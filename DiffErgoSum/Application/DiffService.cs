namespace DiffErgoSum.Application;

using DiffErgoSum.Api.Exceptions;
using DiffErgoSum.Core.Algorithms;
using DiffErgoSum.Core.Enums;
using DiffErgoSum.Core.Models;
using DiffErgoSum.Core.Repositories;

public class DiffService : IDiffService
{
    private readonly IDiffRepository _repo;

    public DiffService(IDiffRepository repo)
    {
        _repo = repo;
    }

    public Task UploadAsync(int id, DiffPart part, string base64)
    {
        return part switch
        {
            DiffPart.Left => _repo.SaveLeftAsync(id, base64),
            DiffPart.Right => _repo.SaveRightAsync(id, base64),
            _ => throw new ArgumentOutOfRangeException(nameof(part))
        };
    }

    public async Task<DiffResult> CompareAsync(int id)
    {
        var pair = await _repo.GetAsync(id);
        if (pair is null || string.IsNullOrEmpty(pair.Value.Left) || string.IsNullOrEmpty(pair.Value.Right))
            throw new DiffNotFoundHttpException(id);

        var left = Convert.FromBase64String(pair.Value.Left);
        var right = Convert.FromBase64String(pair.Value.Right);

        return DiffAlgorithm.Compare(left, right);
    }
}
