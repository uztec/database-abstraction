namespace UzunTec.Utils.DatabaseAbstraction
{
    public interface IDbQueryBase
    {
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        DataResultTable GetResultTable(string queryString);
        DataResultTable GetResultTable(string queryString, DataBaseParameter[] parameters);
        DataResultRecord GetSingleRecord(string queryString);
        DataResultRecord GetSingleRecord(string queryString, DataBaseParameter[] parameters);
        int ExecuteNonQuery(string queryString);
        int ExecuteNonQuery(string queryString, DataBaseParameter[] parameters);
        object ExecuteScalar(string queryString);
        object ExecuteScalar(string queryString, DataBaseParameter[] parameters);
    }
}