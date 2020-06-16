using System.Collections.Generic;
using System.Data;

namespace UzunTec.Utils.DatabaseAbstraction
{
    internal static class ConnectionExtensions
    {
        internal static IDbCommand CreateCommand(this IDbConnection connection, string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = queryString;
            command.FillParamenters(parameters);
            return command;
        }

        internal static IDbCommand CreateCommand(this IDbConnection connection, string queryString)
        {
            return connection.CreateCommand(queryString, null);
        }

        private static void FillParamenters(this IDbCommand command, IEnumerable<DataBaseParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (DataBaseParameter param in parameters)
                {
                    IDbDataParameter p = command.CreateParameter();
                    p.ParameterName = param.ParameterName;
                    p.Value = param.Value;
                    p.Direction = param.Direction;
                    command.Parameters.Add(p);
                }
            }
        }
    }
}
