using System.Collections.Generic;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public interface IDbQueryBase
    {
        DatabaseDialect Dialect { get; }

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
    }
}