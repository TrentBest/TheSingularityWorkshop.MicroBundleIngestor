using System.Net;
using System.Text;
using TheSingularityWorkshop.MicroBundleIngestor.Api;

namespace TheSingularityWorkshop.MicroBundleIngestor.Tests;

public sealed class OpenApiUrlIngestorTests
{
    [Fact]
    public async Task IngestAsync_ExtractsOperationsAndParameters()
    {
        const string json = """
        {
          "openapi": "3.0.3",
          "info": {
            "title": "Places API",
            "version": "1.2.0",
            "description": "Returns real-world places."
          },
          "servers": [
            { "url": "https://example.test/api", "description": "Production" }
          ],
          "paths": {
            "/places": {
              "get": {
                "operationId": "findPlaces",
                "summary": "Find places",
                "parameters": [
                  {
                    "name": "latitude",
                    "in": "query",
                    "required": true,
                    "schema": { "type": "number" }
                  },
                  {
                    "name": "category",
                    "in": "query",
                    "required": false,
                    "schema": {
                      "type": "string",
                      "enum": [ "restaurant", "museum" ]
                    }
                  }
                ],
                "responses": {
                  "200": {
                    "description": "Places found",
                    "content": {
                      "application/json": { "schema": { "type": "array" } }
                    }
                  }
                }
              }
            }
          }
        }
        """;

        using var client = new HttpClient(new StubHandler(json));
        var ingestor = new OpenApiUrlIngestor(client);

        var definition = await ingestor.IngestAsync(
            new Uri("https://example.test/openapi.json"));

        Assert.Equal("Places API", definition.Title);
        Assert.Single(definition.Operations);
        Assert.Equal("GET", definition.Operations[0].Method);
        Assert.Equal("/places", definition.Operations[0].Path);
        Assert.Equal(2, definition.Operations[0].Parameters.Count);
        Assert.Equal(
            ["restaurant", "museum"],
            definition.Operations[0].Parameters[1].AllowedValues);
        Assert.Single(definition.Operations[0].Responses);
        Assert.Equal("200", definition.Operations[0].Responses[0].StatusCode);
        Assert.Equal("array", definition.Operations[0].Responses[0].SchemaType);
    }

    private sealed class StubHandler(string body) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
    }
}
