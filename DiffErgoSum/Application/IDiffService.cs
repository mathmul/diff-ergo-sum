namespace DiffErgoSum.Application;

using DiffErgoSum.Domain;

/// <summary>
/// Defines the contract for comparing two decoded byte arrays and
/// returning a diff result.
/// </summary>
public interface IDiffService
{
    public Task UploadAsync(int id, string side, string base64);
    public Task<DiffResult> CompareAsync(int id);
}
