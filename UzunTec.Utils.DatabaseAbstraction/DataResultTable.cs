using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UzunTec.Utils.Common;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public class DataResultTable : List<DataResulRecord>
    {
        public IReadOnlyDictionary<string, Type> Fields { get; }

        private DataResultTable(IDataReader reader)
        {
            Dictionary<string, Type> cols = new Dictionary<string, Type>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                cols.Add(reader.GetName(i), reader.GetFieldType(i));
            }

            while (reader.Read())
            {
                this.Add(new DataResulRecord(reader));
            }
            reader.Close();
        }

        public void InsertRecord(int index, DataResulRecord record)
        {
            if (record.Keys.CompareTo(new List<string>(this.Fields.Keys)) != 0)
            {
                throw new ArgumentException("This Record does not contains the same fields of this DataResultTable");
            }
            this.Insert(index, record);
        }

        public void AddRecord(DataResulRecord record)
        {
            this.InsertRecord(this.Count, record);
        }
    }
}
