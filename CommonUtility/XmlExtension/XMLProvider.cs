using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utility.CommonUtility.Xml
{
    /// <summary>
    /// Provider for xml
    /// use to load an parse xml
    /// </summary>
    public class XMLProvider
    {
        #region Load xml

        public static Dictionary<string, Dictionary<string, string>> XMLDictionary =
            new Dictionary<string, Dictionary<string, string>>();

        private static string FIRSTNODE = "Nodes";         //root note of xml

        /// <summary>
        /// Load the XML data into the cache
        /// </summary>
        /// <param name="xmls">The XML as string</param>
        public static void LoadXml(string[] xmls)
        {
            for (int i = 0; i < xmls.Length; i++)
            {
                XmlDocument xmlRoot = new XmlDocument();
                xmlRoot.LoadXml(xmls[i]);
                AddToXMLDic(xmlRoot);
            }
        }

        /// <summary>
        /// Load the XML data into the cache 
        /// </summary>
        /// <param name="xmlNames">The XML file name</param>
        public static void Load(string[] xmlNames)
        {
            for (int i = 0; i < xmlNames.Length; i++)
            {
                XmlDocument xmlRoot = new XmlDocument();
                xmlRoot.Load(xmlNames[i]);
                AddToXMLDic(xmlRoot);
            }
        }

        /// <summary>
        /// Reload by xml string
        /// </summary>
        /// <param name="xmls">The XML  as string</param>
        public static void ReloadXml(string[] xmls)
        {
            XMLDictionary.Clear();
            LoadXml(xmls);
        }

        /// <summary>
        /// Reload by xml filename
        /// </summary>
        /// <param name="xmlName">The XML file name</param>
        public static void Reload(string[] xmlName)
        {
            XMLDictionary.Clear();
            Load(xmlName);
        }

        //----------------- private methods

        /// <summary>
        /// add xml to dictionary
        /// </summary>
        /// <param name="xmlRoot"></param>
        private static void AddToXMLDic(XmlDocument xmlRoot, string firstNode = "Nodes")
        {
            FIRSTNODE = firstNode;
            XmlNode firstXmlNode = xmlRoot.SelectSingleNode(FIRSTNODE);
            XmlElement xeFirstNode = (XmlElement)firstXmlNode;
            string moduleName = xeFirstNode.GetAttribute("id");
            XmlNodeList nodeList = firstXmlNode.ChildNodes;

            Dictionary<string, string> idList = new Dictionary<string, string>();

            foreach (XmlNode xn in nodeList)
            {
                XmlElement xe = (XmlElement)xn;

                idList.Add(xe.GetAttribute("id"), xe.InnerText);
            }
            XMLDictionary.Add(moduleName, idList);
        }

        #endregion //load xml

        #region Parse Xml

        private static readonly char REPLACE_START_FLAG = '<';
        private static readonly char REPLACE_END_FLAG = '>';

        private static object _param;
        private static Hashtable _mapParams;
        private static string _inputStr;
        private static StringBuilder _outputStr;
        private static int _currentIndex = -1;
        private static int _lastIndex;
        private static char _currentChar;

        public static string ParseXML(string input, object param)
        {
            _outputStr = new StringBuilder();
            _inputStr = input;

            _currentIndex = -1;
            _lastIndex = input.Length;

            _mapParams = param as Hashtable;
            if (_mapParams == null)
            {
                _param = param;
            }

            while (NextChar())
            {
                if (_currentChar == REPLACE_START_FLAG && input[_currentIndex + 1] == REPLACE_START_FLAG)
                {
                    // "<<"  end of replace is ">>"
                    ReplaceParameter();
                }
                else
                {
                    _outputStr.Append(_currentChar);
                }
            }

            return _outputStr.ToString();
        }

        //---------------- private methods

        /// <summary>
        /// get the next char from input string
        /// </summary>
        /// <returns></returns>
        private static bool NextChar()
        {
            _currentIndex++;

            if (_currentIndex < _lastIndex)
            {
                _currentChar = _inputStr[_currentIndex];
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// replace the Parameter
        /// </summary>
        private static void ReplaceParameter()
        {

            int start = _currentIndex + 2;

            while (NextChar())
            {
                if (_currentChar == REPLACE_END_FLAG)
                {
                    string property = _inputStr.Substring(start, _currentIndex - start);

                    object value = null;

                    try
                    {
                        value = _mapParams == null ? GetPropertyValue(_param, property)
                                : _mapParams[property];
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }

                    _outputStr.Append(value);

                    _currentIndex++;    // next char is '>', so skip it !

                    return;
                }
            }
        }

        /// <summary>
        /// get the property value of object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }

        #endregion  //parse xml
    }
}
