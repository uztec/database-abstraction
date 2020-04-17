using System;
using System.Data;
using System.Data.SqlClient;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace UzunTec.Utils.DatabaseAbstraction.Test
{
    public class DbAbstractionTestContainer : Container
    {
        public DatabaseDialect databaseDialect = DatabaseDialect.SqlServer;
        public string connectionString = @"Data Source=(localdb)\mssqllocaldb; Database=master; Trusted_Connection=True;";

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
            }
            throw new ApplicationException("Database Engine not found.");

        }
    }
}
