using System;
using System.Collections.Generic;
using System.Data;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public class DataResultRecord : Dictionary<string, object>, IDictionary<string, object>, IReadOnlyDictionary<string, object>
    {
        internal DataResultRecord(IDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                this.Add(reader.GetName(i), reader[i]);
            }
        }

        public T? GetNullableValue<T>(string ColumnName) where T : struct
        {
            object value = (this[ColumnName] == System.DBNull.Value) ? null : this[ColumnName];
            return (value == null) ? (T?)null : (T)Convert.ChangeType(value, typeof(T));
        }

        public T GetValue<T>(string ColumnName) where T : struct
        {
            object value = (this[ColumnName] == System.DBNull.Value) ? null : this[ColumnName];
            return (value == null) ? default(T) : (T)Convert.ChangeType(value, typeof(T));
        }

        public string GetString(string ColumnName)
        {
            return (this[ColumnName] == System.DBNull.Value) ? null : (string)this[ColumnName];
        }
    }
}
