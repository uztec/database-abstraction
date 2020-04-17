using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UzunTec.Utils.DatabaseAbstraction.Pagination
{
    internal class SqlServerPaginationFactory : IPaginationFactory
    {
        public string AddLimit(string queryString, int recordLimit)
        {
            Regex pattern = new Regex(@"(?i)SELECT[ \t\r\n]+(DISTINCT)?(.*)");
            return pattern.Replace(queryString, $@"SELECT $1 TOP {recordLimit} $2", 1);
        }

        public string AddPagination(string queryString, int offset, int count)
        {
            // Protection - Must have ORDER BY to paginate for SQL SERVER
            Regex pattern = new Regex(@"(?i)ORDER[ \t\r\n]+BY");
            if (!pattern.IsMatch(queryString))
            {
                throw new PlatformNotSupportedException("SQLServer Queries must have 'ORDER BY' clause for pagination");
            }

            queryString = this.RemoveEndOfQueryString(queryString);
            return queryString + $" OFFSET {offset} ROWS FETCH NEXT {count} ROWS ONLY";
        }

        public string RemoveEndOfQueryString(string queryString)
        {
            queryString = queryString.TrimEnd(" \t\n\r;".ToCharArray());

            if (queryString.EndsWith("GO"))
            {
                char prev = queryString[queryString.Length - 3];
                if (new List<char>(" \t\r\n".ToCharArray()).Contains(prev))
                {
                    queryString = queryString.Substring(-3);
                }
            }
            return queryString;
        }
    }
}