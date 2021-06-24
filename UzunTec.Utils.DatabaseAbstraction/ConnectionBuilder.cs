using System;
using System.Data;
using System.Data.Common;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public class ConnectionBuilder
    {
        private readonly DbProviderFactory dbFactory;
        private readonly string connectionString;
        private readonly IDbConnection dbConnection;
        private IDbTransaction dbTransaction;

        public ConnectionBuilder(DbProviderFactory providerFactory, string connectionString)
        {
            this.dbFactory = providerFactory;
            this.connectionString = connectionString;
        }

        public ConnectionBuilder(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public IDbConnection BuildConnection()
        {
            IDbConnection connection = this.dbFactory.CreateConnection();
            connection.ConnectionString = this.connectionString;
            return connection;
        }

        internal void OpenConnection(Action<IDbConnection, IDbTransaction> action)
        {
            if (this.dbConnection == null && this.dbTransaction == null)
            {
                using (IDbConnection connection = this.BuildConnection())
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


        #region IDbTransaction
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
                    this.dbTransaction = conn.BeginTransaction();
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
            IDbConnection conn = this.dbTransaction?.Connection;
            this.dbTransaction?.Commit();
            conn?.Close();
            this.dbTransaction = null;
        }

        /// <summary>
        /// Finalize but Rollback DB Transaction
        /// </summary>
        public void RollbackTransaction()
        {
            IDbConnection conn = this.dbTransaction?.Connection;
            this.dbTransaction?.Rollback();
            conn?.Close();
            this.dbTransaction = null;
        }
        #endregion


    }
}

