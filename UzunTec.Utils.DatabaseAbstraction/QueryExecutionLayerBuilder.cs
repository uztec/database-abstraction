using System;
using System.Collections.Generic;
using System.Text;

namespace UzunTec.Utils.DatabaseAbstraction
{
    internal static class QueryExecutionLayerBuilder
    {
        public static IQueryExecutionLayer Build(AbstractionOptions options)
        {
            QueryPreProccess queryPreProcess = new QueryPreProccess(options);

            if (options.AllowMultipleConnections)
            {
                return new QueryExecutionLayerMultipleConnections(options, queryPreProcess);
            }
            else
            {
                return new QueryExecutionLayerSingleConnection(options, queryPreProcess);
            }
        }
    }
}
