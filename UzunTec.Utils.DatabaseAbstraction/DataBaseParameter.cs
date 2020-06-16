using System;
using System.Data;
using System.Data.Common;

namespace UzunTec.Utils.DatabaseAbstraction
{
    public class DataBaseParameter : DbParameter, IDbDataParameter, IDataParameter
    {
        private string m_name;
        private object m_value;

        public override string ParameterName
        {
            get { return m_name; }
            set
            {
                // Removes @ or # or : from Param name
                char firstChar = value[0];
                if (firstChar == '#' || firstChar == '@' || firstChar == ':')
                    m_name = value.Substring(1);
                else
                    m_name = value;
            }
        }

        public override object Value
        {
            get { return m_value; }
            set { m_value = (value == null) ? DBNull.Value : value; }   // Null case check
        }

        public override string ToString()
        {
            return $"{{{this.ParameterName} : {this.Value}}}";
        }

        #region Overload Properties

        public override DbType DbType { get; set; }

        public override ParameterDirection Direction { get; set; }

        public override bool IsNullable { get; set; }

        public override void ResetDbType()
        {
            this.DbType = DbType.String;
        }

        public override int Size { get; set; }

        public override string SourceColumn { get; set; }

        public override bool SourceColumnNullMapping { get; set; }

        public override DataRowVersion SourceVersion { get; set; }

        #endregion


        public DataBaseParameter()
        {
            this.Direction = ParameterDirection.Input;
        }

        public DataBaseParameter(string Name) : this()
        {
            this.ParameterName = Name;
        }

        public DataBaseParameter(string Name, object Value) : this(Name)
        {
            this.Value = Value;
        }

        public DataBaseParameter(string Name, object Value, ParameterDirection parameterDirection) : this(Name, Value)
        {
            this.Direction = parameterDirection;
        }
    }
}
