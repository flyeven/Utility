using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = @"log4net.config", Watch = true)]

namespace Utility.ThirdpartUtility.Log
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public sealed class Logger
    {

        #region 构造函数

        static Logger()
        {

        }

        #endregion

        #region log4Net

        private static bool _isInitialize = false;

        /// <summary>
        /// 配置Log4net
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static bool ConfigureLog4Net(string configPath)
        {
            try
            {
                //配置log4Net
                log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(configPath));
                _isInitialize = true;
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("log4Net配置过程失败：" + ex.Message);
                _isInitialize = false;
                return false;
            }
        }

        /// <summary>
        /// 检查log4Net的配置情况
        /// </summary>
        private static void CheckInitialized()
        {
            if (_isInitialize == false)
                throw new InvalidOperationException("使用Log4Net对象之前，请先调用ConfigureLog4Net接口，进行配置。");
        }

        /// <summary>
        /// 获取log4Net类型的日志记录器(只读)
        /// </summary>
        public static ILog Log4Net
        {
            get
            {
                CheckInitialized();

                //获取默认类型的记录器(调用方的方法的类)
                Type targetType = new StackFrame(1).GetMethod().DeclaringType;
                return LogManager.GetLogger(targetType);
            }
        }

        /// <summary>
        /// 获取log4Net类型的日志记录器<para />
        /// (如果确实有需要，需要指定为其他类型，则使用此方法)
        /// </summary>
        /// <typeparam name="T">日志记录器对应的模块类型</typeparam>
        public static ILog Log4NetWithType<T>()
        {
            CheckInitialized();

            return LogManager.GetLogger(typeof(T));
        }

        #endregion

    }
}
