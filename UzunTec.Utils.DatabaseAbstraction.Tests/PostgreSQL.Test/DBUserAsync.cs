using System;
using System.Threading.Tasks;

namespace UzunTec.Utils.DatabaseAbstraction.PostgreSQL.Test
{
    public class DBUserAsync
    {
        private readonly IDbQueryBase dbBase;

        public DBUserAsync(IDbQueryBase dbBase)
        {
            this.dbBase = dbBase;
        }

        internal Task<DataResultRecord> FindByIDAsync(int ID)
        {
            string queryString = @" SELECT COD_USER, USER_NAME, COD_USER_REF, PASSWORD_MD5, INPUT_DATE
	                                    FROM USER_TEST
                                        WHERE ID_USER = @ID_USER";

            DataBaseParameter[] parameters = new DataBaseParameter[]
            {
                new DataBaseParameter("ID_USER", ID),
            };

            return this.dbBase.GetSingleRecordAsync(queryString, parameters);
        }

        internal Task<DataResultRecord> FindByCodeAsync(int userCode)
        {
            string queryString = @" SELECT COD_USER, USER_NAME, COD_USER_REF, PASSWORD_MD5, INPUT_DATE, USER_STATUS
	                                    FROM USER_TEST
                                        WHERE COD_USER = @COD_USER";

            DataBaseParameter[] parameters = new DataBaseParameter[]
            {
                new DataBaseParameter("COD_USER", userCode),
            };

            return this.dbBase.GetSingleRecordAsync(queryString, parameters);
        }

        internal async Task<bool> InsertAsync(int userCode, string userName, long? userCodeRef, string passwordMd5, DateTime inputDate, StatusUser status)
        {
            string queryString = @" INSERT INTO USER_TEST (COD_USER, USER_NAME, COD_USER_REF, PASSWORD_MD5, INPUT_DATE, USER_STATUS)
                                        VALUES(@COD_USER, @USER_NAME, @COD_USER_REF, @PASSWORD_MD5, @INPUT_DATE, @USER_STATUS)";

            DataBaseParameter[] parameters = new DataBaseParameter[]
            {
                new DataBaseParameter("COD_USER", userCode),
                new DataBaseParameter("USER_NAME", userName),
                new DataBaseParameter("COD_USER_REF", userCodeRef),
                new DataBaseParameter("PASSWORD_MD5", passwordMd5),
                new DataBaseParameter("INPUT_DATE", inputDate),
                new DataBaseParameter("USER_STATUS", (char)status),
            };


            int inserted = await this.dbBase.ExecuteNonQueryAsync(queryString, parameters).ConfigureAwait(false);
            return (inserted == 1);
        }

        internal async Task<bool> DeleteAsync(int userCode)
        {

            string queryString = @" DELETE FROM USER_TEST WHERE COD_USER = @COD_USER";

            DataBaseParameter[] parameters = new DataBaseParameter[]
            {
                new DataBaseParameter("COD_USER", userCode)
            };

            int deleted = await this.dbBase.ExecuteNonQueryAsync(queryString, parameters);

            return (deleted == 1);
        }

        internal Task<int> UpdateAsync(int oldCode, int userCode, string userName, long? userCodeRef, string passwordMd5, DateTime inputDate, StatusUser status)
        {
            string queryString = @" UPDATE USER_TEST
                                        SET COD_USER = @COD_USER,
                                            USER_NAME = @USER_NAME,
                                            COD_USER_REF = @COD_USER_REF,
                                            PASSWORD_MD5 = @PASSWORD_MD5,
                                            INPUT_DATE = @INPUT_DATE,
                                            USER_STATUS = @USER_STATUS
                                       WHERE COD_USER = @OLD_COD_USER";

            DataBaseParameter[] parameters = new DataBaseParameter[]
           {
                new DataBaseParameter("OLD_COD_USER", oldCode),
                new DataBaseParameter("COD_USER", userCode),
                new DataBaseParameter("USER_NAME", userName),
                new DataBaseParameter("COD_USER_REF", userCodeRef),
                new DataBaseParameter("PASSWORD_MD5", passwordMd5),
                new DataBaseParameter("INPUT_DATE", inputDate),
                new DataBaseParameter("USER_STATUS", (char)status),
           };

            return this.dbBase.ExecuteNonQueryAsync(queryString, parameters);
        }

        internal Task<DataResultTable> ListAllAsync()
        {
            string queryString = @" SELECT COD_USER, USER_NAME, COD_USER_REF, PASSWORD_MD5, INPUT_DATE, USER_STATUS
	                                    FROM USER_TEST";

            return this.dbBase.GetResultTableAsync(queryString);
        }
    }
}
