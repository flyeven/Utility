using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CommonUtility.Database
{
    /// <summary>
    /// 事务对象封装
    /// </summary>
    public class DBTransaction : IDisposable
    {
        //---------------------  Properies

        #region  Properies

        /// <summary>
        /// 隔离等级
        /// </summary>
        public IsolationLevel IsolationLevel { get; private set; }

        /// <summary>
        /// 标记事务是否结束
        /// </summary>
        private bool _isTransactionEnded = false;

        /// <summary>
        /// 标记执行是否顺利
        /// </summary>
        private bool _isSuccess = false;


        /// <summary>
        /// 数据库连接对象
        /// </summary>
        internal SqlConnection SqlConnection { get; private set; }

        /// <summary>
        /// 事务对象
        /// </summary>
        internal SqlTransaction SqlTransaction { get; private set; }

        #endregion Properies


        //---------------------  Methods

        #region Constructer

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="transactionPurpose">事务用途</param>
        /// <param name="isolationLevel">隔离等级</param>
        public DBTransaction(IsolationLevel isolationLevel)
        {
            this.IsolationLevel = isolationLevel;
        }

        /// <summary>
        /// 析构
        /// </summary>
        ~DBTransaction()
        {
            Dispose(false);
        }
        #endregion

        #region IDisposable

        /// <summary>
        /// 是否已经完成释放
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                //结束事务
                EndTransaction();

                //释放托管资源
                DisposeManagedResource();
            }

            //释放非托管资源
            IsDisposed = true;
        }

        /// <summary>
        /// 释放托管资源
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void DisposeManagedResource()
        {
            if (SqlTransaction != null)
            {
                SqlTransaction.Dispose();
                SqlTransaction = null;
            }

            if (SqlConnection != null)
            {
                SqlConnection.Dispose();
                SqlConnection = null;
            }
        }

        /// <summary>
        /// 结束事务
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void EndTransaction()
        {
            if (SqlTransaction != null && _isTransactionEnded == false)
            {
                if (this._isSuccess)
                    SqlTransaction.Commit();
                else
                    SqlTransaction.Rollback();

                //标记结束
                _isTransactionEnded = true;
            }
        }

        #endregion


        #region 初始化

        public bool IsInitialize { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void Init(string connectionString)
        {
            try
            {
                this.SqlConnection = new SqlConnection(connectionString);
                SqlConnection.Open();
                this.SqlTransaction = SqlConnection.BeginTransaction(IsolationLevel);

                IsInitialize = true;
            }
            catch (Exception e)
            {
                this.Dispose();
                IsInitialize = false;

                throw e;
            }
        }

        #endregion

        #region 标记执行成功

        /// <summary>
        /// 标记执行成功(事务结束之前，会执行Commit；否则，会执行Rollback)
        /// </summary>
        public void MarkSuccess()
        {
            this._isSuccess = true;
        }

        #endregion
    }
}
