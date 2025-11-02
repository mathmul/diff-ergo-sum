namespace DiffErgoSum.Domain;

public class DiffSegment
{
    public int Offset { get; }
    public int Length { get; }

    public DiffSegment(int offset, int length)
    {
        Offset = offset;
        Length = length;
    }
}
