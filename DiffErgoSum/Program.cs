using System.Reflection;

using DiffErgoSum.Api.Filters;
using DiffErgoSum.Application;
using DiffErgoSum.Core.Enums;
using DiffErgoSum.Core.Repositories;
using DiffErgoSum.Infrastructure;
using DiffErgoSum.Middleware;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var dbDriver = builder.Configuration["DB_DRIVER"] ?? "inmemory";

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), includeControllerXmlComments: true);

    c.SupportNonNullableReferenceTypes();

    c.SwaggerDoc("v1", new()
    {
        Title = "Diff Ergo Sum API",
        Version = "v1",
        Description = "A diffing service that compares two base64-encoded inputs and reports offsets and lengths of differing segments.",
        Contact = new()
        {
            Name = "@mathmul",
            Url = new Uri("https://github.com/mathmul/diff-ergo-sum")
        }
    });

    c.MapType<DiffPart>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = [.. Enum.GetNames(typeof(DiffPart)).Select(n => new OpenApiString(n.ToLower()))]
    });
});
builder.Services.AddTransient<IDiffService, DiffService>();
var supported = new[] { "inmemory", "sqlite", "postgres" };
if (!supported.Contains(dbDriver.ToLowerInvariant()))
    throw new InvalidOperationException($"Unsupported DB_DRIVER={dbDriver}");
switch (dbDriver.ToLowerInvariant())
{
    case "sqlite":
        builder.Services.AddDbContext<DiffDbContext>(options =>
            options.UseSqlite("Data Source=diffs.db"));
        builder.Services.AddScoped<IDiffRepository, SqliteDiffRepository>();
        break;
    case "postgres":
        builder.Services.AddDbContext<DiffDbContext>(options =>
            options.UseNpgsql(
                $"Host={builder.Configuration["POSTGRES_HOST"]};" +
                $"Database={builder.Configuration["POSTGRES_DB"]};" +
                $"Username={builder.Configuration["POSTGRES_USER"]};" +
                $"Password={builder.Configuration["POSTGRES_PASSWORD"]}"
            ));
        builder.Services.AddScoped<IDiffRepository, PostgresDiffRepository>();
        break;
    case "inmemory":
    default:
        builder.Services.AddSingleton<IDiffRepository, InMemoryDiffRepository>();
        break;
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Diff Ergo Sum API v1");
        c.RoutePrefix = "docs";
    });
}
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
