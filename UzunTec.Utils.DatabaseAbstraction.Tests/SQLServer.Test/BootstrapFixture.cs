using System;
using System.Data.SqlClient;

namespace UzunTec.Utils.DatabaseAbstraction.SQLServer.Test
{
    public class BootstrapFixture : IDisposable
    {
        private const DatabaseDialect databaseDialect = DatabaseDialect.SqlServer;
        private const string connectionString = @"Data Source=(localdb)\mssqllocaldb; Database=UZTEC_DB_ABSTRACTION_TEST; Trusted_Connection=True;MultipleActiveResultSets=true;";
        private readonly DbAbstractionTestContainer container;

        public BootstrapFixture()
        {
            using (CreateDatabaseForTests db = new CreateDatabaseForTests("UZTEC_DB_ABSTRACTION_TEST"))
            {
                db.CreateDatabase();
            }

            IDbQueryBase dbQueryBase = this.BuildDbQueryBase();
            this.container = new DbAbstractionTestContainer(dbQueryBase);
        }

        public T GetInstance<T>() where T : class
        {
            return this.container.GetInstance<T>();
        }

        public void Dispose()
        {
            try
            {
                this.container.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private IDbQueryBase BuildDbQueryBase()
        {
            ConnectionBuilder connectionBuilder = new ConnectionBuilder(SqlClientFactory.Instance, connectionString);
            return new DbQueryBase(connectionBuilder, databaseDialect);
        }
    }
}
