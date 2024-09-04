using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace UzunTec.Utils.DatabaseAbstraction
{
    internal class QueryExecutionLayer : IQueryExecutionLayer
    {
        private readonly AbstractionOptions options;
        private readonly QueryPreProccess queryPreProcess;
        private readonly ConnectionBuilder connectionBuilder;

        public QueryExecutionLayer(AbstractionOptions options, QueryPreProccess queryPreProcess, ConnectionBuilder connectionBuilder)
        {
            this.options = options;
            this.queryPreProcess = queryPreProcess;
            this.connectionBuilder = connectionBuilder;
        }

        public T SafeRunQuery<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<DbCommand, T> executionFunc) where T : class
        {
            return this.SafeRunQuery<T>(queryString, parameters, executionFunc, this.options.AutoCloseConnection);
        }

        public T SafeRunQuery<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<DbCommand, T> executionFunc, bool closeConnection) where T : class
        {
            T output = null;

            this.connectionBuilder.OpenConnection(delegate (DbConnection conn, DbTransaction trans)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                using (DbCommand command = conn.CreateCommand(queryString, this.queryPreProcess.PreProcessParameters(queryString, parameters)))
                {
                    command.Transaction = trans;
                    command.CommandText = this.queryPreProcess.PreProcessQuery(command.CommandText);
                    output = executionFunc(command);
                }

                if (closeConnection)
                {
                    conn.Close();
                }
            });

            return output;
        }

        public Task<T> SafeRunQueryAsync<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<DbCommand, Task<T>> executionFunc) where T : class
        {
            return this.SafeRunQueryAsync<T>(queryString, parameters, executionFunc, this.options.AutoCloseConnection);
        }

        public async Task<T> SafeRunQueryAsync<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<DbCommand, Task<T>> executionFunc, bool closeConnection) where T : class
        {
            T output = null;

            await this.connectionBuilder.OpenConnectionAsync(async delegate (DbConnection conn, DbTransaction trans)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    await conn.OpenAsync().ConfigureAwait(false);
                }

                using (DbCommand command = conn.CreateCommand(queryString, this.queryPreProcess.PreProcessParameters(queryString, parameters)))
                {
                    command.Transaction = trans;
                    command.CommandText = this.queryPreProcess.PreProcessQuery(command.CommandText);
                    output = await executionFunc(command).ConfigureAwait(false);
                }

                if (closeConnection)
                {
                    conn.Close();
                }
            });

            return output;
        }
    }
}