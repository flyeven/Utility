using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.CommonUtility.Extension;

namespace Utility.CommonUtility.Database.StoredProcedure
{
    public class MSSQLHandler : DBHandler
    {
        public override DbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        string connectionString = ConfigurationManagerExtension.GetDBConnectStr();

        public override DBHandler AddParameter(string parameterName, object value, int size, byte scale, ParameterDirection direct)
        {
            if (!(Command is SqlCommand))
                throw new Exception("Not a Valid SQL Command");
            var command = Command as SqlCommand;

            parameterName = parameterName.Trim();
            if (String.IsNullOrEmpty(parameterName))
                throw new Exception("Empty Parameter Name NOT supported");

            if (parameterName[0] != '@')
                parameterName = "@" + parameterName;

            SqlParameter param = command.Parameters.AddWithValue(parameterName, value);

            param.Direction = direct;
            param.Size = size;
            param.Scale = scale;
            param.Precision = (byte)(size - scale);
            return this;
        }

        public override DataSet Retrieve()
        {

            if (!(Command is SqlCommand))
                throw new Exception("Not a Valid SQL Command");
            var command = Command as SqlCommand;

            SetTimeOut(command);

            foreach (SqlParameter parameter in command.Parameters)
            {
                if (parameter.Value == null)
                {
                    parameter.Value = DBNull.Value;
                }
            }

            try
            {
                command.Connection = new SqlConnection(connectionString);
                command.Connection.Open();

                var da = new SqlDataAdapter(command);
                var ds = new DataSet();
                da.Fill(ds);

                return ds;
            }
            finally
            {
                command.Connection.Close();
            }
        }

        public override object ExecuteScalar()
        {

            if (!(Command is SqlCommand))
                throw new Exception("Not a Valid SQL Command");
            var command = Command as SqlCommand;

            SetTimeOut(command);

            foreach (SqlParameter parameter in command.Parameters)
            {
                if (parameter.Value == null)
                {
                    parameter.Value = DBNull.Value;
                }
            }

            try
            {
                command.Connection = new SqlConnection(connectionString);
                command.Connection.Open();
                return command.ExecuteScalar();
            }
            finally
            {
                command.Connection.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>return value includes the number of rows affected, -1 for other operations</returns>
        public override int Execute()
        {

            if (!(Command is SqlCommand))
                throw new Exception("Not a Valid SQL Command");

            var command = Command as SqlCommand;

            SetTimeOut(command);


            foreach (SqlParameter parameter in command.Parameters)
            {
                if (parameter.Value == null)
                {
                    parameter.Value = DBNull.Value;
                }
            }

            try
            {
                command.Connection = new SqlConnection(connectionString);
                command.Connection.Open();
                return command.ExecuteNonQuery();
            }
            finally
            {
                command.Connection.Close();
                command.Dispose();
            }
        }

        public override int Execute(DbCommand[] cmdArray)
        {
            var connection = new SqlConnection(connectionString);
            SqlTransaction transaction = null;
            int r = 0;

            try
            {
                connection.Open();
                transaction = connection.BeginTransaction();

                for (int i = 0; i < cmdArray.Length; i++)
                {
                    if (!(cmdArray[i] is SqlCommand))
                        throw new Exception("Not a Valid SQL Command");
                    var command = (SqlCommand)cmdArray[i];

                    SetTimeOut(command);

                    command.Connection = connection;
                    command.Transaction = transaction;


                    foreach (SqlParameter parameter in command.Parameters)
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
            catch (SqlException)
            {
                if (transaction != null) transaction.Rollback();
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        private void SetTimeOut(SqlCommand pSqlCommand)
        {
            var cmdTimeout = ConfigurationManagerExtension.GetStringValue("CMDTimeout");
            if (cmdTimeout != null)
            {
                int commandTimeout = int.Parse(cmdTimeout);
                pSqlCommand.CommandTimeout = commandTimeout;
            }
        }

    }
}
