using System.Text.Json.Serialization;

namespace TheSingularityWorkshop.MicroBundleIngestor.Capabilities;

public sealed record CapabilityDefinition(
    [property: JsonPropertyName("capabilityId")] string CapabilityId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("kind")] CapabilityKind Kind,
    [property: JsonPropertyName("source")] string? Source,
    [property: JsonPropertyName("version")] string? Version,
    [property: JsonPropertyName("operations")] IReadOnlyList<CapabilityOperationDefinition> Operations,
    [property: JsonPropertyName("accessPolicies")] IReadOnlyList<CapabilityAccessPolicy> AccessPolicies);

public enum CapabilityKind
{
    RemoteApi,
    Repository
}

public sealed record CapabilityOperationDefinition(
    [property: JsonPropertyName("operationId")] string OperationId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("method")] string Method,
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("inputs")] IReadOnlyList<CapabilityPortDefinition> Inputs,
    [property: JsonPropertyName("outputs")] IReadOnlyList<CapabilityPortDefinition> Outputs);

public enum CapabilityPortDirection
{
    Input,
    Output
}

public sealed record CapabilityPortDefinition(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("direction")] CapabilityPortDirection Direction,
    [property: JsonPropertyName("valueType")] string ValueType,
    [property: JsonPropertyName("required")] bool Required,
    [property: JsonPropertyName("semanticType")] string? SemanticType,
    [property: JsonPropertyName("allowedValues")] IReadOnlyList<string> AllowedValues);

public sealed record CapabilityAccessPolicy(
    [property: JsonPropertyName("consumer")] string Consumer,
    [property: JsonPropertyName("allowedOperations")] IReadOnlyList<string> AllowedOperations);
