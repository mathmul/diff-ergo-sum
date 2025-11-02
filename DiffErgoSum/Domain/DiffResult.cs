namespace DiffErgoSum.Domain;

public class DiffResult
{
    public DiffType Type { get; }
    public List<DiffSegment> Diffs { get; }

    public DiffResult(DiffType type, List<DiffSegment>? diffs = null)
    {
        Type = type;
        Diffs = diffs ?? new List<DiffSegment>();
    }
}
