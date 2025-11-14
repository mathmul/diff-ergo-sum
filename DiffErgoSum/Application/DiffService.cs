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

        // Case 1: Size mismatch
        if (left.Length != right.Length)
            return new DiffResult(DiffType.SizeDoNotMatch);

        // Case 2: Equal content
        if (left.AsSpan().SequenceEqual(right))
            return new DiffResult(DiffType.Equals);

        // Case 3: Same size but content differs
        var diffs = FindDiffs(left, right);
        return new DiffResult(DiffType.ContentDoNotMatch, diffs);
    }

    private static List<DiffSegment> FindDiffs(byte[] left, byte[] right)
    {
        var diffs = new List<DiffSegment>();
        int offset = -1;
        int length = 0;

        for (int i = 0; i < left.Length; i++)
        {
            if (left[i] != right[i])
            {
                if (offset == -1)
                    offset = i;
                length++;
            }
            else if (offset != -1)
            {
                diffs.Add(new DiffSegment(offset, length));
                offset = -1;
                length = 0;
            }
        }

        if (offset != -1)
            diffs.Add(new DiffSegment(offset, length));

        return diffs;
    }
}
