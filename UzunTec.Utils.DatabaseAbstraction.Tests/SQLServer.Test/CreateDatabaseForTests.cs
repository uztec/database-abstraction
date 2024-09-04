using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;

namespace UzunTec.Utils.DatabaseAbstraction.SQLServer.Test
{
    public class CreateDatabaseForTests : IDisposable
    {
        private const DatabaseDialect databaseDialect = DatabaseDialect.SqlServer;
        private const string connectionString = @"Data Source=(localdb)\mssqllocaldb; Database=master; Trusted_Connection=True;MultipleActiveResultSets=false;";
        private const string scriptFilePath = "DbScript.sql";
        private readonly string dbName;
        private DbConnection dbConnection;

        public CreateDatabaseForTests(string dbName)
        {
            this.dbName = dbName;
        }
        public void CreateDatabase()
        {
            IDbQueryBase dbQueryBase = this.BuildDbQueryBase();
            string fullSql = File.ReadAllText(scriptFilePath).Replace("@DBNAME", this.dbName);

            foreach (string sql in fullSql.Split(";"))
            {
                dbQueryBase.ExecuteNonQuery(sql);
            }
        }

        public void DropDatabase()
        {
            IDbQueryBase dbQueryBase = this.BuildDbQueryBase();
            dbQueryBase.ExecuteNonQuery("USE [master]");
            dbQueryBase.ExecuteNonQuery($"DROP DATABASE {this.dbName}");
        }

        public void Dispose()
        {
            try
            {
                this.dbConnection.Close();
                this.dbConnection.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private IDbQueryBase BuildDbQueryBase()
        {
            ConnectionBuilder connectionBuilder = new ConnectionBuilder(SqlClientFactory.Instance, connectionString);
            this.dbConnection = connectionBuilder.BuildConnection();
            return new DbQueryBase(this.dbConnection, new AbstractionOptions
            {
                AutoCloseConnection = false,
                Dialect = databaseDialect,
                DialectParameterIdentifier = '@',
                QueryParameterIdentifier = '@',
                SortQueryParameters = false,
            }); ;
        }
    }
}
