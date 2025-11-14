namespace DiffErgoSum.Application;

using System.Buffers.Text;
using System.Collections.Generic;

using DiffErgoSum.Api.Exceptions;
using DiffErgoSum.Core;

public class DiffService : IDiffService
{
    private readonly IDiffRepository _repo;

    public DiffService(IDiffRepository repo)
    {
        _repo = repo;
    }

    public async Task UploadAsync(int id, string side, string base64)
    {
        if (!Base64.IsValid(base64))
            throw new InvalidBase64HttpException();

        if (side == "left")
            await _repo.SaveLeftAsync(id, base64);
        else
            await _repo.SaveRightAsync(id, base64);
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
