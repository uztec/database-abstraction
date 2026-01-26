using Xunit;

namespace UzunTec.Utils.DatabaseAbstraction.PostgreSQL.Test
{
    [CollectionDefinition("BootstrapCollectionFixture")]
    public class BootstrapCollectionFixture : ICollectionFixture<BootstrapFixture>
    {
    }
}
