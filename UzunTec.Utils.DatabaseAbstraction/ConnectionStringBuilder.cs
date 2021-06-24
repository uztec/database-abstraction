namespace UzunTec.Utils.DatabaseAbstraction
{
    public static class ConnectionStringBuilder
    {

        public static string GetConnectionString(string server, string databaseName, int? port, bool windowsAuthentication, bool multipleResultSets = false)
        {
            if (port != null)
            {
                server = $"{server},{port}";
            }

            return GetConnectionString(server, databaseName, windowsAuthentication, multipleResultSets);
        }

        public static string GetConnectionString(string server, string databaseName, int? port, string user, string password, bool multipleResultSets = false)
        {
            if (port != null)
            {
                server = $"{server},{port}";
            }

            return GetConnectionString(server, databaseName, user, password, multipleResultSets);
        }

        public static string GetConnectionString(string server, string databaseName, bool windowsAuthentication, bool multipleResultSets = false)
        {
            string connectionString = $"Initial Catalog={databaseName};Data Source={server};Trusted_Connection={windowsAuthentication};";
            if (multipleResultSets)
            {
                connectionString += "MultipleActiveResultSets=true;";
            }
            return connectionString;

        }

        public static string GetConnectionString(string server, string databaseName, string user, string password, bool multipleResultSets = false)
        {
            string connectionString = $"Initial Catalog={databaseName};Data Source={server};User ID={user}; Password={password};";
            if (multipleResultSets)
            {
                connectionString += "MultipleActiveResultSets=true;";
            }
            return connectionString;
        }
    }
}

