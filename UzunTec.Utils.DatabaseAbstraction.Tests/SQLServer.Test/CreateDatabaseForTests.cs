using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace UzunTec.Utils.DatabaseAbstraction.Test
{
    public class CreateDatabaseForTests : IDisposable
    {
        private const DatabaseDialect databaseDialect = DatabaseDialect.SqlServer;
        private const string connectionString = @"Data Source=(localdb)\mssqllocaldb; Database=master; Trusted_Connection=True;MultipleActiveResultSets=false;";
        private const string scriptFilePath = "DbScript.sql";
        private readonly string dbName;
        private IDbConnection connection;

        public CreateDatabaseForTests(string dbName)
        {
            this.dbName = dbName;
        }
        public void CreateDatabase()
        {
            IDbQueryBase dbQueryBase = this.BuildDbQueyBase();
            string fullSql = File.ReadAllText(scriptFilePath).Replace("@DBNAME", this.dbName);

            foreach (string sql in fullSql.Split(";"))
            {
                dbQueryBase.ExecuteNonQuery(sql);
            }
        }

        public void DropDatabase()
        {
            IDbQueryBase dbQueryBase = this.BuildDbQueyBase();
            dbQueryBase.ExecuteNonQuery("USE [master]");
            dbQueryBase.ExecuteNonQuery($"DROP DATABASE {this.dbName}");
        }

        public void Dispose()
        {
            try
            {
                this.connection.Close();
                this.connection.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private IDbQueryBase BuildDbQueyBase()
        {
            ConnectionBuilder connectionBuilder = new ConnectionBuilder(SqlClientFactory.Instance);
            this.connection = connectionBuilder.BuildConnection(connectionString);
            this.connection.Open();
            return new DbQueryBase(this.connection, new AbstractionOptions
            {
                AutoCloseConnection = false,
                Dialect = databaseDialect,
                DialectParameterIdentifier = '@',
                QueryParameterIdentifier = '@',
                SortQueryParameters = false,
            });
        }
    }
}
