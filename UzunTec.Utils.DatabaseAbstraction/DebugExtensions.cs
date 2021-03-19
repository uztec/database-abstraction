using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public static class DebugScriptExtensions
    {
        public static string GenerateOracleScriptToDebug(string queryString, IEnumerable<DataBaseParameter> parameters, char paramIdentifier = '@')
        {
            AbstractionOptions options = DefaultDialectOptions.GetDefaultOptions(DatabaseDialect.Oracle);
            options.QueryParameterIdentifier = paramIdentifier;

            QueryPreProccess preProccess = new QueryPreProccess(options);
            parameters = preProccess.PreProcessParameters(queryString, parameters);

            string newQuery = preProccess.PreProcessQuery(queryString);
            foreach (char c in "\n\t\r")
            {
                newQuery = newQuery.Replace(c, ' ');
            }
            newQuery = new Regex(" +").Replace(newQuery, " ");

            string output = $"  DECLARE \n    sql_query VARCHAR2({newQuery.Length + 2}) := '{newQuery}';\n";

            List<string> parametersNames = new List<string>();
            foreach (DataBaseParameter parameter in parameters)
            {
                if (parameter.Value is DateTime)
                {
                    DateTime dt = (DateTime)parameter.Value;
                    output += $"    {parameter.ParameterName} DATE := TO_DATE('{dt:yyyy-MM-dd HH:mm:ss}', 'YYYY-MM-DD HH24:MI:SS');\n";
                }
                else
                {
                    output += $"    {parameter.ParameterName} VARCHAR2({parameter.Value.ToString().Length}) := '{parameter.Value}';\n";
                }
                parametersNames.Add(parameter.ParameterName);
            }

            output += $"BEGIN\n     EXECUTE IMMEDIATE sql_query USING {string.Join(",", parametersNames.ToArray())};\n END;\n  /";
            return output;
        }

        public static IEnumerable<DataBaseParameter> TruncateDateTimeParameters(this IEnumerable<DataBaseParameter> parameters)
        {
            foreach (DataBaseParameter parameter in parameters)
            {
                if (parameter.Value is DateTime dt)
                {
                    parameter.Value = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
                }
            }
            return parameters;
        }
    }
}
