namespace DiffErgoSum.Application;

using DiffErgoSum.Core;

/// <summary>
/// Defines the contract for comparing two decoded byte arrays and
/// returning a diff result.
/// </summary>
public interface IDiffService
{
    public Task UploadAsync(int id, DiffPart part, string base64);
    public Task<DiffResult> CompareAsync(int id);
}
