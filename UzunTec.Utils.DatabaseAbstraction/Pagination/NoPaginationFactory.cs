using System;

namespace UzunTec.Utils.DatabaseAbstraction.Pagination
{
    internal class NoPaginationFactory : IPaginationFactory
    {
        public string AddLimit(string queryString, int recordLimit)
        {
            throw new ArgumentException("Dialect / Engine must be set to use pagination");
        }

        public string AddPagination(string queryString, int offset, int count)
        {
            throw new ArgumentException("Dialect / Engine must be set to use pagination");
        }
    }
}