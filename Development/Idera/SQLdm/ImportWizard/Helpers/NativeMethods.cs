using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.ImportWizard.Helpers
{
    static class NativeMethods
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

        [DllImport("DmLicUtil.dll")]
        public static extern string DecryptString(byte[] encryptedString, string key, int length, out int errorCode);

        [DllImport("version.dll")]
        public static extern bool GetFileVersionInfo(string sFileName,
              int handle, int size, byte[] infoBuffer);

        [DllImport("version.dll")]
        public static extern int GetFileVersionInfoSize(string sFileName,
              out int handle);

        // The third parameter - "out string pValue" - is automatically
        // marshaled from ANSI to Unicode:
        [DllImport("version.dll")]
        unsafe public static extern bool VerQueryValue(byte[] pBlock,
              string pSubBlock, out string pValue, out uint len);

        // This VerQueryValue overload is marked with 'unsafe' because 
        // it uses a short*:
        [DllImport("version.dll")]
        unsafe public static extern bool VerQueryValue(byte[] pBlock,
              string pSubBlock, out short* pValue, out uint len);
    }
}
