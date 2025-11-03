namespace DiffErgoSum.Application;

using System.Collections.Generic;

using DiffErgoSum.Domain;

public class DiffService : IDiffService
{
    public DiffResult Compare(byte[] left, byte[] right)
    {
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
