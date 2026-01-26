using System.Threading.Tasks;
using Xunit;

namespace UzunTec.Utils.DatabaseAbstraction.SQLServer.Test
{
    [Collection("BootstrapCollectionFixture")]
    public class DbAbstractionTestLimitAsync
    {
        private readonly IDbQueryBase client;

        public DbAbstractionTestLimitAsync(BootstrapFixture bootstrap)
        {
            this.client = bootstrap.GetInstance<IDbQueryBase>();
        }

        [Fact]
        public async Task GetReversedLimitedRecordsTest()
        {
            string query = this.GenerateQueryString(100, true);

            DataResultTable dt = await this.client.GetResultTableAsync(query, 10);
            Assert.Equal(10, dt.Count);

            int firstRecord = dt[0].GetValue<int>("n");
            int lastRecord = dt[dt.Count - 1].GetValue<int>("n");

            Assert.Equal(100, firstRecord);
            Assert.Equal(91, lastRecord);
        }

        private string GenerateQueryString(int rows, bool desc = false)
        {
            string query = "SELECT * FROM ( SELECT 1 AS n ";
            for (int i = 1; i < rows; i++)
            {
                query += $"UNION SELECT {i + 1} ";
            }

            query += ") A ORDER BY n " + ((desc) ? "DESC" : "ASC");

            return query;
        }
    }
}
