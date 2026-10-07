using System.Text.Json;
using TheSingularityWorkshop.MicroBundleIngestor.Api;
using TheSingularityWorkshop.MicroBundleIngestor.Capabilities;

if (args.Length == 1 && args[0].Equals("--repository", StringComparison.OrdinalIgnoreCase))
{
    WriteJson(RepositoryCapabilityDefinition.Create());
    return;
}

if (args.Length != 1 || !Uri.TryCreate(args[0], UriKind.Absolute, out var sourceUrl))
{
    Console.Error.WriteLine("Usage:");
    Console.Error.WriteLine("  MicroBundleIngestor <openapi-or-swagger-json-url>");
    Console.Error.WriteLine("  MicroBundleIngestor --repository");
    return 2;
}

using var httpClient = new HttpClient();
var ingestor = new OpenApiUrlIngestor(httpClient);
var definition = await ingestor.IngestAsync(sourceUrl);
var capability = CapabilityCompiler.Compile(definition);

WriteJson(capability);

static void WriteJson(object value) =>
    Console.WriteLine(JsonSerializer.Serialize(
        value,
        new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true }));
