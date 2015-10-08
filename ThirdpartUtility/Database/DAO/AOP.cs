using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utility.ThirdpartUtility.Database.DAO
{
    /// <summary>
    /// 事务管理AOP，
    /// </summary>
    public class TransactionAOP : IInterceptor
    {
        private string connectString = null;

        private short count = 0;

        public TransactionAOP()
        {
        }

        public TransactionAOP(string connectString)
        {
            this.connectString = connectString;
        }

        public void Intercept(IInvocation invocation)
        {
            if (count > 0)
            {
                // 嵌套调用
                invocation.Proceed();
                return;
            }

            count++;
            IDbTransaction transaction = null;
            string DBName = "";

            object[] attrs = invocation.TargetType.GetCustomAttributes(typeof(DBSourceAttribute), false);
            if (attrs != null && attrs.Length > 0)
            {
                DBName = (attrs[0] as DBSourceAttribute).name;
            }

            //short dbType = 0;
            short dbType = ConfigCache.GetDBType(DBName);

            // 从类中指定，
            // 从方法中指定
            if (connectString == null)
            {

                //connectString = System.Configuration.ConfigurationManager.ConnectionStrings["CONNECTION_STRING"].ConnectionString;
                connectString = ConfigCache.GetDBConnectStr(DBName);

                //connectString = "Data Source=172.30.1.65:1522//xe;;user id=etl;password=etl;";
                //connectString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=Virgil-Chen)(PORT=1522)))(CONNECT_DATA=(SERVICE_NAME=XE)));User Id=czjd_sd;Password=czjd_sd;";
                //connectString = "Data Source=Virgil-PC:1522//xe;;user id=czjd_sd;password=czjd_sd;";
                //connectString = "server=localhost;User Id=root;database=sakila;Password=root;";
            }

            using (IDbConnection connection = DBUtil.GetConnection(dbType, connectString))
            {
                try
                {
                    connection.Open();

                    string methodName = invocation.MethodInvocationTarget.Name.ToLower();
                    if (!(methodName.StartsWith("get")
                                || methodName.StartsWith("find")
                                || methodName.StartsWith("query")
                                || methodName.StartsWith("list")))
                    {

                        transaction = connection.BeginTransaction();
                    }

                    new TransactionContext(connection, transaction, dbType);

                    invocation.Proceed();

                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                catch (Exception e)
                {
                    if (transaction != null)
                    {
                        transaction.Rollback();
                    }


                    throw e;
                }
                finally
                {

                    if (connection != null && ConnectionState.Open == connection.State)
                    {
                        connection.Close();
                    }
                    count--;
                }
            }

        }



        /// <summary>
        /// TransactionAOP的代理类创建，被代理对象具有数据库连接及事务管理功能，
        /// 但对象的方法，必须声明为virtual，方法中，可以直接使用ADOTemplate。
        /// 
        /// <code>
        /// BizBO bizBo = TransactionAOP.newInstance(typeof(BizBO)) as BizBO;
        /// 
        /// bizBo.add();//BizBO中，add方法，需要声明为virtual。
        /// 
        /// </code>
        /// 
        /// </summary>
        /// <param name="classType">被代理对象的类型</param>
        /// <returns>对象的代理</returns>
        public static object newInstance(Type classType)
        {
            return new ProxyGenerator().CreateClassProxy(classType, new TransactionAOP());
        }

        /// <summary>
        /// 
        /// TransactionAOP的代理类创建，被代理对象具有数据库连接及事务管理功能，
        /// 但对象的方法，必须声明为virtual，方法中，可以直接使用ADOTemplate。
        /// 
        /// <code>
        /// BizBO bizBo = TransactionAOP.new<BizBO>();
        /// 
        /// bizBo.add();//BizBO中，add方法，需要声明为virtual。
        /// 
        /// </code>
        /// </summary>
        /// <typeparam name="T">被代理对象的类型</typeparam>
        /// <returns>对象的代理</returns>
        public static T New<T>() where T : class
        {
            var obj = newInstance(typeof(T));
            return obj as T;
        }
    }

    public class AOPUtil
    {
        public static object newInstance(Type classType)
        {
            return new ProxyGenerator().CreateClassProxy(classType, new TransactionAOP());
        }

        public static object newInstance(Type classType, string connectionString)
        {
            return new ProxyGenerator().CreateClassProxy(classType, new TransactionAOP(connectionString));
        }
    }


    class TransactionContext
    {
        private static ThreadLocal<TransactionContext> local = new ThreadLocal<TransactionContext>();


        public TransactionContext(IDbConnection connection, IDbTransaction transaction, short dbType)
        {
            this.connection = connection;
            this.transaction = transaction;
            this.dbType = dbType;

            local.Value = this;
        }

        public static TransactionContext get()
        {
            return local.Value;
        }

        public IDbConnection connection { get; set; }
        public IDbTransaction transaction { get; set; }
        public short dbType { get; set; }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class DBSourceAttribute : Attribute
    {
        public readonly string name;
        public DBSourceAttribute(string name)
        {
            this.name = name;
        }
    }
}
