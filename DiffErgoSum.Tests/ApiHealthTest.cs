namespace DiffErgoSum.Tests;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;

using Xunit.Abstractions;

public class ApiHealthTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    /// <summary>
    /// For logging within tests on Failed (or always if verbose)
    /// </summary>
    private readonly ITestOutputHelper _output;

    public ApiHealthTest(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
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
        // Ensure DB_DRIVER is set in the test environment
        var dbDriver = Environment.GetEnvironmentVariable("DB_DRIVER") ?? "inmemory";
        _output.WriteLine($" - Config DB_DRIVER: {dbDriver}");

        // START TEST
        var url = "/api/health/env";
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        _output.WriteLine($" - Response JSON: {json}");

        Assert.NotNull(json.GetProperty("dbDriver").GetString());

        // Different assertions based on DB_DRIVER
        switch (dbDriver)
        {
            case "inmemory":
                Assert.Null(json.GetProperty("efProvider").GetString());
                Assert.Equal("inmemory", json.GetProperty("dbDriver").GetString());
                break;

            case "sqlite":
                Assert.NotNull(json.GetProperty("efProvider").GetString());
                Assert.Equal("sqlite", json.GetProperty("dbDriver").GetString());
                Assert.Equal("Microsoft.EntityFrameworkCore.Sqlite", json.GetProperty("efProvider").GetString());
                break;

            case "postgres":
                Assert.NotNull(json.GetProperty("efProvider").GetString());
                Assert.Equal("postgres", json.GetProperty("dbDriver").GetString());
                Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", json.GetProperty("efProvider").GetString());
                break;

            default:
                throw new Xunit.Sdk.XunitException($"Unsupported DB_DRIVER={dbDriver} in test environment");
        }
    }
}
