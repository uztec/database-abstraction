using System;
using System.Data;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public class DBbase
    {
        private IDbTransaction dbTransaction;
        private IDbCommand lastCommand;
        private readonly IDbConnection dbConnection;

        public DBbase(IDbConnection connection)
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

        #region GetDataTable

        public DataTable GetDataTable(string queryString)
        {
            return this.GetDataTable(queryString, new DataBaseParameter[0]);
        }

        private DataTable GetDataTable(string queryString, IDbTransaction trans)
        {
            return this.GetDataTable(queryString, trans, null);
        }

        public DataTable GetDataTable(string queryString, DataBaseParameter[] parameters)
        {
            if (this.dbTransaction != null && this.dbTransaction.Connection.State == ConnectionState.Open)
            {
                return this.GetDataTable(queryString, this.dbTransaction, parameters);
            }

            if (this.dbConnection.State == ConnectionState.Closed)
            {
                this.dbConnection.Open();
            }

            this.lastCommand = this.dbConnection.CreateCommand(queryString, parameters);
            return this.ReaderToDataTable(this.lastCommand.ExecuteReader());
        }

        private DataTable GetDataTable(string queryString, IDbTransaction trans, DataBaseParameter[] parameters)
        {
            if (trans == null)
            {
                return this.GetDataTable(queryString, parameters);
            }

            this.lastCommand = trans.Connection.CreateCommand(queryString, parameters);
            this.lastCommand.Transaction = trans;
            return this.ReaderToDataTable(this.lastCommand.ExecuteReader());
        }

        private DataTable ReaderToDataTable(IDataReader reader)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                DataColumn col = new DataColumn(reader.GetName(i), reader.GetFieldType(i));
                dt.Columns.Add(col);
            }

            while (reader.Read())
            {
                DataRow row = dt.NewRow();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[i] = reader[i];
                }

                dt.Rows.Add(row);
            }

            reader.Close();

            return dt;
        }
        #endregion

        #region GetDataRow

        public DataRow GetDataRow(string queryString)
        {
            DataTable dt = this.GetDataTable(queryString);
            if (dt.Rows.Count == 1)
            {
                return dt.Rows[0];
            }
            else
            {
                return null;
            }
        }

        public DataRow GetDataRow(string queryString, DataBaseParameter[] parameters)
        {
            DataTable dt = this.GetDataTable(queryString, parameters);
            if (dt.Rows.Count == 1)
            {
                return dt.Rows[0];
            }
            else
            {
                return null;
            }
        }

        private DataRow GetDataRow(string queryString, IDbTransaction trans)
        {
            DataTable dt = this.GetDataTable(queryString, trans);
            if (dt.Rows.Count == 1)
            {
                return dt.Rows[0];
            }
            else
            {
                return null;
            }
        }

        private DataRow GetDataRow(string queryString, IDbTransaction trans, DataBaseParameter[] parameters)
        {
            DataTable dt = this.GetDataTable(queryString, trans, parameters);
            if (dt.Rows.Count == 1)
            {
                return dt.Rows[0];
            }
            else
            {
                return null;
            }
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
