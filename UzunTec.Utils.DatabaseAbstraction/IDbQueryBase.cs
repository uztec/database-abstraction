using System.Collections.Generic;
using System.Threading.Tasks;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public interface IDbQueryBase
    {
        AbstractionOptions Options { get; }
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        DataResultTable GetResultTable(string queryString);
        DataResultTable GetResultTable(string queryString, IEnumerable<DataBaseParameter> parameters);
        DataResultTable GetResultTableFromProcedure(string queryString, IEnumerable<DataBaseParameter> parameters);
        DataResultRecord GetSingleRecord(string queryString);
        DataResultRecord GetSingleRecord(string queryString, IEnumerable<DataBaseParameter> parameters);
        int ExecuteNonQuery(string queryString);
        int ExecuteNonQuery(string queryString, IEnumerable<DataBaseParameter> parameters);
        object ExecuteScalar(string queryString);
        object ExecuteScalar(string queryString, IEnumerable<DataBaseParameter> parameters);


        // Pagination
        DataResultTable GetResultTable(string queryString, int limit);
        DataResultTable GetResultTable(string queryString, IEnumerable<DataBaseParameter> parameters, int limit);
        DataResultTable GetLimitedRecords(string queryString, int offset, int count);
        DataResultTable GetLimitedRecords(string queryString, IEnumerable<DataBaseParameter> parameters, int offset, int count);
        DataResultTable GetPagedResultTable(string queryString, int page, int pageSize);
        DataResultTable GetPagedResultTable(string queryString, IEnumerable<DataBaseParameter> parameters, int page, int pageSize);


        // Async Operations
        Task<DataResultTable> GetResultTableAsync(string queryString);
        Task<DataResultTable> GetResultTableAsync(string queryString, IEnumerable<DataBaseParameter> parameters);
        Task<DataResultTable> GetResultTableFromProcedureAsync(string queryString, IEnumerable<DataBaseParameter> parameters);
        Task<DataResultRecord> GetSingleRecordAsync(string queryString);
        Task<DataResultRecord> GetSingleRecordAsync(string queryString, IEnumerable<DataBaseParameter> parameters);
        Task<int> ExecuteNonQueryAsync(string queryString);
        Task<int> ExecuteNonQueryAsync(string queryString, IEnumerable<DataBaseParameter> parameters);
        Task<object> ExecuteScalarAsync(string queryString);
        Task<object> ExecuteScalarAsync(string queryString, IEnumerable<DataBaseParameter> parameters);


        // Pagination
        Task<DataResultTable> GetResultTableAsync(string queryString, int limit);
        Task<DataResultTable> GetResultTableAsync(string queryString, IEnumerable<DataBaseParameter> parameters, int limit);
        Task<DataResultTable> GetLimitedRecordsAsync(string queryString, int offset, int count);
        Task<DataResultTable> GetLimitedRecordsAsync(string queryString, IEnumerable<DataBaseParameter> parameters, int offset, int count);
        Task<DataResultTable> GetPagedResultTableAsync(string queryString, int page, int pageSize);
        Task<DataResultTable> GetPagedResultTableAsync(string queryString, IEnumerable<DataBaseParameter> parameters, int page, int pageSize);









    }
}