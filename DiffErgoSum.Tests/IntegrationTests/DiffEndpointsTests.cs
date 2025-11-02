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
    public async Task GetDiff_WhenNoData_ShouldReturn404NotFound()
    {
        var id = 1;
        var response = await _client.GetAsync($"/api/v1/diff/{id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutLeft_ShouldReturn201Created()
    {
        var id = 2;
        var json = JsonContent.Create(new { data = "AAAAAA==" });
        var response = await _client.PutAsync($"/api/v1/diff/{id}/left", json);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetDiff_WhenOnlyLeftData_ShouldReturn404NotFound()
    {
        var id = 3;
        var json = JsonContent.Create(new { data = "AAAAAA==" });
        await _client.PutAsync($"/api/v1/diff/{id}/left", json);

        var response = await _client.GetAsync($"/api/v1/diff/{id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PutRight_ShouldReturn201Created()
    {
        var id = 4;
        var json = JsonContent.Create(new { data = "AAAAAA==" });
        var response = await _client.PutAsync($"/api/v1/diff/{id}/right", json);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetDiff_WhenBothSidesEqual_ShouldReturn200Equals()
    {
        var id = 10;
        var payload = JsonContent.Create(new { data = "AAAAAA==" });
        await _client.PutAsync($"/api/v1/diff/{id}/left", payload);
        await _client.PutAsync($"/api/v1/diff/{id}/right", payload);

        var response = await _client.GetAsync($"/api/v1/diff/{id}");
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Equals", json.GetProperty("diffResultType").GetString());
    }

    [Fact]
    public async Task GetDiff_WhenSizesDiffer_ShouldReturn200SizeDoNotMatch()
    {
        var id = 11;
        await _client.PutAsync($"/api/v1/diff/{id}/left", JsonContent.Create(new { data = "AAAAAA==" }));
        await _client.PutAsync($"/api/v1/diff/{id}/right", JsonContent.Create(new { data = "AAA=" }));

        var response = await _client.GetAsync($"/api/v1/diff/{id}");
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("SizeDoNotMatch", json.GetProperty("diffResultType").GetString());
    }

    [Fact]
    public async Task GetDiff_WhenContentDiffers_ShouldReturn200DiffOffsets()
    {
        var id = 12;
        await _client.PutAsync($"/api/v1/diff/{id}/left", JsonContent.Create(new { data = "AAAAAA==" }));
        await _client.PutAsync($"/api/v1/diff/{id}/right", JsonContent.Create(new { data = "AQABAQ==" }));

        var response = await _client.GetAsync($"/api/v1/diff/{id}");
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("ContentDoNotMatch", json.GetProperty("diffResultType").GetString());

        var diffs = json.GetProperty("diffs").EnumerateArray().ToList();
        Assert.NotEmpty(diffs);
        Assert.All(diffs, d => Assert.True(d.TryGetProperty("offset", out _)));
        Assert.All(diffs, d => Assert.True(d.TryGetProperty("length", out _)));
    }

    [Fact]
    public async Task PutLeft_WithNullData_ShouldReturn400BadRequest()
    {
        var id = 20;
        var json = JsonContent.Create(new { data = (string?)null });
        var response = await _client.PutAsync($"/api/v1/diff/{id}/left", json);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutRight_WithInvalidBase64_ShouldReturn422UnprocessableEntity()
    {
        var id = 21;
        var json = JsonContent.Create(new { data = "not-base64!!" });
        var response = await _client.PutAsync($"/api/v1/diff/{id}/right", json);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("InvalidBase64", body.GetProperty("error").GetString());
    }
}
