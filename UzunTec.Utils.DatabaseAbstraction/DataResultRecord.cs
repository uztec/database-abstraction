using System;
using System.Collections.Generic;
using System.Data;
using UzunTec.Utils.Common;

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

        public string GetString(string columnName)
        {
            return (this[columnName] == System.DBNull.Value) ? null : (string)this[columnName];
        }

        public T GetValue<T>(string columnName) where T : struct
        {
            return this.GetNullableValue<T>(columnName) ?? default(T);
        }

        public T? GetNullableValue<T>(string columnName) where T : struct
        {
            object value = (this[columnName] == DBNull.Value) ? null : this[columnName];
            return (value == null) ? (T?)null : (T)Convert.ChangeType(value, typeof(T));
        }

        public T GetEnum<T>(string columnName) where T : struct, Enum
        {
            return this.GetNullableEnum<T>(columnName) ?? default(T);
        }

        public T? GetNullableEnum<T>(string columnName) where T : struct, Enum
        {
            object value = (this[columnName] == DBNull.Value) ? null : this[columnName];
            return EnumUtils.GetEnumValue<T>(value);
        }
    }
}
