using System.Data;
using System.Data.SqlClient;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace UzunTec.Utils.DatabaseAbstraction.Test
{
    public class DbAbstractionTestContainer : Container
    {
        public string connectionString = @"Data Source=(localdb)\mssqllocaldb; Database=master; Trusted_Connection=True; MultipleActiveResultSets=true";
        public static DbAbstractionTestContainer INSTANCE = new DbAbstractionTestContainer();

        private DbAbstractionTestContainer()
        {
            this.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            this.Register<IDbConnection>(this.BuildConnection, Lifestyle.Singleton);
            this.Register<IDbQueryBase, DBBootstrap>(Lifestyle.Singleton);

            this.Register<DBUser>(Lifestyle.Singleton);
            this.Register<UserQueryClient>(Lifestyle.Singleton);

            this.Verify();
        }

        private IDbConnection BuildConnection()
        {
            ConnectionBuilder connectionBuilder = new ConnectionBuilder(SqlClientFactory.Instance);
            IDbConnection connection = connectionBuilder.BuildConnection(connectionString);
            connection.Open();
            return connection;
        }

    }
}
