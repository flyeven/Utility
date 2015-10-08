using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CommonUtility.Extension
{
    /// <summary>
    /// 拷贝拓展
    /// 拷贝原对象的属性到指定对象的属性
    /// </summary>
    public class ObjectCopyExtension
    {
        /// <summary>
        /// copy source object's properties to target object
        /// </summary>
        /// <param name="source">source object</param>
        /// <param name="target">target object</param>
        public static void CopyProperties(object source, object target)
        {
            CopyProperties(target, source, null);
        }

        /// <summary>
        /// copy source object's properties to target object
        /// </summary>
        /// <param name="source">source object</param>
        /// <param name="target">target object</param>
        /// <param name="ignoreProperties">name list of ignore property</param>
        public static void CopyProperties(object source, object target, ISet<string> ignoreProperties)
        {
            //get source object's propertyinfo
            PropertyInfo[] sourceInfos = source.GetType().GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

            //get target object's propertyinfo
            PropertyInfo[] targetInfos = target.GetType().GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            foreach (var property in sourceInfos)
            {
                //filter properties
                if (ignoreProperties != null && ignoreProperties.Contains(property.Name))
                {
                    continue;
                }

                //copy value
                var info = targetInfos.FirstOrDefault(x => x.Name == property.Name);
                if (info != null && info.CanWrite)
                    info.SetValue(target, property.GetValue(source, null), null);
            }

        }

        /// <summary>
        /// get property value from name
        /// </summary>
        /// <param name="source">source object</param>
        /// <param name="propertyName">name of property</param>
        /// <returns></returns>
        public static object GetPropertyValue(object source, string propertyName)
        {

            PropertyInfo[] propertyInfos = source.GetType().GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

            var info = propertyInfos.FirstOrDefault(x => x.Name == propertyName);

            return info.GetValue(source, null);
        }
    }
}
