using System;
using System.Data;
using System.Data.Common;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public class ConnectionBuilder
    {
        private readonly DbProviderFactory dbFactory;

        public ConnectionBuilder(DbProviderFactory providerFactory)
        {
            this.dbFactory = providerFactory;
        }

        public IDbConnection GetConnection(string server, string databaseName, string user, string password)
        {
            string connectionString = this.GetConnectionString(server, databaseName, user, password);
            return this.BuildConnection(connectionString);
        }

        public IDbConnection GetConnection(string server, string databaseName, int? port, string user, string password)
        {
            if (port != null)
            {
                server = $"{server},{port}";
            }

            return this.GetConnection(server, databaseName, user, password);
        }

        private string GetConnectionString(string server, string databaseName, string user, string password)
        {
            return String.Format("Persist Security Info=False;Initial Catalog={0};Data Source={1};User ID={2}; Password={3};",
                    databaseName, server, user, password);
        }


        public IDbConnection BuildConnection(string connectionString)
        {
            IDbConnection connection = this.dbFactory.CreateConnection();
            connection.ConnectionString = connectionString;
            return connection;
        }

    }
}

