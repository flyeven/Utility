using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CommonUtility.SystemOption
{
    /// <summary>
    /// 消息框控制器
    /// </summary>
    public static class MessageBoxControl
    {

        //##---------Public:

        #region 关闭指定窗体内，指定标题的MessageBox
        /// <summary>
        /// 关闭指定窗体内，指定标题的MessageBox
        /// </summary>
        /// <param name="ParentWindow"></param>
        /// <param name="MessageBoxTitleName"></param>
        public static void CloseMessageBox(IntPtr ParentWindow, string MessageBoxTitleName)
        {
            //获取目标对话框句柄
            //IntPtr targetHwnd = FindWindowEx(ParentWindow, IntPtr.Zero, "#32770", MessageBoxTitleName);
            IntPtr targetHwnd = FindWindow("#32770", MessageBoxTitleName);

            //判断
            if (targetHwnd == IntPtr.Zero)
                return;

            //关闭
            IntPtr result;
            EndDialog(targetHwnd, out result);
        }

        #endregion


        //##---------Private:

        #region API定义

        private const int WM_CLOSE = 0x10;

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool EndDialog(IntPtr hDlg, out IntPtr nResult);

        #endregion


    }
}
