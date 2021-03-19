using System;
using System.Collections.Generic;
using System.Data;

namespace UzunTec.Utils.DatabaseAbstraction
{
    internal class QueryExecutionLayerWithLock : IQueryExecutionLayer
    {
        private readonly object queryLocking = new object();
        private readonly QueryExecutionLayer baseLayer;

        internal QueryExecutionLayerWithLock(AbstractionOptions options, QueryPreProccess queryPreProcess)
        {
            this.baseLayer = new QueryExecutionLayer(options, queryPreProcess);
        }

        public T SafeRunQuery<T>(IDbConnection conn, string queryString, IEnumerable<DataBaseParameter> parameters, Func<IDbCommand, T> executionFunc) where T : class
        {
            return this.baseLayer.SafeRunQuery<T>(conn, queryString, parameters, executionFunc);
        }

        public T SafeRunQuery<T>(IDbConnection conn, string queryString, IEnumerable<DataBaseParameter> parameters, Func<IDbCommand, T> executionFunc, bool closeConnection) where T : class
        {
            lock (this.queryLocking)
            {
                return this.baseLayer.SafeRunQuery<T>(conn, queryString, parameters, executionFunc, closeConnection);
            }
        }
    }
}
