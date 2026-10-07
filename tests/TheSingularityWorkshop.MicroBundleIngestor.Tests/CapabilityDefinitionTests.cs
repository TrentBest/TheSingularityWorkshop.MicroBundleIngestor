using TheSingularityWorkshop.MicroBundleIngestor.Api;
using TheSingularityWorkshop.MicroBundleIngestor.Capabilities;

namespace TheSingularityWorkshop.MicroBundleIngestor.Tests;

public sealed class CapabilityDefinitionTests
{
    [Fact]
    public void RepositoryCapability_ExposesPutOnlyToForge()
    {
        var capability = RepositoryCapabilityDefinition.Create();

        Assert.Equal(CapabilityKind.Repository, capability.Kind);
        Assert.Contains(capability.Operations, operation => operation.OperationId == RepositoryCapabilityDefinition.GetOperation);
        Assert.Contains(capability.Operations, operation => operation.OperationId == RepositoryCapabilityDefinition.PutOperation);

        var policy = Assert.Single(capability.AccessPolicies);
        Assert.Equal(RepositoryCapabilityDefinition.ForgeConsumer, policy.Consumer);
        Assert.Equal(
            [RepositoryCapabilityDefinition.GetOperation, RepositoryCapabilityDefinition.PutOperation],
            policy.AllowedOperations);
    }

    [Fact]
    public void Compiler_ConvertsApiParametersIntoInputPorts()
    {
        var definition = new ApiExperienceDefinition(
            "https://example.test/openapi.json",
            "Places API",
            "Places",
            "1.0",
            [new ApiServerDefinition("https://example.test/api", null)],
            [
                new ApiOperationDefinition(
                    "findPlaces",
                    "GET",
                    "/places",
                    "Find places",
                    null,
                    [
                        new ApiParameterDefinition("latitude", "query", true, "number", null, []),
                        new ApiParameterDefinition("category", "query", false, "string", null, ["restaurant", "museum"])
                    ],
                    [new ApiResponseDefinition("200", "Places found", "array", "application/json")])
            ]);

        var capability = CapabilityCompiler.Compile(definition);
        var operation = Assert.Single(capability.Operations);

        Assert.Equal(CapabilityKind.RemoteApi, capability.Kind);
        Assert.Equal(2, operation.Inputs.Count);
        Assert.Equal("geographic-coordinate", operation.Inputs[0].SemanticType);
        Assert.Equal("enumeration", operation.Inputs[1].SemanticType);
        Assert.Single(operation.Outputs);
        Assert.Equal("array", operation.Outputs[0].ValueType);
        Assert.Equal("json", operation.Outputs[0].SemanticType);
    }
}
