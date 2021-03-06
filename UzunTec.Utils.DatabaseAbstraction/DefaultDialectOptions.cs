﻿using UzunTec.Utils.Common;

namespace UzunTec.Utils.DatabaseAbstraction
{
    internal static class DefaultDialectOptions
    {
        public static AbstractionOptions GetDefaultOptions(string engine, bool usingConnectionBuilder)
        {
            return GetDefaultOptions(EnumUtils.GetEnumValue<DatabaseDialect>(engine) ?? DatabaseDialect.NotSet, usingConnectionBuilder);
        }

        public static AbstractionOptions GetDefaultOptions(DatabaseDialect dialect, bool usingConnectionBuilder)
        {
            return new AbstractionOptions
            {
                Dialect = dialect,
                UseLockedCommands = (usingConnectionBuilder == false && (dialect == DatabaseDialect.SqlServer || dialect == DatabaseDialect.MySql)),
                AutoCloseConnection = (dialect == DatabaseDialect.SqlServer || dialect == DatabaseDialect.Oracle),
                SortQueryParameters = (dialect == DatabaseDialect.Oracle),
                QueryParameterIdentifier = '@',
                DialectParameterIdentifier = (dialect == DatabaseDialect.Oracle) ? ':' : '@',
            };
        }
    }
}
