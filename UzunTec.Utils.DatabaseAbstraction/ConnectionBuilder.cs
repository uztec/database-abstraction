using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public class ConnectionBuilder
    {
        private readonly DbProviderFactory dbFactory;
        private readonly string connectionString;
        private readonly DbConnection dbConnection;
        private DbTransaction dbTransaction;

        public ConnectionBuilder(DbProviderFactory providerFactory, string connectionString)
        {
            this.dbFactory = providerFactory;
            this.connectionString = connectionString;
        }

        public ConnectionBuilder(DbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public DbConnection BuildConnection()
        {
            DbConnection connection = this.dbFactory.CreateConnection();
            connection.ConnectionString = this.connectionString;
            return connection;
        }

        internal void OpenConnection(Action<DbConnection, DbTransaction> action)
        {
            if (this.dbConnection == null && this.dbTransaction == null)
            {
                using (DbConnection connection = this.BuildConnection())
                {
                    connection.Open();
                    action(connection, null);
                }
            }
            else if (this.dbTransaction != null)
            {
                action(this.dbTransaction.Connection, this.dbTransaction);
            }
            else
            {
                action(this.dbConnection, null);
            }
        }

        internal async Task OpenConnectionAsync(Func<DbConnection, DbTransaction, Task> action)
        {
            if (this.dbConnection == null && this.dbTransaction == null)
            {
                using (DbConnection connection = this.BuildConnection())
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    await action(connection, null).ConfigureAwait(false);
                }
            }
            else if (this.dbTransaction != null)
            {
                await action(this.dbTransaction.Connection, this.dbTransaction).ConfigureAwait(false);
            }
            else
            {
                await action(this.dbConnection, null).ConfigureAwait(false);
            }
        }


        #region DbTransaction
        /// <summary>
        /// Start DB Transaction
        /// </summary>
        public void BeginTransaction()
        {
            if (this.dbConnection == null)
            {
                if (this.dbTransaction == null)
                {
                    IDbConnection conn = this.BuildConnection();
                    conn.Open();
                    this.dbTransaction = conn.BeginTransaction() as DbTransaction;
                }
            }
            else if (this.dbTransaction == null)
            {
                if (this.dbConnection.State == ConnectionState.Closed)
                {
                    this.dbConnection.Open();
                }
                this.dbTransaction = this.dbConnection.BeginTransaction();
            }
        }

        /// <summary>
        /// Finalize and Commit DB Transaction
        /// </summary>
        public void CommitTransaction()
        {
            DbConnection conn = this.dbTransaction?.Connection;
            this.dbTransaction?.Commit();
            conn?.Close();
            this.dbTransaction = null;
        }

        /// <summary>
        /// Finalize but Rollback DB Transaction
        /// </summary>
        public void RollbackTransaction()
        {
            DbConnection conn = this.dbTransaction?.Connection;
            this.dbTransaction?.Rollback();
            conn?.Close();
            this.dbTransaction = null;
        }
        #endregion
    }
}

