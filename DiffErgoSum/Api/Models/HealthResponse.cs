namespace DiffErgoSum.Api.Models;

/// <summary>
/// Represents a simple API health check response.
/// </summary>
/// <remarks>
/// Returned by <c>GET /api/health</c> to indicate the service is alive.
/// </remarks>
public record HealthResponse(bool Ok = true);
