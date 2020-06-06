using System;
using System.Collections.Generic;
using System.Text;

namespace UzunTec.Utils.DatabaseAbstraction.Pagination
{
    internal class PaginationAbstractFactory
    {
        internal static IPaginationFactory GetObject(DatabaseDialect dialect)
        {
            switch (dialect)
            {
                case DatabaseDialect.SqlServer: return new SqlServerPaginationFactory();
                case DatabaseDialect.MySql: return new MySqlPaginationFactory();
                case DatabaseDialect.SQLite: return new SQLitePaginationFactory();
                case DatabaseDialect.NotSet: return new NoPaginationFactory();
            }
            throw new ArgumentException("Invalid Database Dialect");
        }
    }
}
