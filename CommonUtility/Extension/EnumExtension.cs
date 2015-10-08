using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CommonUtility.Extension
{
    /// <summary>
    /// 枚举类型拓展
    /// </summary>
    public class EnumExtension
    {
        /// <summary>
        /// 获取枚举类型的Description特性（Attribute）
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <returns>Description特性</returns>
        public static string GetDescription(Enum value)
        {
            var attributes = (DescriptionAttribute[])value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            //如果当前枚举值不存在Description，则返回当前值（ToString）
            return attributes != null && attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}
