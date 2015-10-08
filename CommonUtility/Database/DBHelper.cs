/* 【DBHelper】，用于封装与数据库操作相关的方法。 
 * 
 *  依赖说明：
 *  其下层调用，依赖于 "SQLHelper"，在它们的基础上，进行包装和扩展。
 *  后续可能封装不同数据库的调用逻辑
 * 
 *  这个封装类的主要的目标：
 *  1.希望封装重复、重要的操作步骤，降低编码疏忽而引起的DB问题；
 *  2.而非功能方面的灵活或全面，因此功能上会少于下层的Helper。
 *  
 * 
 *  使用说明：
 *  1.提供3个基础的DB操作接口,其中“参数1”为SQL语句，“参数2”为参数数据，“参数3”为是否使用事务(传null时，表示不使用事务，新建连接去处理)
      DBHelper.ExecuteNonQuery(sql, paramValues, tran);
      DBHelper.ExecuteDataTable(sql, paramValues, tran);
      DBHelper.ExecuteScalar(sql, paramValues, tran); 
      DBHelper.ExecuteDataSet(sql, paramValues, tran);
 *    
 *  2.使用事务时，请用using包括，
      using (var tran = DBHelper.CreateTransaction(隔离等级)) 
      {
         DBHelper.ExecuteNonQuery(sql, paramValues, tran);
         tran.MarkSuccess();
      }
 *    成功执行的最后，请调用MarkSuccess()，事务结束前会自动Commit；发生错误或者不显式MarkSuccess()的情况下，均会自动Rollback所有修改;
 *     
 *  3.隔离级别，根据实际情况进行选择。选择依据：
      未提交读（read uncommitted）   当事务A更新某条数据时，不容许其他事务来更新该数据，但可以读取。
      提交读（read committed） 	    当事务A更新某条数据时，不容许其他事务进行任何操作包括读取，但事务A读取时，其他事务可以进行读取、更新
      重复读（repeatable read） 	    当事务A更新数据时，不容许其他事务进行任何操作，但当事务A进行读取时，其他事务只能读取，不能更新。
      序列化（serializable）         最严格的隔离级别，事务必须依次进行。
 *    
 */

using Mirabeau.Sql.Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AsyncSQLHelper = Mirabeau.Sql.Library.DatabaseHelper;
using SyncSQLHelper = Utility.CommonUtility.Database.Helpers.MSSQLHelper;

namespace Utility.CommonUtility.Database
{
    /// <summary>
    /// 数据库辅助类
    /// </summary>
    public class DBHelper
    {
        //------------------------------

        #region 连接字符串

        /// <summary>
        /// 异步用途的连接字符串(此连接字符串，也适用于同步方式时使用)
        /// </summary>
        public static string SqlConnectionString = "";

        /// <summary>
        /// 设置连接字符串
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        public static void SetConnectionString(string connectionString)
        {
            //检查连接字符串
            SqlConnectionStringBuilder builder = null;
            try
            {
                builder = new SqlConnectionStringBuilder(connectionString);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("SqlConnectionString is invalid, please provide an effective parameter. ", "connectionString", ex);
            }

            //##异步的
            /* 启用异步SQL支持的时候，连接字符串必须包含以下属性。
             * https://msdn.microsoft.com/zh-cn/library/system.data.sqlclient.sqlconnectionstringbuilder.asynchronousprocessing(v=vs.110).aspx
             * 
             * 标记为“允许异步”。即：
             * 1.没有标记时，及时使用Command的Async方法，依然是同步操作；
             * 2.有标记时，如果继续使用Command的同步方法，仍然能用于同步操作。
             */
            builder.AsynchronousProcessing = true;
            SqlConnectionString = builder.ToString();
        }

        #endregion


        //------Database Operate Methods------

        #region 查询，返回DataSet结果集

        /// <summary>
        /// 查询，返回DataSet结果集
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数集合</param>
        /// <param name="transaction">不为空时，使用指定的事务处理；为空时，创建新的连接处理</param>
        /// <returns>DataSet结果集</returns>
        public static DataSet ExecuteDataSet(string commandText, SqlParameter[] commandParameters, DBTransaction transaction = null)
        {
            DataSet ds = null;

            if (transaction != null)
            {
                ds = SyncSQLHelper.ExecuteDataset(transaction.SqlTransaction, CommandType.Text, commandText, commandParameters);
            }
            else
            {
                ds = SyncSQLHelper.ExecuteDataset(SqlConnectionString, CommandType.Text, commandText, commandParameters);
            }

            //返回结果
            return ds;
        }

        /// <summary>
        /// 查询，返回DataSet结果集
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数集合</param>
        /// <param name="transaction">不为空时，使用指定的事务处理；为空时，创建新的连接处理</param>
        /// <returns>DataSet结果集</returns>
        public static async Task<DataSet> ExecuteDataSetAsync(string commandText, SqlParameter[] commandParameters, DBTransaction transaction = null)
        {
            DataSet ds = null;

            if (transaction != null)
            {
                ds = await Task.Run<DataSet>(() => AsyncSQLHelper.ExecuteDataSet(transaction.SqlTransaction, CommandType.Text, commandText, commandParameters));
            }
            else
            {
                ds = await Task.Run<DataSet>(() => AsyncSQLHelper.ExecuteDataSet(SqlConnectionString, CommandType.Text, commandText, commandParameters));
            }

            //返回结果
            return ds;
        }

        #endregion

        #region 查询，返回DataTable结果集

        /// <summary>
        /// 查询，返回DataTable结果集
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数集合</param>
        /// <param name="transaction">不为空时，使用指定的事务处理；为空时，创建新的连接处理</param>
        /// <returns>DataTable结果集</returns>
        public static DataTable ExecuteDataTable(string commandText, SqlParameter[] commandParameters, DBTransaction transaction = null)
        {
            DataSet ds = null;

            if (transaction != null)
            {
                ds = SyncSQLHelper.ExecuteDataset(transaction.SqlTransaction, CommandType.Text, commandText, commandParameters);
            }
            else
            {
                ds = SyncSQLHelper.ExecuteDataset(SqlConnectionString, CommandType.Text, commandText, commandParameters);
            }

            //返回结果
            if (ds == null || ds.Tables == null || ds.Tables.Count <= 0)
                return null;
            return ds.Tables[0];
        }

        /// <summary>
        /// 查询，返回DataTable结果集
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数集合</param>
        /// <param name="transaction">不为空时，使用指定的事务处理；为空时，创建新的连接处理</param>
        /// <returns>DataTable结果集</returns>
        public static async Task<DataTable> ExecuteDataTableAsync(string commandText, SqlParameter[] commandParameters, DBTransaction transaction = null)
        {
            DataSet ds = null;

            if (transaction != null)
            {
                ds = await Task.Run<DataSet>(() => AsyncSQLHelper.ExecuteDataSet(transaction.SqlTransaction, CommandType.Text, commandText, commandParameters));
            }
            else
            {
                ds = await Task.Run<DataSet>(() => AsyncSQLHelper.ExecuteDataSet(SqlConnectionString, CommandType.Text, commandText, commandParameters));
            }

            //返回结果
            if (ds == null || ds.Tables == null || ds.Tables.Count <= 0)
                return null;
            return ds.Tables[0];
        }

        #endregion

        #region 查询，返回单一结果

        /// <summary>
        /// 查询，返回单一结果
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数集合</param>
        /// <param name="transaction">不为空时，使用指定的事务处理；为空时，创建新的连接处理</param>
        /// <returns>单一结果</returns>
        public static object ExecuteScalar(string commandText, SqlParameter[] commandParameters, DBTransaction transaction = null)
        {
            object result = null;

            if (transaction != null)
            {
                result = SyncSQLHelper.ExecuteScalar(transaction.SqlTransaction, CommandType.Text, commandText, commandParameters);
            }
            else
            {
                result = SyncSQLHelper.ExecuteScalar(SqlConnectionString, CommandType.Text, commandText, commandParameters);
            }

            return result;
        }

        /// <summary>
        /// 查询，返回单一结果
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数集合</param>
        /// <param name="transaction">不为空时，使用指定的事务处理；为空时，创建新的连接处理</param>
        /// <returns>单一结果</returns>
        public static async Task<object> ExecuteScalarAsync(string commandText, SqlParameter[] commandParameters, DBTransaction transaction = null)
        {
            object result = null;

            if (transaction != null)
            {
                result = await AsyncSQLHelper.ExecuteScalarAsync(transaction.SqlTransaction, CommandType.Text, commandText, commandParameters);
            }
            else
            {
                result = await AsyncSQLHelper.ExecuteScalarAsync(SqlConnectionString, CommandType.Text, commandText, commandParameters);
            }

            return result;
        }

        #endregion

        #region 执行，返回受影响行数

        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数集合</param>
        /// <param name="transaction">不为空时，使用指定的事务处理；为空时，创建新的连接处理</param>
        /// <returns>受影响行数</returns>
        public static int ExecuteNonQuery(string commandText, SqlParameter[] commandParameters, DBTransaction transaction = null)
        {
            int result = 0;

            if (transaction != null)
            {
                result = SyncSQLHelper.ExecuteNonQuery(transaction.SqlTransaction, CommandType.Text, commandText, commandParameters);
            }
            else
            {
                result = SyncSQLHelper.ExecuteNonQuery(SqlConnectionString, CommandType.Text, commandText, commandParameters);
            }

            return result;
        }

        /// <summary>
        /// ExecuteNonQueryAsync
        /// </summary>
        /// <param name="commandText">SQL语句</param>
        /// <param name="commandParameters">参数集合</param>
        /// <param name="transaction">不为空时，使用指定的事务处理；为空时，创建新的连接处理</param>
        /// <returns>受影响行数</returns>
        public static async Task<int> ExecuteNonQueryAsync(string commandText, SqlParameter[] commandParameters, DBTransaction transaction = null)
        {
            int result = 0;

            if (transaction != null)
            {
                result = await AsyncSQLHelper.ExecuteNonQueryAsync(transaction.SqlTransaction, CommandType.Text, commandText, commandParameters);
            }
            else
            {
                result = await AsyncSQLHelper.ExecuteNonQueryAsync(SqlConnectionString, CommandType.Text, commandText, commandParameters);
            }

            return result;
        }


        #endregion


        //------Extend Methods----------

        #region 事务处理

        /// <summary>
        /// 获取事务支持的对象<para />
        /// 成功执行的最后，请调用MarkSuccess()，事务结束前会自动Commit;
        /// 发生错误或者不显式MarkSuccess()的情况下，均会自动Rollback所有修改;
        /// </summary>
        /// <param name="IsolationLevel">隔离等级</param>
        public static DBTransaction CreateTransaction(IsolationLevel IsolationLevel)
        {
            DBTransaction result = new DBTransaction(IsolationLevel);

            result.Init(SqlConnectionString);

            if (result.IsInitialize == false)
                throw new InvalidOperationException("Create transaction faild.");

            return result;
        }

        #endregion
    }
}
