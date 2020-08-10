using System;
using System.Collections.Generic;
using System.Data;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public interface IQueryExecutionLayer
    {
        T SafeRunQuery<T>(IDbConnection conn, string queryString, IEnumerable<DataBaseParameter> parameters, Func<IDbCommand, T> executionFunc) where T : class;
        T SafeRunQuery<T>(IDbConnection conn, string queryString, IEnumerable<DataBaseParameter> parameters, Func<IDbCommand, T> executionFunc, bool closeConnection) where T : class;
    }
}