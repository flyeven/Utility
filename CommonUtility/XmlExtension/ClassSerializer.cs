using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Utility.CommonUtility.Xml
{
    /// <summary>
    /// 类的“序列化反序列化”转换类
    /// </summary>
    public class ClassSerializer
    {
        #region 根据指定路径进行序列化和反序列化

        /// <summary>
        /// 根据指定路径进行序列化
        /// </summary>
        /// <typeparam name="T">Object引用类型</typeparam>
        /// <param name="config">需要进行序列化的对象</param>
        /// <param name="filePath">文件路径（相对路径，绝对路径均可）</param>
        /// <returns>是否成功</returns>
        public static bool SerializeWithFilePath<T>(T config, string filePath) where T : class
        {
            bool result = true;

            try
            {
                //创建文件流
                using (FileStream writer = new FileStream(filePath, FileMode.Create))
                {
                    result = Serialize<T>(config, writer);
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 根据指定路径进行反序列化
        /// </summary>
        /// <typeparam name="T">Object引用类型</typeparam>
        /// <param name="config">需要进行序列化的对象</param>
        /// <param name="filePath">文件路径（相对路径，绝对路径均可）</param>
        /// <returns>是否成功</returns>
        public static bool DeserializeWithFilePath<T>(ref T config, string filePath) where T : class
        {
            bool result = true;

            try
            {
                //判断文件是否存在
                if (File.Exists(filePath) == false)
                {
                    result = false;
                }
                else
                {
                    //创建文件流
                    using (FileStream writer = new FileStream(filePath, FileMode.Open))
                    {
                        result = Deserialize<T>(ref config, writer);
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }


        #endregion

        #region 根据内存流进行序列化和反序列化

        /// <summary>
        /// 根据内存流进行序列化
        /// </summary>
        /// <typeparam name="T">Object引用类型</typeparam>
        /// <param name="config">需要进行序列化的对象</param>
        /// <param name="ms">MemoryStream</param>
        /// <returns>是否成功</returns>
        public static bool SerializeWithMemoryStream<T>(T config, MemoryStream ms) where T : class
        {
            return Serialize<T>(config, ms);
        }

        /// <summary>
        /// 根据内存流进行反序列化
        /// </summary>
        /// <typeparam name="T">Object引用类型</typeparam>
        /// <param name="config">需要进行序列化的对象</param>
        /// <param name="ms">MemoryStream</param>
        /// <returns>是否成功</returns>
        public static bool DeserializeWithMemoryStream<T>(ref T config, MemoryStream ms) where T : class
        {
            return Deserialize<T>(ref config, ms);
        }

        #endregion


        //---------Private-----------

        #region 基本的 序列化和反序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T">Object引用类型</typeparam>
        /// <param name="config">需要进行序列化的对象</param>
        /// <param name="stream">流对象(可以为FileStream或MemoryStream等)</param>
        /// <returns>是否成功</returns>
        private static bool Serialize<T>(T config, Stream stream) where T : class
        {
            bool result = true;

            try
            {
                //序列化
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, config);
            }
            catch (Exception)
            {
                result = false;
            }
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">Object引用类型</typeparam>
        /// <param name="config">需要进行序列化的对象</param>
        /// <param name="stream">流对象(可以为FileStream或MemoryStream等)</param>
        /// <returns>是否成功</returns>
        private static bool Deserialize<T>(ref T config, Stream stream) where T : class
        {
            bool result = true;

            //检查Stream
            if (stream == null)
                return false;
            if (stream.CanRead == false)
                return false;

            //每次反序列化时，使用新的数据。
            config = default(T);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                config = (T)serializer.Deserialize(stream);
            }
            catch (Exception)
            {
                result = false;
            }
            finally
            {

            }

            return result;
        }
        #endregion
    }
}
