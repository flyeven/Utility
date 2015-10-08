using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CommonUtility.SystemOption
{
    /// <summary>
    /// 鼠标控制
    /// </summary>
    public class MouseControl
    {
        #region API定义

        [DllImport("user32.dll")]
        private static extern int ReleaseCapture();

        #endregion

        #region 释放鼠标
        /// <summary>
        /// 释放鼠标
        /// </summary>
        public static void ReleaseMouse()
        {
            ReleaseCapture();
        }
        #endregion
    }
}
