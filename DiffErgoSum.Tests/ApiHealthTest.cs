namespace DiffErgoSum.Tests;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;

public class ApiHealthTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiHealthTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealthEndpoint_ShouldReturnOkTrue()
    {
        var url = "/api/health";
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.True(json.GetProperty("ok").GetBoolean());
    }

    // TODO: remove as it is only for debugging purposes
    [Fact]
    public async Task GetHealthEnv_ShouldReturnDbDriverFromEnv()
    {
        var url = "/api/health/env";
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.NotNull(json.GetProperty("dbDriver").GetString());
        Assert.NotNull(json.GetProperty("efProvider").GetString());
        Assert.Equal("sqlite", json.GetProperty("dbDriver").GetString());
        Assert.Equal("Microsoft.EntityFrameworkCore.Sqlite", json.GetProperty("efProvider").GetString());
    }
}
