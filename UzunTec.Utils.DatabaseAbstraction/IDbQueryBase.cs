namespace UzunTec.Utils.DatabaseAbstraction
{
    public interface IDbQueryBase
    {
        DatabaseDialect Dialect { get; }

        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        DataResultTable GetResultTable(string queryString);
        DataResultTable GetResultTable(string queryString, DataBaseParameter[] parameters);
        DataResultTable GetResultTableFromProcedure(string queryString, DataBaseParameter[] parameters);
        DataResultRecord GetSingleRecord(string queryString);
        DataResultRecord GetSingleRecord(string queryString, DataBaseParameter[] parameters);
        int ExecuteNonQuery(string queryString);
        int ExecuteNonQuery(string queryString, DataBaseParameter[] parameters);
        object ExecuteScalar(string queryString);
        object ExecuteScalar(string queryString, DataBaseParameter[] parameters);


        // Pagination
        DataResultTable GetResultTable(string queryString, int limit);
        DataResultTable GetResultTable(string queryString, DataBaseParameter[] parameters, int limit);
        DataResultTable GetLimitedRecords(string queryString, int offset, int count);
        DataResultTable GetLimitedRecords(string queryString, DataBaseParameter[] parameters, int offset, int count);
        DataResultTable GetPagedResultTable(string queryString, int page, int pageSize);
        DataResultTable GetPagedResultTable(string queryString, DataBaseParameter[] parameters, int page, int pageSize);
    }
}