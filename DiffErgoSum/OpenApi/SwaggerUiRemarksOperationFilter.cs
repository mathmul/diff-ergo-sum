namespace DiffErgoSum.OpenApi;

using System.Reflection;
using System.Xml.XPath;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

/// <summary>
/// Includes &lt;remarks&gt; sections from XML documentation into Swagger operation descriptions.
/// Safe for environments where XML docs may not be present.
/// </summary>
public class SwaggerUiRemarksOperationFilter : IOperationFilter
{
    private static readonly Lazy<XPathDocument?> XmlComments = new(() =>
    {
        try
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            return File.Exists(xmlPath) ? new XPathDocument(xmlPath) : null;
        }
        catch
        {
            return null;
        }
    });

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var xml = XmlComments.Value;
        if (xml is null)
            return;

        try
        {
            var methodFullName = $"M:{context.MethodInfo.DeclaringType?.FullName}.{context.MethodInfo.Name}";
            var node = xml.CreateNavigator()?.SelectSingleNode($"//member[@name='{methodFullName}']");
            var remarksNode = node?.SelectSingleNode("remarks");

            if (remarksNode != null)
            {
                operation.Description = (operation.Description ?? string.Empty) +
                    "\n\n" + remarksNode.Value.Trim();
            }
        }
        catch
        {
            // swallow any parsing issues silently
        }
    }
}
