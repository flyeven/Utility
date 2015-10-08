using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CommonUtility.CryptoService
{
    /// <summary>
    /// MD5 不可逆加密服务
    /// </summary>
    public sealed class MD5Cryptography
    {
        /// <summary>
        /// 是否属于可逆加密
        /// </summary>
        public bool IsDecryptable
        {
            get { return false; }
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="basicData">源字符串</param>
        /// <returns>加密后的字符串</returns>
        public string EncryptString(string basicData)
        {
            if (basicData == null)
                basicData = String.Empty;

            MD5 md5Creator = MD5.Create();
            StringBuilder sb = new StringBuilder();

            byte[] pwdData = md5Creator.ComputeHash(Encoding.UTF8.GetBytes(basicData));
            for (int i = 0; i < pwdData.Length; i++)
            {
                sb.Append(pwdData[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
