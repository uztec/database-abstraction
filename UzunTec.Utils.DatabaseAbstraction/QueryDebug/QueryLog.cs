using System;
using System.Collections.Generic;

namespace UzunTec.Utils.DatabaseAbstraction.QueryDebug
{
    public class QueryLog
    {
        private static readonly List<QueryLog> queryLogs = new List<QueryLog>();
        public static IReadOnlyList<QueryLog> QueryLogs => queryLogs;

        public string Query { get; set; } = string.Empty;
        public IEnumerable<DataBaseParameter> Parameters { get; set; } = new List<DataBaseParameter>();
        public double QueryTimeInMs { get; set; } = 0;
        public object QueryResult { get; set; } = null;
        public DateTime QueryTime { get; set; } = DateTime.MinValue;
        public int Limit { get; set; } = -1;
        public int Offset { get; set; } = -1;

        public static void Log(string query, IEnumerable<DataBaseParameter> parameters, double time, object result, int limit = -1, int offset = -1)
        {
            queryLogs.Add(new QueryLog 
            { 
                Query = query,
                Parameters = parameters, 
                QueryTimeInMs = time, 
                QueryResult = result,
                QueryTime = DateTime.Now,
                Limit = limit,
                Offset = offset
            });
        }
    }
}
