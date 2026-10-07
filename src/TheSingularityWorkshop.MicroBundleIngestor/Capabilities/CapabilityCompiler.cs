using TheSingularityWorkshop.MicroBundleIngestor.Api;

namespace TheSingularityWorkshop.MicroBundleIngestor.Capabilities;

public static class CapabilityCompiler
{
    public static CapabilityDefinition Compile(ApiExperienceDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var operations = definition.Operations
            .Select(operation => new CapabilityOperationDefinition(
                operation.OperationId,
                operation.Summary ?? operation.OperationId,
                operation.Method,
                operation.Path,
                operation.Parameters
                    .Select(parameter => new CapabilityPortDefinition(
                        parameter.Name,
                        CapabilityPortDirection.Input,
                        parameter.SchemaType ?? "string",
                        parameter.Required,
                        InferSemanticType(parameter),
                        parameter.AllowedValues))
                    .ToArray(),
                []))
            .ToArray();

        return new CapabilityDefinition(
            $"{definition.Title.Trim()}:{definition.ApiVersion ?? "unversioned"}",
            definition.Title,
            CapabilityKind.RemoteApi,
            definition.SourceUrl,
            definition.ApiVersion,
            operations,
            []);
    }

    private static string? InferSemanticType(ApiParameterDefinition parameter)
    {
        if (parameter.Name.Contains("latitude", StringComparison.OrdinalIgnoreCase) ||
            parameter.Name.Contains("longitude", StringComparison.OrdinalIgnoreCase))
            return "geographic-coordinate";

        if (parameter.Name.Contains("location", StringComparison.OrdinalIgnoreCase))
            return "location";

        if (parameter.AllowedValues.Count > 0)
            return "enumeration";

        return null;
    }
}
