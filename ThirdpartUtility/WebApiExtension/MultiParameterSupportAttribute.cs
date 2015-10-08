using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.ThirdpartUtility.WebApiExtension
{
    /// <summary>
    /// 标记目标方法，允许使用SimplePostVariableParameterBinding的处理<para />
    /// (具体用法，请参数 ReadMe.txt )
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MultiParameterSupportAttribute : Attribute
    {

    }
}
