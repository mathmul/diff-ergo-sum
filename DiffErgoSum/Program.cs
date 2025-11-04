using System.Reflection;

using DiffErgoSum.Application;
using DiffErgoSum.Controllers.Filters;
using DiffErgoSum.Domain;
using DiffErgoSum.Infrastructure;
using DiffErgoSum.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
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
});
builder.Services.AddTransient<IDiffService, DiffService>();
builder.Services.AddSingleton<IDiffRepository, InMemoryDiffRepository>();

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
