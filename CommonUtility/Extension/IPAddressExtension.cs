using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utility.CommonUtility.Extension
{
    /// <summary>
    /// IP地址拓展
    /// </summary>
    public class IPAddressExtension
    {
        /// <summary>
        /// Get localhost IP address(IPv4)
        /// </summary>
        public static string GetIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] ips = Dns.GetHostEntry(hostName).AddressList;
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    continue;
                return ip.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 检查是否为有效的IP地址
        /// </summary>
        /// <param name="baseString"></param>
        /// <returns></returns>
        public static bool IsIPAddress(string baseString)
        {
            IPAddress target;
            return IPAddress.TryParse(baseString, out target);
        }

        /// <summary>
        /// 检查是否为有效的IPv4地址
        /// </summary>
        /// <param name="baseString"></param>
        /// <returns></returns>
        public static bool IsIPv4Address(string baseString)
        {
            IPAddress target;
            if (IPAddress.TryParse(baseString, out target))
            {
                if (target.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    //增强判断
                    Regex reg = new Regex(@"\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}", RegexOptions.IgnoreCase);
                    return reg.IsMatch(baseString);
                }
                else
                    return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否为有效的IPv6地址
        /// </summary>
        /// <param name="baseString"></param>
        /// <returns></returns>
        public static bool IsIPv6Address(string baseString)
        {
            IPAddress target;
            if (IPAddress.TryParse(baseString, out target))
            {
                return target.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
            }
            else
            {
                return false;
            }
        }
    }
}
