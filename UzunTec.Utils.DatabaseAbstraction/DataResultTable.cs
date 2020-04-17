using System;
using System.Collections.Generic;
using System.Data;
using UzunTec.Utils.Common;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public class DataResultTable : List<DataResultRecord>
    {
        public IReadOnlyDictionary<string, Type> Fields { get; }

        internal DataResultTable(IDataReader reader)
        {
            Dictionary<string, Type> cols = new Dictionary<string, Type>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                cols.Add(reader.GetName(i), reader.GetFieldType(i));
            }

            while (reader.Read())
            {
                this.Add(new DataResultRecord(reader));
            }

            reader.Close();
        }

        public void InsertRecord(int index, DataResultRecord record)
        {
            if (record.Keys.CompareTo(new List<string>(this.Fields.Keys)) != 0)
            {
                throw new ArgumentException("This Record does not contains the same fields of this DataResultTable");
            }
            this.Insert(index, record);
        }

        public void AddRecord(DataResultRecord record)
        {
            this.InsertRecord(this.Count, record);
        }

        public List<T> BuildList<T>(Func<DataResultRecord, T> buildObjectFunction)
        {
            List<T> list = new List<T>();
            foreach (DataResultRecord record in this)
            {
                list.Add(buildObjectFunction(record));
            }
            return list;
        }
    }
}
