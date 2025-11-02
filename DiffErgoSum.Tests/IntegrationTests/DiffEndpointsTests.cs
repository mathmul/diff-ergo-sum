using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;

namespace DiffErgoSum.Tests.IntegrationTests;

public class DiffEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public DiffEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        Controllers.DiffController.ResetRepository();
    }

    [Fact]
    public async Task GetDiff_WhenNoData_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync("/api/v1/diff/1");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutLeft_ShouldReturnCreated()
    {
        var json = JsonContent.Create(new { data = "AAAAAA==" });
        var response = await _client.PutAsync("/api/v1/diff/1/left", json);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PutRight_ShouldReturnCreated()
    {
        var json = JsonContent.Create(new { data = "AAAAAA==" });
        var response = await _client.PutAsync("/api/v1/diff/1/right", json);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
