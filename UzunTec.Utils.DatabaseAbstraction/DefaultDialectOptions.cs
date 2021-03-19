using UzunTec.Utils.Common;

namespace UzunTec.Utils.DatabaseAbstraction
{
    internal static class DefaultDialectOptions
    {
        public static AbstractionOptions GetDefaultOptions(string engine)
        {
            return GetDefaultOptions(EnumUtils.GetEnumValue<DatabaseDialect>(engine) ?? DatabaseDialect.NotSet);
        }

        public static AbstractionOptions GetDefaultOptions(DatabaseDialect dialect)
        {
            return new AbstractionOptions
            {
                Dialect = dialect,
                UseLockedCommands = (dialect == DatabaseDialect.SqlServer || dialect == DatabaseDialect.MySql),
                AutoCloseConnection = (dialect == DatabaseDialect.SqlServer || dialect == DatabaseDialect.Oracle),
                SortQueryParameters = (dialect == DatabaseDialect.Oracle),
                QueryParameterIdentifier = '@',
                DialectParameterIdentifier = (dialect == DatabaseDialect.Oracle) ? ':' : '@',
            };
        }
    }
}
