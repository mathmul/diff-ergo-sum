namespace DiffErgoSum.Domain;

public interface IDiffRepository
{
    void SaveLeft(int id, string base64Data);
    void SaveRight(int id, string base64Data);
    (string? Left, string? Right)? Get(int id);
}
