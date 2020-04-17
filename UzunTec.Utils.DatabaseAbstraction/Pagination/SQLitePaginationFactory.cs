namespace UzunTec.Utils.DatabaseAbstraction.Pagination
{
    internal class SQLitePaginationFactory : IPaginationFactory
    {
        private IDbQueryBase dbBase;

        public SQLitePaginationFactory()
        {
        }

        public SQLitePaginationFactory(IDbQueryBase dbBase)
        {
            this.dbBase = dbBase;
        }

        public string AddLimit(string queryString, int recordLimit)
        {
            throw new System.NotImplementedException();
        }

        public string AddPagination(string queryString, int offset, int count)
        {
            throw new System.NotImplementedException();
        }
    }
}