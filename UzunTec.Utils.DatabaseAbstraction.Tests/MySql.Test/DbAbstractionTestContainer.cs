using System;
using MySql.Data.MySqlClient;
using MySql.Server;
using SimpleInjector;
using UzunTec.Utils.DatabaseAbstraction.MySql.Test;

namespace UzunTec.Utils.DatabaseAbstraction.Test
{
    public class DbAbstractionTestContainer : Container
    {
        public static DbAbstractionTestContainer INSTANCE = new DbAbstractionTestContainer();
        private readonly string dbTestName = "UZTEC_DB_ABSTRACTION_TEST";

        private DbAbstractionTestContainer()
        {
            try
            {
                MySqlServer.Instance.KillPreviousProcesses();
                MySqlServer.Instance.StartServer();
            }
            catch (PlatformNotSupportedException ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }

            this.Register<IDbQueryBase>(this.BuildDbQueryBase, Lifestyle.Singleton);
            this.Register<DBUser>(Lifestyle.Singleton);
            this.Register<UserQueryClient>(Lifestyle.Singleton);

            this.CreateDatabaseForTests();
            this.Verify();
        }

        private void CreateDatabaseForTests()
        {
            string connectionString = this.GetBaseConnectionString();
            using (CreateDatabaseForTests createDb = new CreateDatabaseForTests(MySqlClientFactory.Instance, connectionString, DatabaseDialect.MySql))
            {
                createDb.Create(dbTestName);
            };

        }

        private string GetBaseConnectionString(string dbName = null)
        {
            string baseConnectionString = MySqlServer.Instance.GetConnectionString().Replace("Protocol=pipe;", "");
            return baseConnectionString + ((dbName != null) ? $"Initial Catalog={dbName};" : "");
        }

        ~DbAbstractionTestContainer()
        {
            MySqlServer.Instance.ShutDown();
        }

        private IDbQueryBase BuildDbQueryBase()
        {
            string connectionString = this.GetBaseConnectionString(this.dbTestName);
            ConnectionBuilder connectionBuilder = new ConnectionBuilder(MySqlClientFactory.Instance, connectionString);
            return new DbQueryBase(connectionBuilder, DatabaseDialect.MySql);
        }
    }
}
