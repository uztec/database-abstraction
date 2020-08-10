using System;
using System.Collections.Generic;

namespace UzunTec.Utils.DatabaseAbstraction
{
    internal class QueryPreProccess
    {
        private readonly AbstractionOptions options;
        //private readonly char dialectParamIdentifier;
        public Func<string, string> PreProcessQuey { get; }
        public Func<string, IEnumerable<DataBaseParameter>, IEnumerable<DataBaseParameter>> PreProcessParameters { get; }

        internal QueryPreProccess(AbstractionOptions options)
        {
            this.options = options;

            if (options.QueryParameterIdentifier == options.DialectParameterIdentifier)
            {
                this.PreProcessQuey = delegate (string s) { return s; };
            }
            else
            {
                this.PreProcessQuey = this.PreProcessQueyForDifferentIdentifiers;
            }

            if (options.SortQueryParameters)
            {
                this.PreProcessParameters = this.SortParamsFromQuery;
            }
            else
            {
                this.PreProcessParameters = delegate (string s, IEnumerable<DataBaseParameter> parameters) { return parameters; };
            }
        }

        private IEnumerable<DataBaseParameter> SortParamsFromQuery(string queryString, IEnumerable<DataBaseParameter> parameters)
        {
            SortedList<int, DataBaseParameter> dicParameters = new SortedList<int, DataBaseParameter>();

            foreach (DataBaseParameter parameter in parameters)
            {
                string search = this.options.QueryParameterIdentifier + parameter.ParameterName;
                int pos = -1;
                do
                {
                    pos = queryString.IndexOf(search, pos + 1);

                    if (pos >= 0)
                    {
                        int endOfParamIndex = pos + search.Length;
                        char nextChar = (endOfParamIndex >= queryString.Length) ? ' ' : queryString[endOfParamIndex];
                        if (" ,=) \t\n\r".IndexOf(nextChar) >= 0)
                        {
                            dicParameters.Add(pos, parameter);
                        }
                    }
                } while (pos > 0);
            }

            return dicParameters.Values;
        }

        private IEnumerable<DataBaseParameter> TruncateParamsDateTime(IList<DataBaseParameter> parameters)
        {
            foreach (DataBaseParameter parameter in parameters)
            {
                if (parameter.Value is DateTime)
                {
                    DateTime dt = (DateTime)parameter.Value;
                    parameter.Value = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
                }
            }

            return parameters;
        }

        private string PreProcessQueyForDifferentIdentifiers(string queryString)
        {
            return queryString.Replace(this.options.QueryParameterIdentifier, this.options.DialectParameterIdentifier);
        }

        private char GetDefaultParamIdentifierFromDialect(DatabaseDialect dialect)
        {
            return (dialect == DatabaseDialect.Oracle) ? ':' : '@';
        }
    }
}
