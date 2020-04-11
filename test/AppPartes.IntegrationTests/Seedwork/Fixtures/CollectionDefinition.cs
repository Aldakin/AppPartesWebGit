using Xunit;

namespace AppPartes.IntegrationTests.Seedwork.Fixtures
{
    [CollectionDefinition(nameof(Collection.TestServer))]
    public class ServerCollectionDefinition : ICollectionFixture<ServerFixture>
    {
    }
}
