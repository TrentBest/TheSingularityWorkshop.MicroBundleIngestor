using System.Text.Json;

namespace TheSingularityWorkshop.MicroBundleIngestor.Api;

public sealed class OpenApiUrlIngestor(HttpClient httpClient)
{
    public async Task<ApiExperienceDefinition> IngestAsync(
        Uri sourceUrl,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceUrl);

        using var response = await httpClient.GetAsync(sourceUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var root = document.RootElement;

        if (!root.TryGetProperty("openapi", out _) &&
            !root.TryGetProperty("swagger", out _))
        {
            throw new InvalidDataException(
                "The supplied URL did not return an OpenAPI/Swagger JSON document.");
        }

        var info = root.TryGetProperty("info", out var infoElement) ? infoElement : default;
        var title = GetString(info, "title") ?? sourceUrl.Host;

        return new ApiExperienceDefinition(
            sourceUrl.AbsoluteUri,
            title,
            GetString(info, "description"),
            GetString(info, "version"),
            ParseServers(root),
            ParseOperations(root));
    }

    private static IReadOnlyList<ApiServerDefinition> ParseServers(JsonElement root)
    {
        if (!root.TryGetProperty("servers", out var servers) ||
            servers.ValueKind != JsonValueKind.Array)
            return [];

        return servers.EnumerateArray()
            .Select(server => new ApiServerDefinition(
                GetString(server, "url") ?? string.Empty,
                GetString(server, "description")))
            .Where(server => !string.IsNullOrWhiteSpace(server.Url))
            .ToArray();
    }

    private static IReadOnlyList<ApiOperationDefinition> ParseOperations(JsonElement root)
    {
        if (!root.TryGetProperty("paths", out var paths) ||
            paths.ValueKind != JsonValueKind.Object)
            return [];

        var operations = new List<ApiOperationDefinition>();

        foreach (var path in paths.EnumerateObject())
        {
            foreach (var method in path.Value.EnumerateObject())
            {
                if (!IsHttpMethod(method.Name) || method.Value.ValueKind != JsonValueKind.Object)
                    continue;

                var operation = method.Value;
                var operationId = GetString(operation, "operationId")
                    ?? $"{method.Name.ToUpperInvariant()} {path.Name}";

                operations.Add(new ApiOperationDefinition(
                    operationId,
                    method.Name.ToUpperInvariant(),
                    path.Name,
                    GetString(operation, "summary"),
                    GetString(operation, "description"),
                    ParseParameters(operation),
                    ParseResponses(operation)));
            }
        }

        return operations;
    }

    private static IReadOnlyList<ApiParameterDefinition> ParseParameters(JsonElement operation)
    {
        if (!operation.TryGetProperty("parameters", out var parameters) ||
            parameters.ValueKind != JsonValueKind.Array)
            return [];

        return parameters.EnumerateArray()
            .Where(parameter => parameter.ValueKind == JsonValueKind.Object)
            .Select(parameter =>
            {
                var schemaType = parameter.TryGetProperty("schema", out var schema)
                    ? GetString(schema, "type")
                    : GetString(parameter, "type");

                var allowedValues = parameter.TryGetProperty("schema", out var parameterSchema)
                    ? ParseEnum(parameterSchema)
                    : ParseEnum(parameter);

                return new ApiParameterDefinition(
                    GetString(parameter, "name") ?? string.Empty,
                    GetString(parameter, "in") ?? string.Empty,
                    GetBoolean(parameter, "required"),
                    schemaType,
                    GetString(parameter, "description"),
                    allowedValues);
            })
            .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Name))
            .ToArray();
    }

    private static IReadOnlyList<ApiResponseDefinition> ParseResponses(JsonElement operation)
    {
        if (!operation.TryGetProperty("responses", out var responses) ||
            responses.ValueKind != JsonValueKind.Object)
            return [];

        return responses.EnumerateObject()
            .Select(response =>
            {
                string? schemaType = null;
                string? contentType = null;

                if (response.Value.TryGetProperty("content", out var content) &&
                    content.ValueKind == JsonValueKind.Object)
                {
                    var mediaType = content.EnumerateObject().FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(mediaType.Name))
                    {
                        contentType = mediaType.Name;
                        if (mediaType.Value.TryGetProperty("schema", out var schema))
                            schemaType = GetString(schema, "type");
                    }
                }

                return new ApiResponseDefinition(
                    response.Name,
                    GetString(response.Value, "description"),
                    schemaType,
                    contentType);
            })
            .ToArray();
    }

    private static IReadOnlyList<string> ParseEnum(JsonElement element)
    {
        if (!element.TryGetProperty("enum", out var values) ||
            values.ValueKind != JsonValueKind.Array)
            return [];

        return values.EnumerateArray()
            .Where(value => value.ValueKind == JsonValueKind.String)
            .Select(value => value.GetString()!)
            .ToArray();
    }

    private static bool IsHttpMethod(string name) =>
        name.Equals("get", StringComparison.OrdinalIgnoreCase) ||
        name.Equals("post", StringComparison.OrdinalIgnoreCase) ||
        name.Equals("put", StringComparison.OrdinalIgnoreCase) ||
        name.Equals("patch", StringComparison.OrdinalIgnoreCase) ||
        name.Equals("delete", StringComparison.OrdinalIgnoreCase) ||
        name.Equals("head", StringComparison.OrdinalIgnoreCase) ||
        name.Equals("options", StringComparison.OrdinalIgnoreCase) ||
        name.Equals("trace", StringComparison.OrdinalIgnoreCase);

    private static string? GetString(JsonElement element, string propertyName) =>
        element.ValueKind == JsonValueKind.Object &&
        element.TryGetProperty(propertyName, out var value) &&
        value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static bool GetBoolean(JsonElement element, string propertyName) =>
        element.ValueKind == JsonValueKind.Object &&
        element.TryGetProperty(propertyName, out var value) &&
        value.ValueKind == JsonValueKind.True;
}
