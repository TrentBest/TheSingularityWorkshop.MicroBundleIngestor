namespace TheSingularityWorkshop.MicroBundleIngestor.Capabilities;

public static class RepositoryCapabilityDefinition
{
    public const string ForgeConsumer = "forge";
    public const string GetOperation = "microbundle.get";
    public const string PutOperation = "microbundle.put";

    public static CapabilityDefinition Create()
    {
        var addressInputs = new[]
        {
            new CapabilityPortDefinition("bundleId", CapabilityPortDirection.Input, "uint64", true, "microbundle-id", []),
            new CapabilityPortDefinition("version", CapabilityPortDirection.Input, "string", true, "semantic-version", []),
            new CapabilityPortDefinition("contentHash", CapabilityPortDirection.Input, "string", true, "sha256", [])
        };

        return new CapabilityDefinition(
            "TheSingularityWorkshop.MicroBundleRepository",
            "MicroBundle Repository",
            CapabilityKind.Repository,
            null,
            null,
            [
                new CapabilityOperationDefinition(
                    GetOperation,
                    "Get MicroBundle",
                    "GET",
                    "api/microbundles/{bundleId}/{version}/{contentHash}",
                    addressInputs,
                    [
                        new CapabilityPortDefinition(
                            "artifact",
                            CapabilityPortDirection.Output,
                            "microbundle-artifact",
                            false,
                            "microbundle-artifact",
                            [])
                    ]),
                new CapabilityOperationDefinition(
                    PutOperation,
                    "Put MicroBundle",
                    "PUT",
                    "api/microbundles/{bundleId}/{version}/{contentHash}",
                    [
                        new CapabilityPortDefinition(
                            "artifact",
                            CapabilityPortDirection.Input,
                            "microbundle-artifact",
                            true,
                            "microbundle-artifact",
                            [])
                    ],
                    [])
            ],
            [
                new CapabilityAccessPolicy(ForgeConsumer, [GetOperation, PutOperation])
            ]);
    }
}
