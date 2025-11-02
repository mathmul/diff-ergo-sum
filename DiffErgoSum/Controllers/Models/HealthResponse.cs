namespace DiffErgoSum.Controllers.Models;

public class HealthResponse
{
    public bool Ok { get; set; }

    public HealthResponse(bool ok = true)
    {
        Ok = ok;
    }
}
