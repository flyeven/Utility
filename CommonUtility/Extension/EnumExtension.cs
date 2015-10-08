using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
        /// Get Enum Description
        /// </summary>
        /// <param name="value">Enum Value</param>
        /// <returns>Enum Description</returns>
        /// <from>http://stackoverflow.com/questions/9126228/how-to-set-string-in-enum-c</from>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        /// <summary>
        /// Convert a string to an enumeration value
        /// </summary>
        /// <typeparam name="T">enum type</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <from>http://stackoverflow.com/questions/16100/how-do-i-convert-a-string-to-an-enum-in-c</from>
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
