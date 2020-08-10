using System;
using System.Collections.Generic;
using System.Text;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public class AbstractionOptions
    {
        public DatabaseDialect Dialect { get; set; }
        public bool AutoCloseConnection { get; set; }
        public bool AllowMultipleConnections { get; set; }
        public bool SortQueryParameters { get; set; }
        public char QueryParameterIdentifier { get; set; }
        public char DialectParameterIdentifier { get; set; }
    }
}
