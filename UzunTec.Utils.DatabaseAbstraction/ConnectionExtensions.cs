using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace UzunTec.Utils.DatabaseAbstraction
{
    internal static class ConnectionExtensions
    {
        internal static DbCommand CreateCommand(this DbConnection connection, string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = queryString;
            command.FillParamenters(parameters);
            return command;
        }

        internal static DbCommand CreateCommand(this DbConnection connection, string queryString)
        {
            return connection.CreateCommand(queryString, null);
        }

        private static void FillParamenters(this DbCommand command, IEnumerable<DataBaseParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (DataBaseParameter param in parameters)
                {
                    IDataParameter p = command.CreateParameter();
                    p.ParameterName = param.ParameterName;
                    p.Value = param.Value;
                    p.Direction = param.Direction;
                    command.Parameters.Add(p);
                }
            }
        }
    }
}
