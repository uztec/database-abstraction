using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace UzunTec.Utils.DatabaseAbstraction.SQLServer.Test
{
    [Collection("BootstrapCollectionFixture")]
    public class DbAbstractionTestPagingAsync
    {
        private readonly IDbQueryBase client;

        public DbAbstractionTestPagingAsync(BootstrapFixture bootstrap)
        {
            this.client = bootstrap.GetInstance<IDbQueryBase>();
        }

        [Fact]
        public async Task GetLimitedRecordsTest()
        {
            string query = this.GenerateQueryString(100);

            DataResultTable dt = await this.client.GetLimitedRecordsAsync(query, 20, 20);
            Assert.Equal(20, dt.Count);

            int firstRecord = dt[0].GetValue<int>("n");
            int lastRecord = dt[dt.Count - 1].GetValue<int>("n");

            Assert.Equal(21, firstRecord);
            Assert.Equal(40, lastRecord);
        }

        [Fact]
        public async Task GetTop10RecordsTest()
        {
            string query = this.GenerateQueryString(1000);
            DataResultTable dt = await this.client.GetResultTableAsync(query, 10);
            Assert.Equal(10, dt.Count);
        }

        public static IEnumerable<object[]> GetPagedResultTableMassTest()
        {
            return new List<object[]>
            {
                new object[] { 1, 1, 1, 1, 1 },
                new object[] { 0, 0, 1, 1, 1 },
                new object[] { 3, 10, 10, 21, 30 },
                new object[] { 25, 5, 5, 121, 125 },
                new object[] { 5, 20, 20, 81, 100 },
                new object[] { 11, 100, 0, 0, 0 },
            };
        }

        [Theory]
        [MemberData(nameof(GetPagedResultTableMassTest))]
        public async Task PagedResultTableMassTest(int page, int pageSize, int expCount, int expFirst, int expLast)
        {
            string query = this.GenerateQueryString(1000);

            DataResultTable dt = await this.client.GetPagedResultTableAsync(query, page, pageSize);

            Assert.Equal(expCount, dt.Count);

            if (dt.Count > 0)
            {
                int firstRecord = dt[0].GetValue<int>("n");
                int lastRecord = dt[dt.Count - 1].GetValue<int>("n");

                Assert.Equal(expFirst, firstRecord);
                Assert.True(expLast == lastRecord);
            }
        }

        private string GenerateQueryString(int rows, bool desc = false)
        {
            string query = "SELECT * FROM ( SELECT 1 AS n ";
            for (int i = 1; i < rows; i++)
            {
                query += $"UNION SELECT {i} ";
            }

            query += ") A ORDER BY n " + ((desc) ? "DESC" : "ASC");

            return query;
        }
    }
}
