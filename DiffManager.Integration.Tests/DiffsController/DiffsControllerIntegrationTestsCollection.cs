namespace DiffManager.Integration.Tests.DiffsController
{
    using Xunit;

    [CollectionDefinition("DiffsController integration tests collection")]
    public class DiffsControllerIntegrationTestsCollection : ICollectionFixture<TestHostCollectionFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
