using System;
using System.Data;
using MySql.Data.MySqlClient;
using MySql.Server;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace UzunTec.Utils.DatabaseAbstraction.Test
{
    public class DbAbstractionTestContainer : Container
    {
        public static DbAbstractionTestContainer INSTANCE = new DbAbstractionTestContainer();

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

            this.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            this.Register<IDbConnection>(this.BuildConnection, Lifestyle.Singleton);
            this.Register<IDbQueryBase>(this.BuildDbQueyBase, Lifestyle.Singleton);

            this.Register<DBUser>(Lifestyle.Singleton);
            this.Register<UserQueryClient>(Lifestyle.Singleton);

            this.Verify();
        }

        ~DbAbstractionTestContainer()
        {
            MySqlServer.Instance.ShutDown();
        }

        private IDbQueryBase BuildDbQueyBase()
        {
            return new DBBootstrap(this.GetInstance<IDbConnection>(), DatabaseDialect.MySql) ;
        }

        private IDbConnection BuildConnection()
        {
            string connectionString = MySqlServer.Instance.GetConnectionString().Replace("Protocol=pipe;", "");
            ConnectionBuilder connectionBuilder = new ConnectionBuilder(MySqlClientFactory.Instance);
            IDbConnection connection = connectionBuilder.BuildConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
