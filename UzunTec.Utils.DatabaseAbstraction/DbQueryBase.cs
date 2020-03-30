using System;
using System.Data;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public class DbQueryBase :IDbQueryBase
    {
        private IDbTransaction dbTransaction;
        private IDbCommand lastCommand;
        private readonly IDbConnection dbConnection;

        public DbQueryBase(IDbConnection connection)
        {
            this.dbConnection = connection;
        }

        #region IDbTransaction
        /// <summary>
        /// Start DB Transaction
        /// </summary>
        public void BeginTransaction()
        {
            if (this.dbConnection.State == ConnectionState.Open && this.dbTransaction != null)
            {
                return;
            }
            this.dbConnection.Open();
            this.dbTransaction = this.dbConnection.BeginTransaction();
        }

        /// <summary>
        /// Finalize and Commit DB Transaction
        /// </summary>
        public void CommitTransaction()
        {
            this.dbTransaction.Commit();
            this.dbConnection.Close();
            this.dbTransaction = null;
        }

        /// <summary>
        /// Finalize but Rollback DB Transaction
        /// </summary>
        public void RollbackTransaction()
        {
            this.dbTransaction.Rollback();
            this.dbConnection.Close();
            this.dbTransaction = null;
        }

        #endregion

        #region GetResultTable

        public DataResultTable GetResultTable(string queryString)
        {
            return this.GetResultTable(queryString, new DataBaseParameter[0]);
        }

        private DataResultTable GetResultTable(string queryString, IDbTransaction trans)
        {
            return this.GetResultTable(queryString, trans, null);
        }

        public DataResultTable GetResultTable(string queryString, DataBaseParameter[] parameters)
        {
            if (this.dbTransaction != null && this.dbTransaction.Connection.State == ConnectionState.Open)
            {
                return this.GetResultTable(queryString, this.dbTransaction, parameters);
            }

            if (this.dbConnection.State == ConnectionState.Closed)
            {
                this.dbConnection.Open();
            }

            this.lastCommand = this.dbConnection.CreateCommand(queryString, parameters);
            return new DataResultTable(this.lastCommand.ExecuteReader());
        }

        private DataResultTable GetResultTable(string queryString, IDbTransaction trans, DataBaseParameter[] parameters)
        {
            if (trans == null)
            {
                return this.GetResultTable(queryString, parameters);
            }

            this.lastCommand = trans.Connection.CreateCommand(queryString, parameters);
            this.lastCommand.Transaction = trans;
            return new DataResultTable(this.lastCommand.ExecuteReader());
        }

        #endregion

        #region GetSingleRecord

        public DataResultRecord GetSingleRecord(string queryString)
        {
            DataResultTable dt = this.GetResultTable(queryString);
            return (dt.Count == 1) ? dt[0] : null;
        }

        public DataResultRecord GetSingleRecord(string queryString, DataBaseParameter[] parameters)
        {
            DataResultTable dt = this.GetResultTable(queryString, parameters);
            return (dt.Count == 1) ? dt[0] : null;
        }

        private DataResultRecord GetSingleRecord(string queryString, IDbTransaction trans)
        {
            DataResultTable dt = this.GetResultTable(queryString, trans);
            return (dt.Count == 1) ? dt[0] : null;
        }

        private DataResultRecord GetSingleRecord(string queryString, IDbTransaction trans, DataBaseParameter[] parameters)
        {
            DataResultTable dt = this.GetResultTable(queryString, trans, parameters);
            return (dt.Count == 1) ? dt[0] : null;
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// To execute INSERT, UPDATE or DELETE queries
        /// </summary>
        public int ExecuteNonQuery(string queryString)
        {
            return this.ExecuteNonQuery(queryString, new DataBaseParameter[0]);
        }

        private int ExecuteNonQuery(string queryString, IDbTransaction trans)
        {
            return this.ExecuteNonQuery(queryString, trans, new DataBaseParameter[0]);
        }

        /// <summary>
        /// To execute INSERT, UPDATE or DELETE queries with params
        /// </summary>
        public int ExecuteNonQuery(string queryString, DataBaseParameter[] parameters)
        {
            if (this.dbTransaction != null && this.dbTransaction.Connection.State == ConnectionState.Open)
            {
                return this.ExecuteNonQuery(queryString, this.dbTransaction, parameters);
            }

            if (this.dbConnection.State == ConnectionState.Closed)
            {
                this.dbConnection.Open();
            }

            this.lastCommand = this.dbConnection.CreateCommand(queryString, parameters);
            int RowsAffected = this.lastCommand.ExecuteNonQuery();
            return RowsAffected;

        }

        /// <summary>
        /// To execute INSERT, UPDATE or DELETE queries with transaction
        /// </summary>
        private int ExecuteNonQuery(string queryString, IDbTransaction trans, DataBaseParameter[] parameters)
        {
            if (trans == null)
            {
                return this.ExecuteNonQuery(queryString, parameters);
            }

            this.lastCommand = trans.Connection.CreateCommand(queryString, parameters);
            this.lastCommand.Transaction = trans;
            int RowsAffected = this.lastCommand.ExecuteNonQuery();
            return RowsAffected;
        }

        #endregion

        #region ExecuteScalar

        public object ExecuteScalar(string queryString)
        {
            return this.ExecuteScalar(queryString, new DataBaseParameter[0]);
        }

        private object ExecuteScalar(string queryString, IDbTransaction trans)
        {
            return this.ExecuteScalar(queryString, trans, new DataBaseParameter[0]);
        }

        public object ExecuteScalar(string queryString, DataBaseParameter[] parameters)
        {
            if (this.dbTransaction != null && this.dbTransaction.Connection.State == ConnectionState.Open)
            {
                return this.ExecuteNonQuery(queryString, this.dbTransaction, parameters);
            }

            if (this.dbConnection.State == ConnectionState.Closed)
            {
                this.dbConnection.Open();
            }
            this.lastCommand = this.dbConnection.CreateCommand(queryString, parameters);
            return this.lastCommand.ExecuteScalar();
        }

        private object ExecuteScalar(string queryString, IDbTransaction trans, DataBaseParameter[] parameters)
        {
            if (trans == null)
            {
                return this.ExecuteScalar(queryString, parameters);
            }
            this.lastCommand = trans.Connection.CreateCommand(queryString, parameters);
            this.lastCommand.Transaction = trans;
            return this.lastCommand.ExecuteScalar();
        }

        #endregion

        public long GetLastInsertedID()
        {
            string getIDENTITY = @"SELECT @@IDENTITY";
            IDbCommand newCommand = this.lastCommand.Connection.CreateCommand(getIDENTITY);
            newCommand.Transaction = this.dbTransaction;
            return Convert.ToInt64(newCommand.ExecuteScalar());
        }

    }
}
