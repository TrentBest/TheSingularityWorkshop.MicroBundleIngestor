using System.Text.Json;
using TheSingularityWorkshop.MicroBundleIngestor.Api;

if (args.Length != 1 || !Uri.TryCreate(args[0], UriKind.Absolute, out var sourceUrl))
{
    Console.Error.WriteLine("Usage: MicroBundleIngestor <openapi-or-swagger-json-url>");
    return 2;
}

using var httpClient = new HttpClient();
var ingestor = new OpenApiUrlIngestor(httpClient);
var definition = await ingestor.IngestAsync(sourceUrl);

Console.WriteLine(JsonSerializer.Serialize(
    definition,
    new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true }));
