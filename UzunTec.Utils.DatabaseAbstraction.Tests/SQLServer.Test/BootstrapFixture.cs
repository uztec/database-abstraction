using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace UzunTec.Utils.DatabaseAbstraction.Test
{
    public class BootstrapFixture : IDisposable
    {
        private const DatabaseDialect databaseDialect = DatabaseDialect.SqlServer;
        private const string connectionString = @"Data Source=(localdb)\mssqllocaldb; Database=master; Trusted_Connection=True;";
        private const string scriptFilePaht = "DbScript.sql";
        private const string dbName = "UZTEC_DB_ABSTRACTION_TEST";
        private readonly DbAbstractionTestContainer container;
        private IDbConnection connection;

        public BootstrapFixture()
        {
            IDbQueryBase dbQueryBase = this.BuildDbQueyBase();
            this.container = new DbAbstractionTestContainer(dbQueryBase);

            string fullSql = File.ReadAllText(scriptFilePaht).Replace("@DBNAME", dbName);

            foreach (string sql in fullSql.Split(";"))
            {
                dbQueryBase.ExecuteNonQuery(sql);
            }
        }

        public T GetInstance<T>() where T : class
        {
            return this.container.GetInstance<T>();
        }

        public void Dispose()
        {
            try
            {
                this.container.Dispose();
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
            return new DbQueryBase(this.connection, databaseDialect);
        }
    }
}
