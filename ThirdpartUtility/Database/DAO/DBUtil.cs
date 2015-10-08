using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ThirdpartUtility.Database.DAO
{
    public class DBUtil
    {

        public static char GetDBParamFlag(short dbType)
        {
            if (ADOTemplate.DB_TYPE_ORACLE == dbType)
                return ':';
            return '@';
        }

        /// <summary>
        /// 把Table中的数据，通过反射，转为指定类型的对象数组
        /// </summary>
        /// <param name="DataTable">数据表</param>
        /// <generic name="T">返回列表中，元素的对象类型</generic>
        /// <returns>objectType指定类型的数组</returns>
        public static T[] Table2Array<T>(DataTable table) where T : new()
        {
            var list = Table2List<T>(table);
            if (list == null)
                return new T[0];
            return list.ToArray();
        }

        /// <summary>
        /// 把Table中的数据，通过反射，转为指定类型的对象列表
        /// </summary>
        /// <param name="DataTable">数据表</param>
        /// <generic name="T">返回列表中，元素的对象类型</generic>
        /// <returns>objectType指定类型的列表</returns>
        public static IList<T> Table2List<T>(DataTable table) where T : new()
        {
            if (table == null || table.Rows.Count <= 0)
                return new List<T>();

            IList<T> result = new List<T>(table.Rows.Count);

            IList<Object[]> proNameList = getEntityNames<T>(table);//Object[]:Object[]{PropertyInfo,string}

            foreach (DataRow row in table.Rows)
            {
                T t = Row2Object<T>(proNameList, row);
                result.Add(t);
            }
            return result;
        }

        /// <summary>
        /// 把Row类型数据，通过反射，转为指定类型的对象，
        /// 转换时，
        /// 1）创建objectType的实例object，遍历objectType中的所有属性
        /// 2）每一属性，先看有没有Column Attribute定义，
        ///       a)如果有，取出column's name，
        ///       b)若没有，则属性名
        ///    作为数据行字段查询名字（名字统一细写）
        /// 3）从数据行查出来的值，放于实例object中
        /// </summary>
        /// <param name="row">数据行</param>
        /// <generic name="T">返回的对象类型</generic>
        /// <returns>objectType指定类型的实例</returns>
        public static T Row2Object<T>(DataRow row) where T : new()
        {
            if (row == null)
                return default(T);
            IList<Object[]> proNameList = getEntityNames<T>(row.Table);
            return Row2Object<T>(proNameList, row);
        }

        /// <summary>
        /// 把Row类型数据，通过反射，转为指定类型的对象，
        /// 转换时，
        /// 1）创建objectType的实例object，遍历objectType中的所有属性
        /// 2）每一属性，先看有没有Column Attribute定义，
        ///       a)如果有，取出column's name，
        ///       b)若没有，则属性名
        ///    作为数据行字段查询名字（名字统一细写）
        /// 3）从数据行查出来的值，放于实例object中
        /// </summary>
        /// <param name="row">数据行</param>
        /// <generic name="T">返回的对象类型</generic>
        /// <returns>objectType指定类型的实例</returns>
        private static T Row2Object<T>(IList<Object[]> proNameList, DataRow row) where T : new()
        {
            T t = new T();  //t is temp
            for (int i = 0; i < proNameList.Count; i++)
            {
                PropertyInfo prop = proNameList[i][0] as PropertyInfo;
                string strColumnName = proNameList[i][1] as string;
                var value = row[strColumnName];
                if (value != null && value != DBNull.Value)
                    prop.SetValue(t, value, null);
            }
            return t;
        }

        private static IList<Object[]> getEntityNames<T>(DataTable dt)
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

                if (dt != null)
                {
                    if (dt.Columns.Contains(proName))
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

        /// <summary>
        /// get connection Object by dbType & connect String
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="connectString"></param>
        /// <returns></returns>
        public static IDbConnection GetConnection(string dbName)
        {
            return GetConnection(ConfigCache.GetDBType(dbName), ConfigCache.GetDBConnectStr(dbName));
        }

        public static IDbConnection GetConnection(short dbType, string connectString)
        {

            if (dbType == ADOTemplate.DB_TYPE_ORACLE)
            {
                return OracleConnectionFactory.Get(connectString);
            }
            else if (dbType == ADOTemplate.DB_TYPE_SQLSERVER)
            {
                return new SqlConnection(connectString);
            }
            else
            {
                return MySqlConnectionFactory.Get(connectString);
            }
        }
    }

    /// <summary>
    /// Oracle 连接工程
    /// </summary>
    class OracleConnectionFactory
    {
        public static IDbConnection Get(string connectString)
        {
            return new Oracle.DataAccess.Client.OracleConnection(connectString);
        }
    }
    /// <summary>
    /// MySql 连接工程
    /// </summary>
    class MySqlConnectionFactory
    {
        public static IDbConnection Get(string connectString)
        {
            return new MySql.Data.MySqlClient.MySqlConnection(connectString);
        }
    }
}
