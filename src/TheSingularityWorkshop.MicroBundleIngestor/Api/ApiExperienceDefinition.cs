using System.Text.Json.Serialization;

namespace TheSingularityWorkshop.MicroBundleIngestor.Api;

public sealed record ApiExperienceDefinition(
    [property: JsonPropertyName("sourceUrl")] string SourceUrl,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("apiVersion")] string? ApiVersion,
    [property: JsonPropertyName("servers")] IReadOnlyList<ApiServerDefinition> Servers,
    [property: JsonPropertyName("operations")] IReadOnlyList<ApiOperationDefinition> Operations);

public sealed record ApiServerDefinition(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("description")] string? Description);

public sealed record ApiOperationDefinition(
    [property: JsonPropertyName("operationId")] string OperationId,
    [property: JsonPropertyName("method")] string Method,
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("summary")] string? Summary,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("parameters")] IReadOnlyList<ApiParameterDefinition> Parameters,
    [property: JsonPropertyName("responses")] IReadOnlyList<ApiResponseDefinition> Responses);

public sealed record ApiParameterDefinition(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("location")] string Location,
    [property: JsonPropertyName("required")] bool Required,
    [property: JsonPropertyName("schema")] string? SchemaType,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("enum")] IReadOnlyList<string> AllowedValues);

public sealed record ApiResponseDefinition(
    [property: JsonPropertyName("statusCode")] string StatusCode,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("schema")] string? SchemaType,
    [property: JsonPropertyName("contentType")] string? ContentType);
