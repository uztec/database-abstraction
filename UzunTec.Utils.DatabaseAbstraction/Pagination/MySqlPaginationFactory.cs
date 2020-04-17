namespace UzunTec.Utils.DatabaseAbstraction.Pagination
{
    internal class MySqlPaginationFactory : IPaginationFactory
    {
        public string AddLimit(string queryString, int recordLimit)
        {
            queryString = queryString.TrimEnd(" \t\n\r;".ToCharArray());
            return queryString + $" LIMIT {recordLimit}";
        }

        public string AddPagination(string queryString, int offset, int count)
        {
            queryString = queryString.TrimEnd(" \t\n\r;".ToCharArray());
            return queryString + $" LIMIT {count} OFFSET {offset}";
        }
    }
}