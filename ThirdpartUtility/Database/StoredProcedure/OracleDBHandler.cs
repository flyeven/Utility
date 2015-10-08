using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utility.ThirdpartUtility.Database.StoredProcedure
{
    public class OracleDBHandler : DBHandler
    {
        public override DbCommand CreateCommand()
        {
            return new Oracle.DataAccess.Client.OracleCommand();
        }

        string connectionString = ConfigurationReader.GetDBConnectStr(null);

        public override DBHandler AddParameter(string parameterName, object value, int size, byte scale, ParameterDirection direct)
        {
            if (!(Command is Oracle.DataAccess.Client.OracleCommand))
                throw new Exception("Not a Valid SQL Command");
            var command = Command as Oracle.DataAccess.Client.OracleCommand;

            parameterName = parameterName.Trim();
            if (String.IsNullOrEmpty(parameterName))
                throw new Exception("Empty Parameter Name NOT supported");

            if (parameterName[0] != ':')
                parameterName = ":" + parameterName;

            Oracle.DataAccess.Client.OracleParameter param = command.Parameters.Add(parameterName, value);

            param.Direction = direct;
            param.Size = size;
            param.Scale = scale;
            param.Precision = (byte)(size - scale);
            return this;
        }

        public DBHandler AddCursorParameter(string parameterName)
        {

            var command = Command as Oracle.DataAccess.Client.OracleCommand;
            Oracle.DataAccess.Client.OracleParameter oracleParameter = new Oracle.DataAccess.Client.OracleParameter(parameterName, Oracle.DataAccess.Client.OracleDbType.RefCursor);
            oracleParameter.Direction = ParameterDirection.Output;
            command.Parameters.Add(oracleParameter);

            return this;
        }


        public override DataSet Retrieve()
        {

            if (!(Command is Oracle.DataAccess.Client.OracleCommand))
                throw new Exception("Not a Valid SQL Command");
            var command = Command as Oracle.DataAccess.Client.OracleCommand;

            SetTimeOut(command);

            foreach (Oracle.DataAccess.Client.OracleParameter parameter in command.Parameters)
            {
                if (parameter.Value == null)
                {
                    parameter.Value = DBNull.Value;
                }
            }

            try
            {
                command.Connection = new Oracle.DataAccess.Client.OracleConnection(connectionString);
                command.Connection.Open();

                var da = new Oracle.DataAccess.Client.OracleDataAdapter(command);
                var ds = new DataSet();
                da.Fill(ds);

                return ds;
            }
            catch (Exception ex)
            {
                string exceptionMessage = GetOracleExceptionMessage(ex);

                throw new Exception(exceptionMessage);
            }
            finally
            {
                command.Connection.Close();
            }
        }

        public override object ExecuteScalar()
        {

            if (!(Command is Oracle.DataAccess.Client.OracleCommand))
                throw new Exception("Not a Valid SQL Command");
            var command = Command as Oracle.DataAccess.Client.OracleCommand;

            SetTimeOut(command);

            foreach (Oracle.DataAccess.Client.OracleParameter parameter in command.Parameters)
            {
                if (parameter.Value == null)
                {
                    parameter.Value = DBNull.Value;
                }
            }

            try
            {
                command.Connection = new Oracle.DataAccess.Client.OracleConnection(connectionString);
                command.Connection.Open();
                return command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                string exceptionMessage = GetOracleExceptionMessage(ex);

                throw new Exception(exceptionMessage);
            }
            finally
            {
                command.Connection.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Command"></param>
        /// <returns>return value includes the number of rows affected, -1 for other operations</returns>
        public override int Execute()
        {

            if (!(Command is Oracle.DataAccess.Client.OracleCommand))
                throw new Exception("Not a Valid SQL Command");

            var command = Command as Oracle.DataAccess.Client.OracleCommand;

            SetTimeOut(command);


            foreach (Oracle.DataAccess.Client.OracleParameter parameter in command.Parameters)
            {
                if (parameter.Value == null)
                {
                    parameter.Value = DBNull.Value;
                }
            }

            try
            {
                command.Connection = new Oracle.DataAccess.Client.OracleConnection(connectionString);
                command.Connection.Open();
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string exceptionMessage = GetOracleExceptionMessage(ex);

                throw new Exception(exceptionMessage);
            }
            finally
            {
                command.Connection.Close();
                command.Dispose();
            }
        }

        public override int Execute(DbCommand[] cmdArray)
        {
            var connection = new Oracle.DataAccess.Client.OracleConnection(connectionString);
            Oracle.DataAccess.Client.OracleTransaction transaction = null;
            int r = 0;

            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                for (int i = 0; i < cmdArray.Length; i++)
                {
                    if (!(cmdArray[i] is Oracle.DataAccess.Client.OracleCommand))
                        throw new Exception("Not a Valid SQL Command");
                    var command = (Oracle.DataAccess.Client.OracleCommand)cmdArray[i];

                    SetTimeOut(command);

                    command.Connection = connection;
                    command.Transaction = transaction;


                    foreach (Oracle.DataAccess.Client.OracleParameter parameter in command.Parameters)
                    {
                        if (parameter.Value == null)
                        {
                            parameter.Value = DBNull.Value;
                        }
                    }

                    r += command.ExecuteNonQuery();
                }
                transaction.Commit();
                return r;
            }
            catch (Exception ex)
            {
                if (transaction != null) transaction.Rollback();
                string exceptionMessage = GetOracleExceptionMessage(ex);

                throw new Exception(exceptionMessage);
            }
            finally
            {
                connection.Close();
            }
        }

        private void SetTimeOut(Oracle.DataAccess.Client.OracleCommand pSqlCommand)
        {
            var cmdTimeout = ConfigurationReader.GetStringValue("CmdTimeout");
            if (cmdTimeout != null)
            {
                int commandTimeout = int.Parse(cmdTimeout);
                pSqlCommand.CommandTimeout = commandTimeout;
            }
        }

        private string GetOracleExceptionMessage(Exception ex)
        {
            string exceptionMessage = "Oracle_Database_Exception";
            Regex regex = new Regex(@"\s*ORA-(\d{5}):\s*(.*?)\n.*?");
            MatchCollection ms = regex.Matches(ex.Message);
            if (ms != null && ms.Count > 0)
            {
                int exceptionCode = Convert.ToInt32(ms[0].Groups[1].Value);
                if (exceptionCode >= 20000 && exceptionCode <= 20999)  //20000-20999: the oracle custom exception code
                {
                    exceptionMessage = ms[0].Groups[2].Value;
                }
            }
            return exceptionMessage;
        }
    }
}
