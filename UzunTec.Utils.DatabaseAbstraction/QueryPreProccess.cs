using System;
using System.Collections.Generic;

namespace UzunTec.Utils.DatabaseAbstraction
{
    internal class QueryPreProccess
    {
        private readonly AbstractionOptions options;
        public Func<string, string> PreProcessQuery { get; }
        public Func<string, IEnumerable<DataBaseParameter>, IEnumerable<DataBaseParameter>> PreProcessParameters { get; }

        internal QueryPreProccess(AbstractionOptions options)
        {
            this.options = options;

            if (options.QueryParameterIdentifier == options.DialectParameterIdentifier)
            {
                this.PreProcessQuery = delegate (string s) { return s; };
            }
            else
            {
                this.PreProcessQuery = this.PreProcessQueyForDifferentIdentifiers;
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

        private string PreProcessQueyForDifferentIdentifiers(string queryString)
        {
            return queryString.Replace(this.options.QueryParameterIdentifier, this.options.DialectParameterIdentifier);
        }
    }
}
