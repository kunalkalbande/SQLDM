using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Idera.SQLdm.ManagementService.UI
{
    class NativeMethods
    {
        internal delegate bool EnumWinCallBack(int hwnd, int lParam);
        internal const int SW_RESTORE = 9;
        internal const int SW_HIDE = 5;
        internal const int SW_SHOW = 5;

        [DllImport("user32.Dll")]
        internal static extern int EnumWindows(EnumWinCallBack callBackFunc, int lParam);

        [DllImport("User32.Dll")]
        internal static extern void GetWindowText(int hWnd, StringBuilder str, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32", EntryPoint = "FindWindow")]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    }
}
