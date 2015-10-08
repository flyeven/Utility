using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CommonUtility.SystemInfo
{
    #region 内存的信息结构
    /// <summary>
    /// 内存情况 结果数据类
    /// </summary>
    [Serializable()]
    public class OsMemoryInfo
    {
        /// <summary>
        /// 长度
        /// </summary>
        public ulong dwLength;
        /// <summary>
        /// 内存正在使用率(%)
        /// </summary>
        public ulong dwMemoryLoad;
        /// <summary>
        /// 物理内存共(字节)
        /// </summary>
        public ulong dwTotalPhys;
        /// <summary>
        /// 可使用的物理内存(字节)
        /// </summary>
        public ulong dwAvailPhys;
        /// <summary>
        /// 交换文件总大小(字节)
        /// </summary>
        public ulong dwTotalPageFile;
        /// <summary>
        /// 尚可交换文件大小(字节)
        /// </summary>
        public ulong dwAvailPageFile;
        /// <summary>
        /// 总虚拟内存(字节)
        /// </summary>
        public ulong dwTotalVirtual;
        /// <summary>
        /// 未用虚拟内存(字节)
        /// </summary>
        public ulong dwAvailVirtual;
    }
    #endregion

    /// <summary>
    /// 内存信息获取
    /// </summary>
    public class MemoryInfo
    {
        /// <summary>
        /// 实例
        /// </summary>
        public readonly static MemoryInfo Instance = new MemoryInfo();

        #region 数据结构
        /// <summary>
        /// 定义内存的信息结构 32位
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_INFO_32bit
        {
            public UInt32 dwLength;
            /// <summary>
            /// 内存正在使用率(%)
            /// </summary>
            public UInt32 dwMemoryLoad;
            /// <summary>
            /// 物理内存共(字节)
            /// </summary>
            public UInt32 dwTotalPhys;
            /// <summary>
            /// 可使用的物理内存(字节)
            /// </summary>
            public UInt32 dwAvailPhys;
            /// <summary>
            /// 交换文件总大小(字节)
            /// </summary>
            public UInt32 dwTotalPageFile;
            /// <summary>
            /// 尚可交换文件大小(字节)
            /// </summary>
            public UInt32 dwAvailPageFile;
            /// <summary>
            /// 总虚拟内存(字节)
            /// </summary>
            public UInt32 dwTotalVirtual;
            /// <summary>
            /// 未用虚拟内存(字节)
            /// </summary>
            public UInt32 dwAvailVirtual;
        }

        /// <summary>
        /// 定义内存的信息结构 64位
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_INFO_64bit
        {
            public UInt64 dwLength;
            /// <summary>
            /// 内存正在使用率(%)
            /// </summary>
            public UInt64 dwMemoryLoad;
            /// <summary>
            /// 物理内存共(字节)
            /// </summary>
            public UInt64 dwTotalPhys;
            /// <summary>
            /// 可使用的物理内存(字节)
            /// </summary>
            public UInt64 dwAvailPhys;
            /// <summary>
            /// 交换文件总大小(字节)
            /// </summary>
            public UInt64 dwTotalPageFile;
            /// <summary>
            /// 尚可交换文件大小(字节)
            /// </summary>
            public UInt64 dwAvailPageFile;
            /// <summary>
            /// 总虚拟内存(字节)
            /// </summary>
            public UInt64 dwTotalVirtual;
            /// <summary>
            /// 未用虚拟内存(字节)
            /// </summary>
            public UInt64 dwAvailVirtual;
        }

        #endregion

        //32Bit
        [DllImport("kernel32")]
        private static extern void GlobalMemoryStatus(ref MEMORY_INFO_32bit meminfo);

        //64Bit
        [DllImport("kernel32")]
        private static extern void GlobalMemoryStatus(ref MEMORY_INFO_64bit meminfo);

        /// <summary>
        /// 获取内存信息
        /// </summary>
        /// <returns></returns>
        public OsMemoryInfo GetInfo()
        {
            OsMemoryInfo result = new OsMemoryInfo();

            if (Environment.Is64BitProcess)
            {
                //调用GlobalMemoryStatus函数获取内存的相关信息
                MEMORY_INFO_64bit MemInfo_64bit = new MEMORY_INFO_64bit();
                GlobalMemoryStatus(ref MemInfo_64bit);

                //赋值结果
                result.dwAvailPageFile = Convert.ToUInt64(MemInfo_64bit.dwAvailPageFile);
                result.dwAvailPhys = Convert.ToUInt64(MemInfo_64bit.dwAvailPhys);
                result.dwAvailVirtual = Convert.ToUInt64(MemInfo_64bit.dwAvailVirtual);
                result.dwLength = Convert.ToUInt64(MemInfo_64bit.dwLength);
                result.dwMemoryLoad = Convert.ToUInt64(MemInfo_64bit.dwMemoryLoad);
                result.dwTotalPageFile = Convert.ToUInt64(MemInfo_64bit.dwTotalPageFile);
                result.dwTotalPhys = Convert.ToUInt64(MemInfo_64bit.dwTotalPhys);
                result.dwTotalVirtual = Convert.ToUInt64(MemInfo_64bit.dwTotalVirtual);
            }
            else
            {
                //调用GlobalMemoryStatus函数获取内存的相关信息 
                MEMORY_INFO_32bit MemInfo_32bit = new MEMORY_INFO_32bit();
                GlobalMemoryStatus(ref MemInfo_32bit);

                //赋值结果
                result.dwAvailPageFile = Convert.ToUInt32(MemInfo_32bit.dwAvailPageFile);
                result.dwAvailPhys = Convert.ToUInt32(MemInfo_32bit.dwAvailPhys);
                result.dwAvailVirtual = Convert.ToUInt32(MemInfo_32bit.dwAvailVirtual);
                result.dwLength = Convert.ToUInt32(MemInfo_32bit.dwLength);
                result.dwMemoryLoad = Convert.ToUInt32(MemInfo_32bit.dwMemoryLoad);
                result.dwTotalPageFile = Convert.ToUInt32(MemInfo_32bit.dwTotalPageFile);
                result.dwTotalPhys = Convert.ToUInt32(MemInfo_32bit.dwTotalPhys);
                result.dwTotalVirtual = Convert.ToUInt32(MemInfo_32bit.dwTotalVirtual);
            }

            return result;
        }
    }
}
