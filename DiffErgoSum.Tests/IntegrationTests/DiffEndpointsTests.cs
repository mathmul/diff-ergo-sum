namespace DiffErgoSum.Tests.IntegrationTests;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using DiffErgoSum.Core.Repositories;
using DiffErgoSum.Infrastructure.Repositories;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

public class DiffEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DiffEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        {
            // Replace singleton repository with a new instance per test
            services.AddSingleton<IDiffRepository, InMemoryDiffRepository>();
        })).CreateClient();
    }

    [Fact]
    public async Task PutLeft_ShouldReturn201Created()
    {
        var id = 101;
        var payload = new { data = "AAAAAA==" };
        var response = await _client.PutAsJsonAsync($"/api/v1/diff/{id}/left", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PutLeft_WithNullData_ShouldReturn400BadRequest()
    {
        var id = 102;
        var payload = new { data = (string?)null };
        var response = await _client.PutAsJsonAsync($"/api/v1/diff/{id}/left", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutLeft_WithInvalidCharacters_ShouldReturn422UnprocessableEntity()
    {
        var id = 103;
        var payload = new { data = "@@@===" }; // Illegal Base64 characters
        var response = await _client.PutAsJsonAsync($"/api/v1/diff/{id}/left", payload);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task PutLeft_WithInvalidBase64_ShouldReturn422UnprocessableEntity()
    {
        var id = 104;
        var payload = new { data = "Zm9vYmFy===" }; // Valid regex, invalid Base64 padding
        var response = await _client.PutAsJsonAsync($"/api/v1/diff/{id}/left", payload);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(422, json.GetProperty("status").GetInt32());
        Assert.Equal("Invalid Base64 Input", json.GetProperty("title").GetString());
        Assert.Equal("Value is not valid Base64.", json.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task PutRight_ShouldReturn201Created()
    {
        var id = 201;
        var payload = new { data = "AAAAAA==" };
        var response = await _client.PutAsJsonAsync($"/api/v1/diff/{id}/right", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PutRight_WithNullData_ShouldReturn400BadRequest()
    {
        var id = 202;
        var payload = new { data = (string?)null };
        var response = await _client.PutAsJsonAsync($"/api/v1/diff/{id}/right", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutRight_WithInvalidCharacters_ShouldReturn422UnprocessableEntity()
    {
        var id = 203;
        var payload = new { data = "@@@===" }; // Illegal Base64 characters
        var response = await _client.PutAsJsonAsync($"/api/v1/diff/{id}/right", payload);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task PutRight_WithInvalidBase64_ShouldReturn422UnprocessableEntity()
    {
        var id = 204;
        var payload = new { data = "Zm9vYmFy===" }; // Valid regex, invalid Base64 padding
        var response = await _client.PutAsJsonAsync($"/api/v1/diff/{id}/right", payload);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(422, json.GetProperty("status").GetInt32());
        Assert.Equal("Invalid Base64 Input", json.GetProperty("title").GetString());
        Assert.Equal("Value is not valid Base64.", json.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task GetDiff_WhenBothSidesEqual_ShouldReturn200Equals()
    {
        var id = 301;
        var payload = new { data = "AAAAAA==" };
        await _client.PutAsJsonAsync($"/api/v1/diff/{id}/left", payload);
        await _client.PutAsJsonAsync($"/api/v1/diff/{id}/right", payload);
        var response = await _client.GetAsync($"/api/v1/diff/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal("Equals", json.GetProperty("diffResultType").GetString());
    }

    [Fact]
    public async Task GetDiff_WhenNoData_ShouldReturn404NotFound()
    {
        var id = 302;
        var response = await _client.GetAsync($"/api/v1/diff/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDiff_WhenOnlyLeftData_ShouldReturn404NotFound()
    {
        var id = 303;
        var payload = new { data = "AAAAAA==" };
        await _client.PutAsJsonAsync($"/api/v1/diff/{id}/left", payload);
        var response = await _client.GetAsync($"/api/v1/diff/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDiff_WhenOnlyRightData_ShouldReturn404NotFound()
    {
        var id = 304;
        var payload = new { data = "AAAAAA==" };
        await _client.PutAsJsonAsync($"/api/v1/diff/{id}/right", payload);
        var response = await _client.GetAsync($"/api/v1/diff/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDiff_WhenSizesDiffer_ShouldReturn200SizeDoNotMatch()
    {
        var id = 305;
        await _client.PutAsJsonAsync($"/api/v1/diff/{id}/left", new { data = "AAAAAA==" });
        await _client.PutAsJsonAsync($"/api/v1/diff/{id}/right", new { data = "AAA=" });
        var response = await _client.GetAsync($"/api/v1/diff/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal("SizeDoNotMatch", json.GetProperty("diffResultType").GetString());
    }

    [Fact]
    public async Task GetDiff_WhenContentDiffers_ShouldReturn200DiffOffsets()
    {
        var id = 306;
        await _client.PutAsJsonAsync($"/api/v1/diff/{id}/left", new { data = "AAAAAA==" });
        await _client.PutAsJsonAsync($"/api/v1/diff/{id}/right", new { data = "AQABAQ==" });
        var response = await _client.GetAsync($"/api/v1/diff/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal("ContentDoNotMatch", json.GetProperty("diffResultType").GetString());

        var diffs = json.GetProperty("diffs").EnumerateArray().ToList();

        Assert.NotEmpty(diffs);
        Assert.All(diffs, d => Assert.True(d.TryGetProperty("offset", out _)));
        Assert.All(diffs, d => Assert.True(d.TryGetProperty("length", out _)));
    }
}
