using Xunit;

namespace UzunTec.Utils.DatabaseAbstraction.SQLServer.Test
{
    [CollectionDefinition("BootstrapCollectionFixture")]
    public class BootstrapCollectionFixture : ICollectionFixture<BootstrapFixture>
    {
    }
}
