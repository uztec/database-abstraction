using System;
using System.Data;
using System.Data.SqlClient;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using MySql.Data.MySqlClient;
using Microsoft.Data.Sqlite;

namespace UzunTec.Utils.DatabaseAbstraction.Test
{
    public class DbAbstractionTestContainer : Container
    {
        //public DatabaseDialect databaseDialect = DatabaseDialect.SqlServer;
        //public string connectionString = @"Data Source=(localdb)\mssqllocaldb; Database=master; Trusted_Connection=True;";

        public DatabaseDialect databaseDialect = DatabaseDialect.MySql;
        public string connectionString = @"Server=209.208.27.132;Database=UZTEC_DB_ABSTRACTION_TEST;Uid=renato;Pwd=renato-uztec.123;";

        public static DbAbstractionTestContainer INSTANCE = new DbAbstractionTestContainer();

        private DbAbstractionTestContainer()
        {
            this.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            this.Register<IDbConnection>(this.BuildConnection, Lifestyle.Singleton);
            this.Register<IDbQueryBase>(this.BuildDbQueyBase, Lifestyle.Singleton);

            this.Register<DBUser>(Lifestyle.Singleton);
            this.Register<UserQueryClient>(Lifestyle.Singleton);

            this.Verify();
        }

        private IDbQueryBase BuildDbQueyBase()
        {
            return new DBBootstrap(this.GetInstance<IDbConnection>(), this.databaseDialect);
        }

        private IDbConnection BuildConnection()
        {
            ConnectionBuilder connectionBuilder = this.GetConnectionBuilder(this.databaseDialect);
            IDbConnection connection = connectionBuilder.BuildConnection(this.connectionString);
            connection.Open();
            return connection;
        }

        private ConnectionBuilder GetConnectionBuilder(DatabaseDialect engine)
        {
            switch (engine)
            {
                case DatabaseDialect.SqlServer:
                    return new ConnectionBuilder(SqlClientFactory.Instance);

                case DatabaseDialect.MySql:
                    return new ConnectionBuilder(MySqlClientFactory.Instance);

                case DatabaseDialect.SQLite:
                    return new ConnectionBuilder(SqliteFactory.Instance);
            }
            throw new ApplicationException("Database Engine not found.");

        }
    }
}
