namespace DiffErgoSum.Tests.IntegrationTests;

using DiffErgoSum.Infrastructure;

using Microsoft.EntityFrameworkCore;

using Xunit.Abstractions;


public class DiffRepositoryIntegrationTest
{
    private readonly ITestOutputHelper _output;

    public DiffRepositoryIntegrationTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task PostgresRepository_ShouldInsertAndRetrieveData()
    {
        var dbDriver = Environment.GetEnvironmentVariable("DB_DRIVER") ?? "inmemory";
        if (dbDriver != "postgres")
        {
            _output.WriteLine($"Skipping Postgres test — DB_DRIVER={dbDriver}");
            return;
        }

        // Build the connection string from environment variables
        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
        var db = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "differgosum_test";
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "differgosum_test";
        var pass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "secret";

        var connectionString = $"Host={host};Database={db};Username={user};Password={pass}";

        var options = new DbContextOptionsBuilder<DiffDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        // Recreate database for test isolation
        using (var setupCtx = new DiffDbContext(options))
        {
            setupCtx.Database.EnsureDeleted();
            setupCtx.Database.EnsureCreated();
        }

        // Arrange
        using var context = new DiffDbContext(options);
        var repo = new PostgresDiffRepository(context);

        const int diffId = 1;
        const string left = "SGVsbG8=";   // base64(Hello)
        const string right = "V29ybGQ=";  // base64(World)

        // Act
        await repo.SaveLeftAsync(diffId, left);
        await repo.SaveRightAsync(diffId, right);

        var result = await repo.GetAsync(diffId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(left, result?.Left);
        Assert.Equal(right, result?.Right);

        _output.WriteLine($"Postgres test passed for DB={db}");
    }
}
