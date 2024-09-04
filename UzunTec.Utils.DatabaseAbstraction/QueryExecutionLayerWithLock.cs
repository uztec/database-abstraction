using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace UzunTec.Utils.DatabaseAbstraction
{
    internal class QueryExecutionLayerWithLock : IQueryExecutionLayer
    {
        private readonly object queryLocking = new object();
        private readonly QueryExecutionLayer baseLayer;

        public QueryExecutionLayerWithLock(AbstractionOptions options, QueryPreProccess queryPreProcess, ConnectionBuilder connectionBuilder)
        {
            this.baseLayer = new QueryExecutionLayer(options, queryPreProcess, connectionBuilder);
        }

        public T SafeRunQuery<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<DbCommand, T> executionFunc) where T : class
        {
            lock (this.queryLocking)
            {
                return this.baseLayer.SafeRunQuery<T>(queryString, parameters, executionFunc);
            }
        }

        public T SafeRunQuery<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<DbCommand, T> executionFunc, bool closeConnection) where T : class
        {
            lock (this.queryLocking)
            {
                return this.baseLayer.SafeRunQuery<T>(queryString, parameters, executionFunc, closeConnection);
            }
        }

        public Task<T> SafeRunQueryAsync<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<DbCommand, Task<T>> executionFunc) where T : class
        {
            lock (this.queryLocking)
            {
                return this.baseLayer.SafeRunQueryAsync<T>(queryString, parameters, executionFunc);
            }
        }

        public Task<T> SafeRunQueryAsync<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<DbCommand, Task<T>> executionFunc, bool closeConnection) where T : class
        {
            lock (this.queryLocking)
            {
                return this.baseLayer.SafeRunQueryAsync<T>(queryString, parameters, executionFunc, closeConnection);
            }
        }
    }
}
