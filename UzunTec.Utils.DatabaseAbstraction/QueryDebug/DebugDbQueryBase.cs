using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace UzunTec.Utils.DatabaseAbstraction.QueryDebug
{
    public class DebugDbQueryBase : IDbQueryBase
    {
        private readonly IDbQueryBase baseQueryBase;

        public DebugDbQueryBase(IDbQueryBase baseQueryBase)
        {
            this.baseQueryBase = baseQueryBase;
        }

        public AbstractionOptions Options => this.baseQueryBase.Options;

        public void BeginTransaction()
        {
            this.baseQueryBase.BeginTransaction();
        }

        public void CommitTransaction()
        {
            this.baseQueryBase.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            this.baseQueryBase.RollbackTransaction();
        }

        public int ExecuteNonQuery(string queryString)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int output = this.baseQueryBase.ExecuteNonQuery(queryString);
            sw.Stop();
            QueryLog.Log(queryString, null, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

        public int ExecuteNonQuery(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int output = this.baseQueryBase.ExecuteNonQuery(queryString, parameters);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

        public Task<int> ExecuteNonQueryAsync(string queryString)
        {
            return this.ExecuteNonQueryAsync(queryString, null);
        }

        public async Task<int> ExecuteNonQueryAsync(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int output = await this.baseQueryBase.ExecuteNonQueryAsync(queryString, parameters).ConfigureAwait(false);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

        public object ExecuteScalar(string queryString)
        {
            Stopwatch sw = Stopwatch.StartNew();
            object output = this.baseQueryBase.ExecuteScalar(queryString);
            sw.Stop();
            QueryLog.Log(queryString, null, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

        public object ExecuteScalar(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            Stopwatch sw = Stopwatch.StartNew();
            object output = this.baseQueryBase.ExecuteScalar(queryString, parameters);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

        public Task<object> ExecuteScalarAsync(string queryString)
        {
            return this.ExecuteScalarAsync(queryString, null);
        }

        public async Task<object> ExecuteScalarAsync(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            Stopwatch sw = Stopwatch.StartNew();
            object output = await this.baseQueryBase.ExecuteScalarAsync(queryString, parameters).ConfigureAwait(false);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

        public DataResultTable GetLimitedRecords(string queryString, int offset, int count)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultTable output = this.baseQueryBase.GetLimitedRecords(queryString, offset, count);
            sw.Stop();
            QueryLog.Log(queryString, null, sw.Elapsed.TotalMilliseconds, output, count, offset);
            return output;
        }

        public DataResultTable GetLimitedRecords(string queryString, IEnumerable<DataBaseParameter> parameters, int offset, int count)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultTable output = this.baseQueryBase.GetLimitedRecords(queryString, parameters, offset, count);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output, count, offset);
            return output;
        }

        public Task<DataResultTable> GetLimitedRecordsAsync(string queryString, int offset, int count)
        {
            return this.GetLimitedRecordsAsync(queryString, null, offset, count);
        }

        public async Task<DataResultTable> GetLimitedRecordsAsync(string queryString, IEnumerable<DataBaseParameter> parameters, int offset, int count)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultTable output = await this.baseQueryBase.GetLimitedRecordsAsync(queryString, parameters, offset, count).ConfigureAwait(false);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output, count, offset);
            return output;
        }

        public DataResultTable GetPagedResultTable(string queryString, int page, int pageSize)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultTable output = this.baseQueryBase.GetPagedResultTable(queryString, page, pageSize);
            sw.Stop();
            QueryLog.Log(queryString, null, sw.Elapsed.TotalMilliseconds, output, pageSize, (page - 1) * pageSize);
            return output;
        }

        public DataResultTable GetPagedResultTable(string queryString, IEnumerable<DataBaseParameter> parameters, int page, int pageSize)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultTable output = this.baseQueryBase.GetPagedResultTable(queryString, parameters, page, pageSize);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output, pageSize, (page - 1) * pageSize);
            return output;
        }

        public Task<DataResultTable> GetPagedResultTableAsync(string queryString, int page, int pageSize)
        {
            return this.GetPagedResultTableAsync(queryString, null, page, pageSize);
        }

        public async Task<DataResultTable> GetPagedResultTableAsync(string queryString, IEnumerable<DataBaseParameter> parameters, int page, int pageSize)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultTable output = await this.baseQueryBase.GetPagedResultTableAsync(queryString, parameters, page, pageSize).ConfigureAwait(false);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output, pageSize, (page - 1) * pageSize);
            return output;
        }

        public DataResultTable GetResultTable(string queryString)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultTable output = this.baseQueryBase.GetResultTable(queryString);
            sw.Stop();
            QueryLog.Log(queryString, null, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

        public DataResultTable GetResultTable(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultTable output = this.baseQueryBase.GetResultTable(queryString, parameters);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

        public DataResultTable GetResultTable(string queryString, int limit)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultTable output = this.baseQueryBase.GetResultTable(queryString, limit);
            sw.Stop();
            QueryLog.Log(queryString, null, sw.Elapsed.TotalMilliseconds, output, limit, -1);
            return output;
        }

        public DataResultTable GetResultTable(string queryString, IEnumerable<DataBaseParameter> parameters, int limit)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultTable output = this.baseQueryBase.GetResultTable(queryString, parameters, limit);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output, limit, -1);
            return output;
        }

        public Task<DataResultTable> GetResultTableAsync(string queryString)
        {
            return this.GetResultTableAsync(queryString, null);
        }

        public async Task<DataResultTable> GetResultTableAsync(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultTable output = await this.baseQueryBase.GetResultTableAsync(queryString, parameters).ConfigureAwait(false);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

        public Task<DataResultTable> GetResultTableAsync(string queryString, int limit)
        {
            return this.GetResultTableAsync(queryString, null, limit);
        }

        public async Task<DataResultTable> GetResultTableAsync(string queryString, IEnumerable<DataBaseParameter> parameters, int limit)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var output = await this.baseQueryBase.GetResultTableAsync(queryString, parameters, limit).ConfigureAwait(false);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output, limit, -1);
            return output;
        }

        public DataResultTable GetResultTableFromProcedure(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultTable output = this.baseQueryBase.GetResultTableFromProcedure(queryString, parameters);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

        public async Task<DataResultTable> GetResultTableFromProcedureAsync(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultTable output = await this.baseQueryBase.GetResultTableFromProcedureAsync(queryString, parameters).ConfigureAwait(false);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

        public DataResultRecord GetSingleRecord(string queryString)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultRecord output = this.baseQueryBase.GetSingleRecord(queryString);
            sw.Stop();
            QueryLog.Log(queryString, null, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

        public DataResultRecord GetSingleRecord(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultRecord output = this.baseQueryBase.GetSingleRecord(queryString, parameters);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

        public Task<DataResultRecord> GetSingleRecordAsync(string queryString)
        {
            return this.GetSingleRecordAsync(queryString, null);
        }

        public async Task<DataResultRecord> GetSingleRecordAsync(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            Stopwatch sw = Stopwatch.StartNew();
            DataResultRecord output = await this.baseQueryBase.GetSingleRecordAsync(queryString, parameters).ConfigureAwait(false);
            sw.Stop();
            QueryLog.Log(queryString, parameters, sw.Elapsed.TotalMilliseconds, output);
            return output;
        }

    }
}
