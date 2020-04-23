using Xunit;

namespace UzunTec.Utils.DatabaseAbstraction.Test
{
    [CollectionDefinition("BootstrapCollectionFixture")]
    public class BootstrapCollectionFixture : ICollectionFixture<BootstrapFixture>
    {
    }
}
