using System;
using System.Collections.Generic;
using System.Data;

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

        public T SafeRunQuery<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<IDbCommand, T> executionFunc) where T : class
        {
            return this.SafeRunQuery<T>(queryString, parameters, executionFunc, this.options.AutoCloseConnection);
        }

        public T SafeRunQuery<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<IDbCommand, T> executionFunc, bool closeConnection) where T : class
        {
            T output = null;

            this.connectionBuilder.OpenConnection(delegate (IDbConnection conn, IDbTransaction trans)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                using (IDbCommand command = conn.CreateCommand(queryString, this.queryPreProcess.PreProcessParameters(queryString, parameters)))
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
    }
}