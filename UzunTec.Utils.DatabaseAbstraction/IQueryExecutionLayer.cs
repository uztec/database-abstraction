using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public interface IQueryExecutionLayer
    {
        T SafeRunQuery<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<DbCommand, T> executionFunc) where T : class;
        T SafeRunQuery<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<DbCommand, T> executionFunc, bool closeConnection = false) where T : class;
        Task<T> SafeRunQueryAsync<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<DbCommand, Task<T>> executionFunc) where T : class;
        Task<T> SafeRunQueryAsync<T>(string queryString, IEnumerable<DataBaseParameter> parameters, Func<DbCommand, Task<T>> executionFunc, bool closeConnection = false) where T : class;

    }
}