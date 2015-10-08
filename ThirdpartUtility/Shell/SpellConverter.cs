using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utility.ThirdpartUtility.Shell
{
    /// <summary>
    /// 汉字拼音转换器
    /// 引用 ChnCharInfo.dll 
    /// 使用微软的 Simplified Chinese Pin-Yin Conversion Library 方案。
    /// </summary>
    public class SpellConverter
    {
        /// <summary>
        /// 汉字拼音转换器对象
        /// </summary>
        public static readonly SpellConverter Instance = new SpellConverter();

        /// <summary>
        /// 获取汉字字符串的拼音首字符字符串
        /// </summary>
        /// <param name="wordStr"></param>
        /// <returns></returns>
        public string GetSpellHeadChars(string wordStr)
        {
            return GetSpellStr(wordStr, true);
        }

        /// <summary>
        /// 获取汉字字符串的拼音字符串
        /// </summary>
        /// <param name="wordStr"></param>
        /// <returns></returns>
        public string GetSpellAllChars(string wordStr)
        {
            return GetSpellStr(wordStr, false);
        }

        /// <summary>
        /// 获取拼音字符串
        /// </summary>
        /// <param name="wordStr">汉字数组</param>
        /// <param name="HeadOrAll">true为Head,false为All</param>
        /// <returns></returns>
        private string GetSpellStr(string wordStr, bool HeadOrAll)
        {
            if (string.IsNullOrEmpty(wordStr))
            {
                return string.Empty;
            }
            // 匹配中文字符   
            Regex regexChinese = new Regex("^[\u4e00-\u9fa5]$");

            string result = string.Empty;
            string[] tmpArr;
            char[] arrayChars = wordStr.ToArray<char>();

            foreach (char item in arrayChars)
            {
                if (regexChinese.IsMatch(item.ToString()))
                {
                    ChineseChar data = new ChineseChar(item);
                    if (data.PinyinCount > 0)
                    {
                        if (!string.IsNullOrEmpty(result))
                        {
                            tmpArr = result.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            result = string.Empty;
                            for (int i = 0; i < tmpArr.Length; i++)
                            {
                                for (int j = 0; j < data.PinyinCount; j++)
                                {
                                    string pin = data.Pinyins[j];
                                    if (!string.IsNullOrEmpty(pin))
                                    {
                                        if (HeadOrAll)
                                        {
                                            result += tmpArr[i] + pin[0] + ",";
                                        }
                                        else
                                        {
                                            result += tmpArr[i] + pin.Substring(0, pin.Length - 1) + ",";
                                        }
                                    }
                                }
                            }
                            tmpArr = result.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            string finallyresult = string.Empty;
                            List<string> tempList = new List<string>();
                            foreach (string temp in tmpArr)
                            {
                                if (tempList.Contains(temp) == false)
                                {
                                    tempList.Add(temp);
                                    finallyresult += temp + ",";
                                }
                            }
                            if (finallyresult.EndsWith(","))
                            {
                                result = finallyresult.Substring(0, finallyresult.Length - 1);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < data.PinyinCount; i++)
                            {
                                string pin = data.Pinyins[i];
                                if (!string.IsNullOrEmpty(pin))
                                {
                                    if (HeadOrAll)
                                    {
                                        if (!result.Contains(pin[0]))
                                        {
                                            result += pin[0] + ",";
                                        }
                                    }
                                    else
                                    {
                                        if (!result.Contains(pin.Substring(0, pin.Length - 1)))
                                        {
                                            result += pin.Substring(0, pin.Length - 1) + ",";
                                        }
                                    }
                                }
                            }
                            if (result.EndsWith(","))
                            {
                                result = result.Substring(0, result.Length - 1);
                            }
                        }
                    }
                }
                else
                {
                    result += item;
                }
            }
            return result;
        }
    }
}
