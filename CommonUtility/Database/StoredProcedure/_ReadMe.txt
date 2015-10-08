Demo:
//创建sql server的处理类
DBHandler _dbHandler = new MSSQLHandler();

//创建命令，传入存储过程名称
dbHandler.CreateCommand("StoredProcedure Name");

//添加参数
_dbHandler.AddParameter("Parameter1", value1);
_dbHandler.AddParameter("Parameter2", value2);
...

//添加输出参数
_dbHandler.AddParameter("Parameter3", value3, ParameterDirection.Output); //存储过程必须添加out标记
...

//执行命令
_dbHandler.ExecuteScalar()
_dbHandler.Execute()
_dbHandler.Query()
_dbHandler.Retrieve();

//获取输出参数
value3 = _dbHandler.Command.Parameters["@Parameter3"].Value as typeof(value3);