using System.Collections.Generic;
using System.Data;

namespace UzunTec.Utils.DatabaseAbstraction
{
    internal static class ConnectionExtensions
    {
        public static IDbCommand CreateCommand(this IDbConnection connection, string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = queryString;
            command.FillParamenters(parameters);
            return command;
        }

        public static IDbCommand CreateCommand(this IDbConnection connection, string queryString)
        {
            return connection.CreateCommand(queryString, null);
        }

        public static void FillParamenters(this IDbCommand command, IEnumerable<DataBaseParameter> parameters)
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
