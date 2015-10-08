using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CommonUtility.Extension
{
    /// <summary>
    /// DataTable 拓展
    /// 将DataTable转换为Array或List,将DataRow转换为object
    /// </summary>
    public class DataTableExtension
    {

        #region Convert datatable to list or array

        /// <summary>
        /// convert DataTable to Array<T>
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="table">data source</param>
        /// <returns></returns>
        public static T[] DataTable2Array<T>(DataTable table) where T : new()
        {
            var list = DataTable2List<T>(table);
            if (list == null)
                return new T[0];
            return list.ToArray();
        }

        /// <summary>
        /// convert DataTable to IList<T>
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="table">data source</param>
        /// <returns></returns>
        public static IList<T> DataTable2List<T>(DataTable table) where T : new()
        {
            if (table == null || table.Rows.Count <= 0)
                return new List<T>();

            IList<T> result = new List<T>(table.Rows.Count);

            IList<Object[]> proNameList = GetEntityNames<T>(table);//Object[]:Object[]{PropertyInfo,string}

            foreach (DataRow row in table.Rows)
            {
                T t = Row2Object<T>(proNameList, row);
                result.Add(t);
            }
            return result;
        }

        //---------------------------------

        /// <summary>
        /// datarow to object
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="propertyNameList">list of property name</param>
        /// <param name="row">data row</param>
        /// <returns></returns>
        private static T Row2Object<T>(IList<Object[]> propertyNameList, DataRow row) where T : new()
        {
            T t = new T();  //t is temp
            for (int i = 0; i < propertyNameList.Count; i++)
            {
                PropertyInfo prop = propertyNameList[i][0] as PropertyInfo;
                string strColumnName = propertyNameList[i][1] as string;
                var value = row[strColumnName];
                if (value != null && value != DBNull.Value)
                    prop.SetValue(t, value, null);
            }
            return t;
        }

        /// <summary>
        /// get entity list of T which is contain ColumnAttribute
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="dataTable">data source</param>
        /// <returns></returns>
        private static IList<Object[]> GetEntityNames<T>(DataTable dataTable)
        {
            PropertyInfo[] props = typeof(T).GetProperties();
            IList<Object[]> result = new List<Object[]>();

            foreach (PropertyInfo prop in props)
            {
                string proName = prop.Name;
                object[] colAtt = prop.GetCustomAttributes(typeof(ColumnAttribute), true);
                if (colAtt.Length > 0)//if true,it has ColumnAttribute
                {
                    ColumnAttribute colum = colAtt[0] as ColumnAttribute;
                    if (colum != null && colum.Name != null)
                    {
                        proName = colum.Name;
                    }
                }

                if (dataTable != null)
                {
                    if (dataTable.Columns.Contains(proName))
                    {
                        Object[] objTemp = new Object[] { prop, proName };
                        result.Add(objTemp);
                    }
                }
                else
                {
                    Object[] objTemp = new Object[] { prop, proName };
                    result.Add(objTemp);
                }
            }
            return result;
        }

        #endregion Convert datatable to list or array

        #region Convert datarow to object

        /// <summary>
        /// convert DataRow to object
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="row">data source</param>
        /// <returns></returns>
        public static T DataRow2Object<T>(DataRow row) where T : new()
        {
            if (row == null)
                return default(T);
            IList<Object[]> proNameList = GetEntityNames<T>(row.Table);
            return Row2Object<T>(proNameList, row);
        }


        #endregion Convert datarow to object

        #region Convert object to string

        /// <summary>
        /// convert object list to string
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="sourceList">list of object</param>
        /// <param name="objectSplitChar">split char for object</param>
        /// <param name="fieldSplitChar">split char for field</param>
        /// <returns></returns>
        public static string ObjectList2SplitString<T>(IEnumerable<T> sourceList, string objectSplitChar = "|", string fieldSplitChar = ",")
        {
            if (sourceList == null)
                return null;

            List<string> phoneList = new List<string>();
            foreach (var item in sourceList)
            {
                var str = Object2SplitString<T>(item, fieldSplitChar);
                phoneList.Add(str);
            }

            return string.Join(objectSplitChar, phoneList);
        }

        /// <summary>
        /// convert object to string
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="source">data source</param>
        /// <param name="splitChar">split char for field</param>
        /// <returns></returns>
        public static string Object2SplitString<T>(T source, string splitChar = "|")
        {
            if (source == null)
                return null;
            IList<Object[]> proNameList = GetEntityNames<T>(null);
            return Object2SplitString<T>(proNameList, source, splitChar);
        }


        //-----------------------------------------

        /// <summary>
        /// object to string
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="propertyList">list of property</param>
        /// <param name="source"></param>
        /// <param name="splitChar"></param>
        /// <returns></returns>
        private static string Object2SplitString<T>(IList<Object[]> propertyList, T source, string splitChar)
        {
            List<string> values = new List<string>();
            for (int i = 0; i < propertyList.Count; i++)
            {
                PropertyInfo prop = propertyList[i][0] as PropertyInfo;
                string strColumnName = propertyList[i][1] as string;
                object value = prop.GetValue(source, null);

                if (value != null && value != DBNull.Value)
                    values.Add(value.ToString());
                else
                    values.Add("");
            }
            return string.Join(splitChar, values);
        }

        #endregion Convert object to string

    }
}
