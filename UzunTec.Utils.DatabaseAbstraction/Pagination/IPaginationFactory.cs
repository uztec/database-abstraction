namespace UzunTec.Utils.DatabaseAbstraction.Pagination
{
    internal interface IPaginationFactory
    {
        string AddLimit(string queryString, int recordLimit);
        string AddPagination(string queryString, int offset, int count);
    }
}
