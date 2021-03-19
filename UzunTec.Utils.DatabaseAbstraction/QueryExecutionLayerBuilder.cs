namespace UzunTec.Utils.DatabaseAbstraction
{
    internal static class QueryExecutionLayerBuilder
    {
        public static IQueryExecutionLayer Build(AbstractionOptions options)
        {
            QueryPreProccess queryPreProcess = new QueryPreProccess(options);

            if (options.UseLockedCommands)
            {
                return new QueryExecutionLayerWithLock(options, queryPreProcess);
            }
            else
            {
                return new QueryExecutionLayer(options, queryPreProcess);
            }
        }
    }
}
