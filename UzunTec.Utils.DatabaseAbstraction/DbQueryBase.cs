using System.Collections.Generic;
using System.Data;
using UzunTec.Utils.DatabaseAbstraction.Pagination;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public class DbQueryBase : IDbQueryBase
    {
        private readonly IPaginationFactory paginationFactory;
        private readonly IQueryExecutionLayer exec;
        private readonly ConnectionBuilder connectionBuilder;

        public AbstractionOptions Options { get; }

        public DbQueryBase(ConnectionBuilder connectionBuilder, AbstractionOptions options)
        {
            this.connectionBuilder = connectionBuilder;
            this.Options = options;
            this.paginationFactory = PaginationAbstractFactory.GetObject(options.Dialect);
            this.exec = QueryExecutionLayerBuilder.Build(options, connectionBuilder);
        }

        public DbQueryBase(ConnectionBuilder connectionBuilder, string engine = null)
            : this(connectionBuilder, DefaultDialectOptions.GetDefaultOptions(engine)) { }

        public DbQueryBase(ConnectionBuilder connectionBuilder, DatabaseDialect dialect)
            : this(connectionBuilder, DefaultDialectOptions.GetDefaultOptions(dialect)) { }

        public DbQueryBase(IDbConnection connection, string engine = null)
            : this(new ConnectionBuilder(connection), DefaultDialectOptions.GetDefaultOptions(engine)) { }

        public DbQueryBase(IDbConnection connection, DatabaseDialect dialect)
            : this(new ConnectionBuilder(connection), DefaultDialectOptions.GetDefaultOptions(dialect)) { }

        public DbQueryBase(IDbConnection connection, AbstractionOptions options)
            : this(new ConnectionBuilder(connection), options) { }


        #region Transaction
        /// <summary>
        /// Start DB Transaction
        /// </summary>
        public void BeginTransaction()
        {
            this.connectionBuilder.BeginTransaction();
        }

        /// <summary>
        /// Finalize and Commit DB Transaction
        /// </summary>
        public void CommitTransaction()
        {
            this.connectionBuilder.CommitTransaction();
        }

        /// <summary>
        /// Finalize but Rollback DB Transaction
        /// </summary>
        public void RollbackTransaction()
        {
            this.connectionBuilder.RollbackTransaction();
        }
        #endregion



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
            return this.exec.SafeRunQuery(queryString, parameters, delegate (IDbCommand command)
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

            return this.exec.SafeRunQuery(queryString, parameters, delegate (IDbCommand command)
            {
                command.Transaction = trans;
                return new DataResultTable(command.ExecuteReader());
            });
        }

        public DataResultTable GetResultTableFromProcedure(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return this.exec.SafeRunQuery(queryString, parameters, delegate (IDbCommand command)
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

            return this.exec.SafeRunQuery(queryString, parameters, delegate (IDbCommand command)
            {
                command.Transaction = trans;
                command.CommandType = CommandType.StoredProcedure;
                return new DataResultTable(command.ExecuteReader());
            });
        }

        private DataResultTable GetResultTableWithSingleRow(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return this.exec.SafeRunQuery(queryString, parameters, delegate (IDbCommand command)
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
            return (int)this.exec.SafeRunQuery<object>(queryString, parameters, delegate (IDbCommand command)
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

            return (int)this.exec.SafeRunQuery<object>(queryString, parameters, delegate (IDbCommand command)
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
            return this.exec.SafeRunQuery(queryString, parameters, delegate (IDbCommand command)
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

            return this.exec.SafeRunQuery(queryString, parameters, delegate (IDbCommand command)
            {
                command.Transaction = trans;
                return command.ExecuteScalar();
            });
        }

        #endregion
    }
}
