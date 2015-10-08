using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CommonUtility.Database.StoredProcedure
{
    public class Object2SQLParameter
    {
        private object _entity;
        private Type _type;

        public List<string> KeyNames;
        public List<object> KeyValues;

        public List<string> ParameterNames;
        public List<object> ParameterValues;

        public void Parse(object pEntity)
        {
            this._entity = pEntity;
            this._type = pEntity.GetType();


            //获得实体的属性集合 
            PropertyInfo[] props = _type.GetProperties();

            this.ParameterNames = new List<String>(props.Length);
            this.ParameterValues = new List<Object>(props.Length);

            this.KeyNames = new List<String>(1);
            this.KeyValues = new List<Object>(1);

            foreach (PropertyInfo prop in props)
            {
                string colName = prop.Name;
                object colValue = prop.GetValue(pEntity, null);

                // string null or empty value
                if (prop.PropertyType == typeof(string))
                {
                    if (string.IsNullOrWhiteSpace(colValue as string))
                        colValue = null;
                }

                bool isPK = false;

                //if is keyattribute return key
                object[] columnAttrs = prop.GetCustomAttributes(typeof(ColumnAttribute), true);
                ColumnAttribute colAttr = null;
                if (columnAttrs.Length > 0)
                {
                    colAttr = columnAttrs[0] as ColumnAttribute;
                    if (colAttr.Name != null)
                    {
                        colName = colAttr.Name;
                    }
                    isPK = colAttr.IsPrimaryKey;

                    if (isPK)
                    {
                        this.KeyNames.Add(colName);
                        this.KeyValues.Add(colValue);
                    }
                    else
                    {
                        this.ParameterNames.Add(colName);
                        this.ParameterValues.Add(colValue);
                    }
                }
            }
        }

    }
}
