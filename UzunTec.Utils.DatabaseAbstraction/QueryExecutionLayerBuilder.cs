namespace UzunTec.Utils.DatabaseAbstraction
{
    internal static class QueryExecutionLayerBuilder
    {
        public static IQueryExecutionLayer Build(AbstractionOptions options, ConnectionBuilder connectionBuilder)
        {
            QueryPreProccess queryPreProcess = new QueryPreProccess(options);

            if (options.UseLockedCommands)
            {
                return new QueryExecutionLayerWithLock(options, queryPreProcess, connectionBuilder);
            }
            else
            {
                return new QueryExecutionLayer(options, queryPreProcess, connectionBuilder);
            }
        }
    }
}
