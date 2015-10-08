using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ThirdpartUtility.Database.StoredProcedure
{
    /// <summary>
    /// Get the AppSettings section and ConnectionStrings collection.
    /// This function uses ConfigurationManager to read the appSettings and connectionStrings configuration section.
    /// </summary>
    public class ConfigurationReader
    {
        private static NameValueCollection appSettings = null;                          //the AppSettings section
        private static ConnectionStringSettingsCollection connections = null;           //the ConnectionStrings collection

        /// <summary>
        /// private static constructors
        /// </summary>
        static ConfigurationReader()
        {
            // read app settings
            ReadAppSettings();

            //read connection strings
            ReadConnectionStrings();
        }

        #region Get the AppSettings section Value

        /// <summary>
        /// Get String Value From App Config Settings
        /// </summary>
        /// <param name="paramName">the name to find Setting</param>
        /// <returns>string value</returns>
        public static string GetStringValue(string paramName)
        {

            if (appSettings == null)
            {
                throw new Exception("App Config has not been initialize");
            }

            if (!appSettings.HasKeys())
            {
                throw new Exception("App Config has not keys");
            }

            try
            {
                string value = appSettings[paramName];

                return value;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Read App Config error:{0}", e.Message), e.InnerException);
            }
        }

        /// <summary>
        /// Get Long Value From App Config Settings
        /// </summary>
        /// <param name="paramName">the name to find Setting</param>
        /// <returns>Config Value As Long</returns>
        public static long GetLongValue(string paramName)
        {
            return long.Parse(GetStringValue(paramName));
        }

        /// <summary>
        /// Get Int Value From App Config Settings
        /// </summary>
        /// <param name="paramName">the name to find Setting</param>
        /// <returns>Config Int As Int</returns>
        public static int GetIntValue(string paramName)
        {
            return int.Parse(GetStringValue(paramName));
        }

        /// <summary>
        /// Get Bool Value From App Config Settings
        /// if setting is Y/TRUE/YES/1, then return true, otherwise, return false.
        /// </summary>
        /// <param name="paramName">the name to find Setting</param>
        /// <returns>Config Bool As bool</returns>
        public static bool GetBoolValue(string paramName)
        {
            string value = GetStringValue(paramName);

            if (value == null)
            {
                return false;
            }

            value = value.ToLower();

            if ("y".Equals(value)
                || "true".Equals(value)
                || "yes".Equals(value)
                || "1".Equals(value))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Get Double Value From App Config Settings
        /// </summary>
        /// <param name="paramName">the name to find Setting</param>
        /// <returns>Config Value As Double</returns>
        public static double GetDoubleValue(string paramName)
        {
            return double.Parse(GetStringValue(paramName));
        }

        /// <summary>
        /// Get DateTime Value From App Config Settings
        /// </summary>
        /// <param name="paramName">the name to find Setting</param>
        /// <returns>Config Value As DateTime</returns>
        public static DateTime GetDateTimeValue(string paramName)
        {
            return DateTime.Parse(GetStringValue(paramName));
        }

        #endregion  // Get the AppSettings section Value

        #region Get the ConnectionStrings Settings Value

        /// <summary>
        /// get database connection string
        /// connect string ref: http://wenku.baidu.com/view/0c2a1734ee06eff9aef807ea.html
        /// </summary>
        /// <param name="databaseName">
        /// e.g. if has many db to connect,and wannt to get "Bullion" db connection str now,
        /// DBName should be "Bullion",the appconfig is "Bullion_DB_IP、Bullion_DB_PSW"...and so on
        /// if dbname is blank,can be null or empty
        /// </param>
        /// <returns>database connection string</returns>
        public static string GetDBConnectStr(string databaseName = null)
        {
            /** Note: 
             * Configuration is as follows
             * <add key="ENCRYPTED_FIELDS" value=""/>
               <add key="DB_USER" value="sa"/>
               <add key="DB_DATABASE" value="ESignature"/>
               <add key="DB_PSW" value="Pw123456"/>
               <add key="DB_IP" value="172.30.1.129"/>
               <add key="DB_PORT" value=""/>
               <add key="DB_TYPE" value="1"/><!-- dbtype:0=oracle, 1=sqlserver, 2=mysql, -->
             * **/

            databaseName += string.IsNullOrEmpty(databaseName) ? "" : "_";

            DataBaseTYPE dbtype = (DataBaseTYPE)GetIntValue(databaseName + "DB_TYPE");

            string user = GetStringValue(databaseName + "DB_USER");
            string schema = GetStringValue(databaseName + "DB_DATABASE");
            string IP = GetStringValue(databaseName + "DB_IP");
            string port = GetStringValue(databaseName + "DB_PORT");
            string psw = GetStringValue(databaseName + "DB_PSW");

            StringBuilder connectStr = new StringBuilder();

            switch (dbtype)
            {
                case DataBaseTYPE.SQLSERVER:

                    #region sql server

                    //Data Source=172.30.1.129;uid=sa;password=Pw123456;database=EMPPortal

                    connectStr.Append("User ID=");
                    connectStr.Append(user);
                    connectStr.Append(";initial catalog=");
                    connectStr.Append(schema);
                    connectStr.Append(";Data Source=");
                    connectStr.Append(IP);
                    if (port != null && !port.Equals(""))
                    {
                        connectStr.Append(",").Append(port);
                    }
                    connectStr.Append(";Password=");
                    connectStr.Append(psw);
                    connectStr.Append(";");

                    #endregion
                    break;
                case DataBaseTYPE.ORACLE:

                    #region oracle
                    connectStr.Append("Data Source=");
                    connectStr.Append(IP);
                    if (port != null && !port.Equals(""))
                    {
                        connectStr.Append(":").Append(port);
                    }
                    connectStr.Append("/");
                    connectStr.Append(schema);
                    connectStr.Append(";user id=");
                    connectStr.Append(user);
                    connectStr.Append(";password=");
                    connectStr.Append(psw);
                    connectStr.Append(";");
                    #endregion
                    break;
                case DataBaseTYPE.MYSQL:

                    #region mysql
                    // server=localhost;User Id=root;database=hknd;Character Set=latin1
                    connectStr.Append("server=").Append(IP);
                    if (port != null && !port.Equals(""))
                    {
                        connectStr.Append(";port=");
                        connectStr.Append(port);
                    }
                    connectStr.Append(";database=");
                    connectStr.Append(schema);
                    connectStr.Append(";User Id=");
                    connectStr.Append(user);
                    connectStr.Append(";password=");
                    connectStr.Append(psw);

                    string charSet = GetStringValue(databaseName + "CHARACTER_SET");

                    if (!string.IsNullOrEmpty(charSet))
                    {
                        connectStr.Append(";Character Set=");
                        connectStr.Append(charSet);
                    }
                    connectStr.Append(";");
                    #endregion
                    break;
                default:
                    break;
            }

            return connectStr.ToString();
        }

        /// <summary>
        /// Get Connection String From Config Settings
        /// </summary>
        /// <param name="index">default value = 0</param>
        /// <returns>Connection String</returns>
        public static string GetConnectionString(int index = 0)
        {
            /** Notes:
             *  Configuration is as follows
             *   <connectionStrings>
                    <add name="DataAccess_DBConnection" providerName="System.Data.SqlClient" connectionString="Data Source=172.30.1.129;uid=sa;password=Pw123456;database=EMPPortal" />
                  </connectionStrings>
             */

            if (connections == null)
            {
                throw new Exception("Connection strings has not been initialize");
            }

            if (connections.Count == 0)
            {
                throw new Exception("Connection strings has not keys");
            }

            try
            {
                ConnectionStringSettings setting = connections[index];

                return setting.ConnectionString;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Read connection string settings error:{0}", e.Message), e.InnerException);
            }
        }

        #endregion //Get the ConnectionStrings Settings Value


        //---------------- private methods

        /// <summary>
        /// Get the AppSettings section
        /// </summary>
        private static void ReadAppSettings()
        {
            try
            {
                // Get the AppSettings section.
                appSettings = ConfigurationManager.AppSettings;
            }
            catch (ConfigurationErrorsException e)
            {
                throw new Exception(string.Format("ReadAppSettings Error: {0}", e.Message), e.InnerException);
            }
        }

        /// <summary>
        /// Get the ConnectionStrings section
        /// </summary>
        private static void ReadConnectionStrings()
        {
            try
            {
                // Get the ConnectionStrings collection.
                connections = ConfigurationManager.ConnectionStrings;
            }
            catch (ConfigurationErrorsException e)
            {
                throw new Exception(string.Format("ReadConnectionStrings Error: {0}", e.Message), e.InnerException);
            }
        }

        /// <summary>
        /// type of Database
        /// </summary>
        enum DataBaseTYPE : int
        {
            SQLSERVER = 1,
            ORACLE = 2,
            MYSQL = 3
        }
    }
}
