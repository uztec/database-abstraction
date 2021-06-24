using System.Data;
using System.IO;

namespace UzunTec.Utils.DatabaseAbstraction.Test
{
    public class DBBootstrap : DbQueryBase, IDbQueryBase
    {
        private readonly string dbName = "UZTEC_DB_ABSTRACTION_TEST";

        public DBBootstrap(ConnectionBuilder connectionBuilder, DatabaseDialect dialect) : base(connectionBuilder, dialect)
        {
            string fullSql = File.ReadAllText("DbScript.sql").Replace("@DBNAME", this.dbName);

            foreach (string sql in fullSql.Split(";"))
            {
                base.ExecuteNonQuery(sql);
            }
        }
    }
}
