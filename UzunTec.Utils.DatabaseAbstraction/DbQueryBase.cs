using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
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
            : this(connectionBuilder, DefaultDialectOptions.GetDefaultOptions(engine, true)) { }

        public DbQueryBase(ConnectionBuilder connectionBuilder, DatabaseDialect dialect)
            : this(connectionBuilder, DefaultDialectOptions.GetDefaultOptions(dialect, true)) { }

        public DbQueryBase(DbConnection connection, string engine = null)
            : this(new ConnectionBuilder(connection), DefaultDialectOptions.GetDefaultOptions(engine, false)) { }

        public DbQueryBase(DbConnection connection, DatabaseDialect dialect)
            : this(new ConnectionBuilder(connection), DefaultDialectOptions.GetDefaultOptions(dialect, false)) { }

        public DbQueryBase(DbConnection connection, AbstractionOptions options)
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

        public Task<DataResultTable> GetLimitedRecordsAsync(string queryString, int offset, int count)
        {
            return this.GetLimitedRecordsAsync(queryString, new DataBaseParameter[0], offset, count);
        }

        public DataResultTable GetLimitedRecords(string queryString, IEnumerable<DataBaseParameter> parameters, int offset, int count)
        {
            offset = (offset < 0) ? 0 : offset;
            count = (count < 1) ? 1 : count;
            string paginatedQueryString = this.paginationFactory.AddPagination(queryString, offset, count);
            return this.GetResultTable(paginatedQueryString, parameters);
        }

        public Task<DataResultTable> GetLimitedRecordsAsync(string queryString, IEnumerable<DataBaseParameter> parameters, int offset, int count)
        {
            offset = (offset < 0) ? 0 : offset;
            count = (count < 1) ? 1 : count;
            string paginatedQueryString = this.paginationFactory.AddPagination(queryString, offset, count);
            return this.GetResultTableAsync(paginatedQueryString, parameters);
        }


        public DataResultTable GetPagedResultTable(string queryString, int page, int pageSize)
        {
            return this.GetPagedResultTable(queryString, new DataBaseParameter[0], page, pageSize);
        }
        public Task<DataResultTable> GetPagedResultTableAsync(string queryString, int page, int pageSize)
        {
            return this.GetPagedResultTableAsync(queryString, new DataBaseParameter[0], page, pageSize);
        }


        public DataResultTable GetPagedResultTable(string queryString, IEnumerable<DataBaseParameter> parameters, int page, int pageSize)
        {
            page = (page < 1) ? 1 : page;
            pageSize = (pageSize < 1) ? 1 : pageSize;
            int offset = (page - 1) * pageSize;
            string paginatedQueryString = this.paginationFactory.AddPagination(queryString, offset, pageSize);
            return this.GetResultTable(paginatedQueryString, parameters);
        }

        public Task<DataResultTable> GetPagedResultTableAsync(string queryString, IEnumerable<DataBaseParameter> parameters, int page, int pageSize)
        {
            page = (page < 1) ? 1 : page;
            pageSize = (pageSize < 1) ? 1 : pageSize;
            int offset = (page - 1) * pageSize;
            string paginatedQueryString = this.paginationFactory.AddPagination(queryString, offset, pageSize);
            return this.GetResultTableAsync(paginatedQueryString, parameters);
        }
        #endregion


        #region GetResultTable
        public DataResultTable GetResultTable(string queryString)
        {
            return this.GetResultTable(queryString, new DataBaseParameter[0]);
        }
        public Task<DataResultTable> GetResultTableAsync(string queryString)
        {
            return this.GetResultTableAsync(queryString, new DataBaseParameter[0]);
        }

        public DataResultTable GetResultTable(string queryString, int limit)
        {
            return this.GetResultTable(this.paginationFactory.AddLimit(queryString, limit));
        }

        public Task<DataResultTable> GetResultTableAsync(string queryString, int limit)
        {
            return this.GetResultTableAsync(this.paginationFactory.AddLimit(queryString, limit));
        }

        public DataResultTable GetResultTable(string queryString, IEnumerable<DataBaseParameter> parameters, int limit)
        {
            return this.GetResultTable(this.paginationFactory.AddLimit(queryString, limit), parameters);
        }

        public Task<DataResultTable> GetResultTableAsync(string queryString, IEnumerable<DataBaseParameter> parameters, int limit)
        {
            return this.GetResultTableAsync(this.paginationFactory.AddLimit(queryString, limit), parameters);
        }

        public DataResultTable GetResultTable(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return this.exec.SafeRunQuery(queryString, parameters, delegate (DbCommand command)
            {
                return new DataResultTable(command.ExecuteReader());
            });
        }

        public Task<DataResultTable> GetResultTableAsync(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return this.exec.SafeRunQueryAsync(queryString, parameters, async delegate (DbCommand command)
            {
                DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                return new DataResultTable(reader);
            });
        }

        public DataResultTable GetResultTable(string queryString, DbTransaction trans, IEnumerable<DataBaseParameter> parameters)
        {
            if (trans == null)
            {
                return this.GetResultTable(queryString, parameters);
            }

            return this.exec.SafeRunQuery(queryString, parameters, delegate (DbCommand command)
            {
                command.Transaction = trans;
                return new DataResultTable(command.ExecuteReader());
            });
        }

        public Task<DataResultTable> GetResultTableAsync(string queryString, DbTransaction trans, IEnumerable<DataBaseParameter> parameters)
        {
            if (trans == null)
            {
                return this.GetResultTableAsync(queryString, parameters);
            }

            return this.exec.SafeRunQueryAsync(queryString, parameters, async delegate (DbCommand command)
            {
                command.Transaction = trans;
                DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                return new DataResultTable(reader);
            });
        }

        public DataResultTable GetResultTableFromProcedure(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return this.exec.SafeRunQuery(queryString, parameters, delegate (DbCommand command)
            {
                command.CommandType = CommandType.StoredProcedure;
                return new DataResultTable(command.ExecuteReader());
            });
        }

        public Task<DataResultTable> GetResultTableFromProcedureAsync(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return this.exec.SafeRunQueryAsync(queryString, parameters, async delegate (DbCommand command)
            {
                command.CommandType = CommandType.StoredProcedure;
                DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                return new DataResultTable(reader);
            });
        }

        public DataResultTable GetResultTableFromProcedure(string queryString, DbTransaction trans, IEnumerable<DataBaseParameter> parameters)
        {
            if (trans == null)
            {
                return this.GetResultTableFromProcedure(queryString, parameters);
            }

            return this.exec.SafeRunQuery(queryString, parameters, delegate (DbCommand command)
            {
                command.Transaction = trans;
                command.CommandType = CommandType.StoredProcedure;
                return new DataResultTable(command.ExecuteReader());
            });
        }

        public Task<DataResultTable> GetResultTableFromProcedureAsync(string queryString, DbTransaction trans, IEnumerable<DataBaseParameter> parameters)
        {
            if (trans == null)
            {
                return this.GetResultTableFromProcedureAsync(queryString, parameters);
            }

            return this.exec.SafeRunQueryAsync(queryString, parameters, async delegate (DbCommand command)
            {
                command.Transaction = trans;
                command.CommandType = CommandType.StoredProcedure;
                DbDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                return new DataResultTable(reader);
            });
        }


        private DataResultTable GetResultTableWithSingleRow(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return this.exec.SafeRunQuery(queryString, parameters, delegate (DbCommand command)
            {
                return new DataResultTable(command.ExecuteReader(CommandBehavior.SingleRow));
            });
        }

        private Task<DataResultTable> GetResultTableWithSingleRowAsync(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return this.exec.SafeRunQueryAsync(queryString, parameters, async delegate (DbCommand command)
            {
                DbDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow).ConfigureAwait(false);
                return new DataResultTable(reader);
            });
        }


        #endregion

        #region GetSingleRecord

        public DataResultRecord GetSingleRecord(string queryString)
        {
            return this.GetResultTableWithSingleRow(queryString, new DataBaseParameter[0]).SingleRecord();
        }

        public async Task<DataResultRecord> GetSingleRecordAsync(string queryString)
        {
            var dt = await this.GetResultTableWithSingleRowAsync(queryString, new DataBaseParameter[0]).ConfigureAwait(false);
            return dt.SingleRecord();
        }

        public DataResultRecord GetSingleRecord(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return this.GetResultTableWithSingleRow(queryString, parameters).SingleRecord();
        }

        public async Task<DataResultRecord> GetSingleRecordAsync(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            var dt = await this.GetResultTableWithSingleRowAsync(queryString, parameters).ConfigureAwait(false);
            return dt.SingleRecord();
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
        /// To execute INSERT, UPDATE or DELETE queries async
        /// </summary>
        public Task<int> ExecuteNonQueryAsync(string queryString)
        {
            return this.ExecuteNonQueryAsync(queryString, new DataBaseParameter[0]);
        }

        /// <summary>
        /// To execute INSERT, UPDATE or DELETE queries with params
        /// </summary>
        public int ExecuteNonQuery(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return (int)this.exec.SafeRunQuery<object>(queryString, parameters, delegate (DbCommand command)
            {
                return command.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// To execute INSERT, UPDATE or DELETE queries async with params
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return (int) await this.exec.SafeRunQueryAsync<object>(queryString, parameters, async delegate (DbCommand command)
            {
                return await command.ExecuteNonQueryAsync().ConfigureAwait(false);

            }).ConfigureAwait(false);
        }

        /// <summary>
        /// To execute INSERT, UPDATE or DELETE queries with transaction
        /// </summary>
        public int ExecuteNonQuery(string queryString, DbTransaction trans, IEnumerable<DataBaseParameter> parameters)
        {
            if (trans == null)
            {
                return this.ExecuteNonQuery(queryString, parameters);
            }

            return (int)this.exec.SafeRunQuery<object>(queryString, parameters, delegate (DbCommand command)
            {
                command.Transaction = trans;
                return command.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// To execute INSERT, UPDATE or DELETE queries async with transaction
        /// </summary>
        public async Task<int> ExecuteNonQueryAsync(string queryString, DbTransaction trans, IEnumerable<DataBaseParameter> parameters)
        {
            if (trans == null)
            {
                return await this.ExecuteNonQueryAsync(queryString, parameters).ConfigureAwait(false);
            }

            return (int) await this.exec.SafeRunQueryAsync<object>(queryString, parameters, async delegate (DbCommand command)
            {
                command.Transaction = trans;
                return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);
        }


        #endregion

        #region ExecuteScalar

        public object ExecuteScalar(string queryString)
        {
            return this.ExecuteScalar(queryString, new DataBaseParameter[0]);
        }

        public Task<object> ExecuteScalarAsync(string queryString)
        {
            return this.ExecuteScalarAsync(queryString, new DataBaseParameter[0]);
        }

        public object ExecuteScalar(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return this.exec.SafeRunQuery(queryString, parameters, delegate (DbCommand command)
            {
                return command.ExecuteScalar();
            });
        }

        public Task<object> ExecuteScalarAsync(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            return this.exec.SafeRunQueryAsync(queryString, parameters, async delegate (DbCommand command)
            {
                return await command.ExecuteScalarAsync();
            });
        }


        public object ExecuteScalar(string queryString, DbTransaction trans, IEnumerable<DataBaseParameter> parameters)
        {
            if (trans == null)
            {
                return this.ExecuteScalar(queryString, parameters);
            }

            return this.exec.SafeRunQuery(queryString, parameters, delegate (DbCommand command)
            {
                command.Transaction = trans;
                return command.ExecuteScalar();
            });
        }

        public Task<object> ExecuteScalarAsync(string queryString, DbTransaction trans, IEnumerable<DataBaseParameter> parameters)
        {
            if (trans == null)
            {
                return this.ExecuteScalarAsync(queryString, parameters);
            }

            return this.exec.SafeRunQueryAsync(queryString, parameters, async delegate (DbCommand command)
            {
                command.Transaction = trans;
                return await command.ExecuteScalarAsync().ConfigureAwait(false);
            });
        }


        #endregion
    }
}
