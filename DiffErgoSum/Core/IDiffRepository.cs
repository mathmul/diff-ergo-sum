namespace DiffErgoSum.Core;

public interface IDiffRepository
{
    public Task SaveLeftAsync(int id, string base64Data);
    public Task SaveRightAsync(int id, string base64Data);
    public Task<(string? Left, string? Right)?> GetAsync(int id);
}
