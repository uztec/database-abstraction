using System;
using System.Collections.Generic;
using System.Data;
using UzunTec.Utils.Common;
using UzunTec.Utils.DatabaseAbstraction.Pagination;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public class DbQueryBase : IDbQueryBase
    {
        private IDbTransaction dbTransaction;
        private readonly IDbConnection dbConnection;
        private readonly IPaginationFactory paginationFactory;

        public DatabaseDialect Dialect { get; }

        public DbQueryBase(IDbConnection connection, string engine = null)
            : this(connection, EnumUtils.GetEnumValue<DatabaseDialect>(engine) ?? DatabaseDialect.NotSet) { }

        public DbQueryBase(IDbConnection connection, DatabaseDialect dialect)
        {
            this.dbConnection = connection;
            this.Dialect = dialect;
            this.paginationFactory = PaginationAbstractFactory.GetObject(dialect);
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

        private readonly object queryLocking = new object();

        private T SafeRunQuery<T>(IDbConnection conn, string queryString, IEnumerable<DataBaseParameter> parameters, Func<IDbCommand, T> executionFunc) where T : class
        {
            lock (this.queryLocking)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                T output = null;
                using (IDbCommand command = conn.CreateCommand(queryString, parameters))
                {
                    output = executionFunc(command);
                }
                return output;
            }
        }

        #region Pagination Engine
        public DataResultTable GetLimitedRecords(string queryString, int offset, int count)
        {
            return this.GetLimitedRecords(queryString, new DataBaseParameter[0], offset, count);
        }

        public DataResultTable GetLimitedRecords(string queryString, IEnumerable<DataBaseParameter> parameters, int offset, int count)
        {
            offset = (offset < 0) ? 0 : offset;
            count = (count < 1) ? 1 : count;
            string paginatedQueryString = this.paginationFactory.AddPagination(queryString, offset, count);
            return this.GetResultTable(paginatedQueryString, parameters);
        }

        public DataResultTable GetPagedResultTable(string queryString, int page, int pageSize)
        {
            return this.GetPagedResultTable(queryString, new DataBaseParameter[0], page, pageSize);
        }

        public DataResultTable GetPagedResultTable(string queryString, IEnumerable<DataBaseParameter> parameters, int page, int pageSize)
        {
            page = (page < 1) ? 1 : page;
            pageSize = (pageSize < 1) ? 1 : pageSize;
            int offset = (page - 1) * pageSize;
            string paginatedQueryString = this.paginationFactory.AddPagination(queryString, offset, pageSize);
            return this.GetResultTable(paginatedQueryString, parameters);
        }
        #endregion


        #region GetResultTable

        public DataResultTable GetResultTable(string queryString)
        {
            return this.GetResultTable(queryString, new DataBaseParameter[0]);
        }

        public DataResultTable GetResultTable(string queryString, int limit)
        {
            return this.GetResultTable(this.paginationFactory.AddLimit(queryString, limit));
        }

        public DataResultTable GetResultTable(string queryString, IEnumerable<DataBaseParameter> parameters, int limit)
        {
            return this.GetResultTable(this.paginationFactory.AddLimit(queryString, limit), parameters);
        }

        public DataResultTable GetResultTable(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            if (this.dbTransaction != null && this.dbTransaction.Connection.State == ConnectionState.Open)
            {
                return this.GetResultTable(queryString, this.dbTransaction, parameters);
            }

            return this.SafeRunQuery(this.dbConnection, queryString, parameters, delegate (IDbCommand command)
            {
                return new DataResultTable(command.ExecuteReader());
            });
        }

        private DataResultTable GetResultTable(string queryString, IDbTransaction trans, IEnumerable<DataBaseParameter> parameters)
        {
            if (trans == null)
            {
                return this.GetResultTable(queryString, parameters);
            }

            return this.SafeRunQuery(trans.Connection, queryString, parameters, delegate (IDbCommand command)
            {
                command.Transaction = trans;
                return new DataResultTable(command.ExecuteReader());
            });
        }

        public DataResultTable GetResultTableFromProcedure(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            if (this.dbTransaction != null && this.dbTransaction.Connection.State == ConnectionState.Open)
            {
                return this.GetResultTableFromProcedure(queryString, this.dbTransaction, parameters);
            }

            return this.SafeRunQuery(this.dbConnection, queryString, parameters, delegate (IDbCommand command)
            {
                command.CommandType = CommandType.StoredProcedure;
                return new DataResultTable(command.ExecuteReader());
            });
        }

        private DataResultTable GetResultTableFromProcedure(string queryString, IDbTransaction trans, IEnumerable<DataBaseParameter> parameters)
        {
            if (trans == null)
            {
                return this.GetResultTableFromProcedure(queryString, parameters);
            }

            return this.SafeRunQuery(trans.Connection, queryString, parameters, delegate (IDbCommand command)
            {
                command.Transaction = trans;
                command.CommandType = CommandType.StoredProcedure;
                return new DataResultTable(command.ExecuteReader());
            });
        }

        private DataResultTable GetResultTableWithSingleRow(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            if (this.dbTransaction != null && this.dbTransaction.Connection.State == ConnectionState.Open)
            {
                return this.GetResultTableFromProcedure(queryString, this.dbTransaction, parameters);
            }

            return this.SafeRunQuery(this.dbConnection, queryString, parameters, delegate (IDbCommand command)
            {
                return new DataResultTable(command.ExecuteReader(CommandBehavior.SingleRow));
            });
        }

        #endregion

        #region GetSingleRecord

        public DataResultRecord GetSingleRecord(string queryString)
        {
            return this.GetResultTableWithSingleRow(queryString, new DataBaseParameter[0]).SingleRecord();
        }

        public DataResultRecord GetSingleRecord(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return this.GetResultTableWithSingleRow(queryString, parameters).SingleRecord();
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



        /// <summary>
        /// To execute INSERT, UPDATE or DELETE queries with params
        /// </summary>
        public int ExecuteNonQuery(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            if (this.dbTransaction != null && this.dbTransaction.Connection.State == ConnectionState.Open)
            {
                return this.ExecuteNonQuery(queryString, this.dbTransaction, parameters);
            }

            return (int)this.SafeRunQuery<object>(this.dbConnection, queryString, parameters, delegate (IDbCommand command)
            {
                return command.ExecuteNonQuery();
            });

        }

        /// <summary>
        /// To execute INSERT, UPDATE or DELETE queries with transaction
        /// </summary>
        private int ExecuteNonQuery(string queryString, IDbTransaction trans, IEnumerable<DataBaseParameter> parameters)
        {
            if (trans == null)
            {
                return this.ExecuteNonQuery(queryString, parameters);
            }

            return (int)this.SafeRunQuery<object>(trans.Connection, queryString, parameters, delegate (IDbCommand command)
            {
                command.Transaction = trans;
                return command.ExecuteNonQuery();
            });

        }

        #endregion

        #region ExecuteScalar

        public object ExecuteScalar(string queryString)
        {
            return this.ExecuteScalar(queryString, new DataBaseParameter[0]);
        }

        public object ExecuteScalar(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            if (this.dbTransaction != null && this.dbTransaction.Connection.State == ConnectionState.Open)
            {
                return this.ExecuteScalar(queryString, this.dbTransaction, parameters);
            }

            return this.SafeRunQuery(this.dbConnection, queryString, parameters, delegate (IDbCommand command)
            {
                return command.ExecuteScalar();
            });
        }

        private object ExecuteScalar(string queryString, IDbTransaction trans, IEnumerable<DataBaseParameter> parameters)
        {
            if (trans == null)
            {
                return this.ExecuteScalar(queryString, parameters);
            }

            return this.SafeRunQuery(trans.Connection, queryString, parameters, delegate (IDbCommand command)
            {
                command.Transaction = trans;
                return command.ExecuteScalar();
            });
        }

        #endregion
    }
}
