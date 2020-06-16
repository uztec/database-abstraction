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
                default:
                    return new NoPaginationFactory();
            }
        }
    }
}
