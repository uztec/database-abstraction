using System;
using Npgsql;

namespace UzunTec.Utils.DatabaseAbstraction.PostgreSQL.Test
{
    public class BootstrapFixture : IDisposable
    {
        private readonly DbAbstractionTestContainer container;

        public BootstrapFixture()
        {
            IDbQueryBase dbQueryBase = this.BuildDbQueryBase();
            this.container = new DbAbstractionTestContainer(dbQueryBase);
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private IDbQueryBase BuildDbQueryBase()
        {
            const DatabaseDialect databaseDialect = DatabaseDialect.PostgreSQL;
            const string connectionString = @"User Id=<USERNAME>;Password=<PASS>;Server=<SERVER>;Port=5432;Database=<DATABASE_NAME>";

            throw new ApplicationException("Please follow the instructions");
            /* TODO: 
             * Comment throw line (above)
             * Run DbScript.sql file script in your database to create table
             * Change connection string (above) to your server
            */
    
            ConnectionBuilder connectionBuilder = new ConnectionBuilder(NpgsqlFactory.Instance, connectionString);
            return new DbQueryBase(connectionBuilder, databaseDialect);
        }
    }
}
