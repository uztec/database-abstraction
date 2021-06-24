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
            string connectionString = MySqlServer.Instance.GetConnectionString().Replace("Protocol=pipe;", "");
            ConnectionBuilder connectionBuilder = new ConnectionBuilder(MySqlClientFactory.Instance, connectionString);
            return new DBBootstrap(connectionBuilder, DatabaseDialect.MySql) ;
        }
    }
}
