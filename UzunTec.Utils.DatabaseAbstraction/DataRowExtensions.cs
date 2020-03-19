using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public static class DataRowExtensions
    {

        public static T? GetNullableValue<T>(this DataRow dr, string ColumnName) where T : struct
        {
            object value = (dr[ColumnName] == System.DBNull.Value) ? null : dr[ColumnName];

            if (value == null)
                return (T?)null;
            else
                return (T)System.Convert.ChangeType(value, typeof(T));
        }

        public static T GetValue<T>(this DataRow dr, string ColumnName) where T : struct
        {
            object value = (dr[ColumnName] == System.DBNull.Value) ? null : dr[ColumnName];

            if (value == null)
                return default(T);
            else
                return (T)System.Convert.ChangeType(value, typeof(T));
        }


        public static string GetString(this DataRow dr, string ColumnName)
        {
            return (dr[ColumnName] == System.DBNull.Value) ? null : (string) dr[ColumnName];
        }
    }
}
