【MultipleValues4ApiAction】目录  允许向WebApi的Action，传递多个参数。

*目前只限定，允许传递 基元类型 的参数；
[HttpPost]
DoAction(string name,string pwd);

*复合类型的，请直接构造一个Class类作为Action的唯一参数。
[HttpPost]
DoAction([FromBody]DataClass datas);


-------------------------------------

使用步骤：
1.在对应的Asp.net的Global，优先顺序插入我们的“参数绑定器”:
  GlobalConfiguration.Configuration.ParameterBindingRules.Insert(0,Utility.SimplePostVariableParameterBinding.HookupParameterBinding);

2.在要使用此功能的Put或Post方法上，修饰MultiParameterSupportAttribute特性:
  [MultiParameterSupport]
  DoAction([FromBody]DataClass datas);
  
