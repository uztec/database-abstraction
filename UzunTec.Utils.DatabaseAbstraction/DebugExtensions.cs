using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public static class DebugScriptExtensions
    {
        public static string GenerateOracleScriptToDebug(string queryString, IEnumerable<DataBaseParameter> parameters, char paramIdentifier = '@')
        {
            QueryPreProccess preProccess = new QueryPreProccess(DatabaseDialect.Oracle, paramIdentifier);
            parameters = preProccess.PreProcessParameters(queryString, parameters);

            string newQuery = preProccess.PreProcessQuey(queryString);
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
                    output += $"    {parameter.ParameterName} TIMESTAMP(4) := TO_DATE('{dt.ToString("yyyy-MM-dd HH:mm:ss")}', 'YYYY-MM-DD HH24:MI:SS');\n";
                }
                else
                {
                    output += $"    {parameter.ParameterName} VARCHAR2({parameter.Value.ToString().Length}) := '{parameter.Value.ToString()}';\n";
                }
                parametersNames.Add(parameter.ParameterName);
            }

            output += $"BEGIN\n     EXECUTE IMMEDIATE sql_query USING {string.Join(",", parametersNames.ToArray())};\n END;\n  /";
            return output;
        }
    }
}
