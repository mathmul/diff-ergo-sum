namespace DiffErgoSum.Application;

using DiffErgoSum.Domain;

/// <summary>
/// Defines the contract for comparing two decoded byte arrays and
/// returning a diff result.
/// </summary>
public interface IDiffService
{
    /// <summary>
    /// Compares two byte arrays and returns a <see cref="DiffResult"/>
    /// describing their relationship (equal, size mismatch, or differing segments).
    /// </summary>
    DiffResult Compare(byte[] left, byte[] right);
}
