using System;
using System.Collections.Generic;
using System.Data;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public interface IQueryExecutionLayer
    {
        T SafeRunQuery<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<IDbCommand, T> executionFunc) where T : class;
        T SafeRunQuery<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<IDbCommand, T> executionFunc, bool closeConnection = false) where T : class;
    }
}