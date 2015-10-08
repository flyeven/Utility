using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ThirdpartUtility.Database.StoredProcedure
{
    /// <summary>
    /// Abstract class that provides handling of SQL commands.
    /// </summary>
    public abstract class DBHandler
    {
        public DbCommand Command { get; set; }

        #region Create Command

        public abstract DbCommand CreateCommand();
        /// <summary>
        /// Get a command for stored procedure
        /// </summary>
        /// <param name="pStoredProcedureName"></param>
        /// <returns></returns>
        public DbCommand CreateCommand(string pStoredProcedureName)
        {
            Command = CreateCommand();
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = pStoredProcedureName;
            return Command;
        }

        #endregion

        #region Add Parameter
        /// <summary>
        /// Add parameter, Value contains decimal places need to set size and scale
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <param name="size"></param>
        /// <param name="scale"></param>
        /// <param name="direct"></param>
        public abstract DBHandler AddParameter(string parameterName, object value, int size, byte scale, ParameterDirection direct);

        public DBHandler AddParameter(string parameterName, object value)
        {
            return AddParameter(parameterName, value, ParameterDirection.Input);
        }

        public DBHandler AddParameter(string parameterName, object value, int size, byte scale)
        {
            return AddParameter(parameterName, value, size, scale, ParameterDirection.Input);
        }

        public DBHandler AddParameter(string parameterName, object value, ParameterDirection direct)
        {
            return AddParameter(parameterName, value, 0, 0, direct);
        }

        public DBHandler AddParameter(string parameterName, object value, Type t)
        {
            return AddParameter(parameterName, ConvertToType(value, t));
        }
        public DBHandler AddParameter(string parameterName, object value, Type t, ParameterDirection direct)
        {
            return AddParameter(parameterName, ConvertToType(value, t), direct);
        }
        public DBHandler AddParameter(string parameterName, object value, Type t, int size, ParameterDirection direct)
        {
            return AddParameter(parameterName, ConvertToType(value, t), size, 0, direct);
        }

        public void ParseParameter(object pEntity)
        {
            var obj2SQLParameter = new Object2SQLParameter();
            obj2SQLParameter.Parse(pEntity);
            for (int i = 0; i < obj2SQLParameter.ParameterNames.Count; i++)
            {
                AddParameter(obj2SQLParameter.ParameterNames[i], obj2SQLParameter.ParameterValues[i]);
            }
        }


        // Help function for data Conversion
        private object ConvertToType(Object o, Type t)
        {
            if (o == null) return null;
            // Implement
            // 1 Convert string to Guid, which converter Does NOT handle 
            // The rest, convert should be able to convert.
            try
            {
                if (t.Name == "Guid" && o is string)
                {
                    Guid id = new Guid((string)o);
                    return id;
                }

                return Convert.ChangeType(o, t);
            }
            catch
            {
                throw new Exception("Unable to Cast Object to desired type: " + o.GetType().Name + " ," + t.Name);
            }
        }
        #endregion

        #region Retrieve

        public abstract DataSet Retrieve();

        #endregion

        #region Execute

        public abstract int Execute();

        public abstract int Execute(DbCommand[] cmdArray);

        public abstract object ExecuteScalar();

        #endregion

        #region Query

        public DataTable Query()
        {
            DataSet ds = Retrieve();

            if (ds == null) return null;
            if (ds.Tables == null) return null;
            if (ds.Tables.Count == 0) return null;
            if (ds.Tables[0].Rows == null) return null;
            if (ds.Tables[0].Rows.Count == 0) return null;

            return ds.Tables[0];
        }

        #endregion




    }
}
