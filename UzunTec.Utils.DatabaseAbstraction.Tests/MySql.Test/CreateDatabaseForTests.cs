using System;
using System.Data;
using System.Data.Common;
using System.IO;

namespace UzunTec.Utils.DatabaseAbstraction.MySql.Test
{
    public class CreateDatabaseForTests : IDisposable
    {
        private readonly IDbConnection dbConnection;
        private readonly IDbQueryBase dbQueryBase;

        public CreateDatabaseForTests(DbProviderFactory dbFactory, string connectionString, DatabaseDialect databaseDialect)
        {
            this.dbConnection = new ConnectionBuilder(dbFactory, connectionString).BuildConnection();
            this.dbQueryBase = this.BuildDbQueryBase(databaseDialect);
        }

        private IDbQueryBase BuildDbQueryBase(DatabaseDialect databaseDialect)
        {
            return new DbQueryBase(this.dbConnection, new AbstractionOptions
            {
                AutoCloseConnection = false,
                Dialect = databaseDialect,
                DialectParameterIdentifier = '@',
                QueryParameterIdentifier = '@',
                SortQueryParameters = false,
            }); ;
        }

        public void Create(string dbName)
        {
            string fullSql = File.ReadAllText("DbScript.sql").Replace("@DBNAME", dbName);

            foreach (string sql in fullSql.Split(";"))
            {
                this.dbQueryBase.ExecuteNonQuery(sql);
            }
        }

        public void Dispose()
        {
            this.dbConnection.Close();
            this.dbConnection.Dispose();
        }
    }
}
