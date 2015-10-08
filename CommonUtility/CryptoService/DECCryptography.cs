using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CommonUtility.CryptoService
{
    /// <summary>
    /// DEC 可逆加密服务
    /// </summary>
    public sealed class DECCryptography
    {

        //----------Members-----------

        /// <summary>
        /// DEC加密钥匙  8个字符，64位
        /// </summary>
        private readonly string _cryptographyKey;


        /// <summary>
        /// 是否属于可逆加密
        /// </summary>
        public bool IsDecryptable
        {
            get { return true; }
        }

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cryptographyKey">DEC加密钥匙,8个字符,64位</param>
        public DECCryptography(string cryptographyKey)
        {
            //参数检查
            if (String.IsNullOrEmpty(cryptographyKey) || cryptographyKey.Length != 8)
                throw new ArgumentException("指定的加密钥匙不合法。DEC加密钥匙为有效8个字符组成的字符串。", "cryptographyKey");

            this._cryptographyKey = cryptographyKey;
        }

        #endregion


        #region 加密字符串

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="basicData">源字符串</param>
        /// <returns>加密后的字符串</returns>
        public string EncryptString(string basicData)
        {
            DESCryptoServiceProvider provider = null;
            MemoryStream ms = null;
            CryptoStream cst = null;
            StreamWriter sw = null;

            try
            {
                byte[] byteKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_cryptographyKey);

                provider = new DESCryptoServiceProvider();
                ms = new MemoryStream();
                cst = new CryptoStream(ms, provider.CreateEncryptor(byteKey, byteKey), CryptoStreamMode.Write);

                sw = new StreamWriter(cst);
                sw.Write(basicData);
                sw.Flush();
                cst.FlushFinalBlock();
                sw.Flush();

                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (provider != null)
                    provider.Dispose();
                if (ms != null)
                    ms.Dispose();
                if (cst != null)
                    cst.Dispose();
                if (sw != null)
                    sw.Dispose();
            }
        }

        #endregion

        #region 解密字符串

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="basicData">加密了的字符串</param>
        /// <returns>解密后的字符串</returns>
        public string DecryptString(string basicData)
        {
            DESCryptoServiceProvider provider = null;
            MemoryStream ms = null;
            CryptoStream cst = null;
            StreamReader sr = null;

            try
            {
                byte[] byteEnc;
                byte[] byteKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_cryptographyKey);

                try
                {
                    byteEnc = Convert.FromBase64String(basicData);
                }
                catch
                {
                    return null;
                }

                provider = new DESCryptoServiceProvider();
                ms = new MemoryStream(byteEnc);
                cst = new CryptoStream(ms, provider.CreateDecryptor(byteKey, byteKey), CryptoStreamMode.Read);
                sr = new StreamReader(cst);
                return sr.ReadToEnd();
            }
            catch
            {
                return null;
            }
            finally
            {
                if (sr != null)
                    sr.Dispose();
                if (cst != null)
                    cst.Dispose();
                if (ms != null)
                    ms.Dispose();
                if (provider != null)
                    provider.Dispose();
            }
        }

        #endregion

    }
}
