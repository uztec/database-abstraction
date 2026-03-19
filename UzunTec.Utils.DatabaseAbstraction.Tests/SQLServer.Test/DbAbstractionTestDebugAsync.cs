using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UzunTec.Utils.DatabaseAbstraction.SQLServer.Test
{
    [Collection("BootstrapCollectionFixture")]
    public class DbAbstractionTestDebugAsync
    {
        private readonly IDbQueryBase client;

        public DbAbstractionTestDebugAsync(BootstrapFixture bootstrap)
        {
            var baseClient = bootstrap.GetInstance<IDbQueryBase>();
            this.client = new QueryDebug.DebugDbQueryBase(baseClient);
        }

        public static IEnumerable<object[]> Sequencer()
        {
            for (int i = 0; i < 20; i++)
            {
                yield return new object[] { i };
            }
        }

        [Theory]
        [MemberData(nameof(Sequencer))]
        public async Task VerifyQueryLogTest(int counter)
        {
            int initialValue = QueryDebug.QueryLog.QueryLogs.Count;
            string query = " SELECT 1 ORDER BY 1";
            DataResultTable dt = await this.client.GetResultTableAsync(query);
            Assert.Equal(initialValue + 1, QueryDebug.QueryLog.QueryLogs.Count);
        }
    }
}
