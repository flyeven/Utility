using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Utility.CommonUtility.SystemOption
{
    /// <summary>
    /// 光标控制 工具类
    /// </summary>
    public class CursorControl
    {
        //##---------Public:

        #region 修改默认光标(从文件加载光标文件)
        /// <summary>
        /// 修改默认光标(从文件加载光标文件)
        /// <para />
        /// 影响效果为系统范围
        /// </summary>
        /// <returns>成功与否</returns>
        public static bool SetCursor_System(string m_CursorFilePath)
        {
            IntPtr result = LoadCursorFromFilePath(m_CursorFilePath);
            if (result == IntPtr.Zero)
                return false;

            SetSystemCursor(result, OCR_NORMAL);
            SetSystemCursor(result, OCR_IBEAM);
            return true;
        }

        #endregion

        #region 修改默认光标(从Cursors选择)
        /// <summary>
        /// 修改默认光标(从Cursors选择)
        /// <para />
        /// 影响效果为系统范围
        /// </summary>
        /// <returns>成功与否</returns>
        public static bool SetCursor_System(Cursor m_Cursor)
        {
            IntPtr result = m_Cursor.CopyHandle();
            if (result == IntPtr.Zero)
                return false;

            SetSystemCursor(result, OCR_NORMAL);
            SetSystemCursor(result, OCR_IBEAM);
            return true;
        }

        #endregion

        #region 还原默认光标
        /// <summary>
        /// 还原默认光标
        /// <para />
        /// 影响效果为系统范围
        /// </summary>
        public static void RestoreCursor_System()
        {
            SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, SPIF_SENDWININICHANGE);
        }
        #endregion


        //##---------Private:

        #region API定义

        const int OCR_WAIT = 32514;
        const int OCR_NORMAL = 32512;
        const int OCR_APPSTARTING = 32650;
        const int OCR_IBEAM = 32513;
        const int SPI_SETCURSORS = 87;
        const int SPIF_SENDWININICHANGE = 2;


        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursorFromFile(string fileName);

        [DllImport("user32.dll")]
        private static extern int SetSystemCursor(IntPtr hcur, int id);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursor(int hInstance, int lpCunrsorName);

        /// <summary>
        /// 修改参数结构体信息
        /// </summary>
        /// <param name="uiAction"></param>
        /// <param name="uiParam"></param>
        /// <param name="pvParam"></param>
        /// <param name="fWinIni"></param>
        /// <returns></returns>
        [DllImport("User32.DLL")]
        public static extern bool SystemParametersInfo(int uiAction, int uiParam, IntPtr pvParam, int fWinIni);

        #endregion

        #region 加载光标文件并返回指针
        /// <summary>
        /// 加载光标文件并返回指针
        /// </summary>
        /// <param name="m_CursorFilePath">光标文件路径</param>
        /// <returns>返回指针</returns>
        private static IntPtr LoadCursorFromFilePath(string m_CursorFilePath)
        {
            //增强判断
            if (File.Exists(m_CursorFilePath) == false)
                return IntPtr.Zero;

            IntPtr result = LoadCursorFromFile(m_CursorFilePath);
            return result;
        }
        #endregion

    }
}
