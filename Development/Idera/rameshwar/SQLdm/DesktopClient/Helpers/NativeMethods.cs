using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    static class NativeMethods
    {
        internal delegate bool EnumWinCallBack(int hwnd, int lParam);
        internal const int SW_RESTORE = 9;
        internal const int SW_HIDE = 5;
        internal const int SW_SHOW = 5;

        #region COM Interfaces

        [ComImport, GuidAttribute("79EAC9EE-BAF9-11CE-8C82-00AA004BA90B")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInternetSecurityManager
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetSecuritySite([In] IntPtr pSite);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetSecuritySite([Out] IntPtr pSite);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int MapUrlToZone([In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl,
                     out UInt32 pdwZone, UInt32 dwFlags);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetSecurityId([MarshalAs(UnmanagedType.LPWStr)] string pwszUrl,
                      [MarshalAs(UnmanagedType.LPArray)] byte[] pbSecurityId,
                      ref UInt32 pcbSecurityId, uint dwReserved);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ProcessUrlAction([In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl,
                     UInt32 dwAction, out byte pPolicy, UInt32 cbPolicy,
                     byte pContext, UInt32 cbContext, UInt32 dwFlags,
                     UInt32 dwReserved);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int QueryCustomPolicy([In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl,
                      ref Guid guidKey, ref byte ppPolicy, ref UInt32 pcbPolicy,
                      ref byte pContext, UInt32 cbContext, UInt32 dwReserved);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int SetZoneMapping(UInt32 dwZone,
                       [In, MarshalAs(UnmanagedType.LPWStr)] string lpszPattern,
                       UInt32 dwFlags);

            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetZoneMappings(UInt32 dwZone, out IEnumString ppenumString,
                    UInt32 dwFlags);
        }

        [ComImport, GuidAttribute("6D5140C1-7436-11CE-8034-00AA006009FA")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IServiceProvider
        {
            [PreserveSig]
            [return: MarshalAs(UnmanagedType.I4)]
            int QueryService(ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
        }

        private static Guid _CLSID_SecurityManager = new Guid("7b8a2d94-0ac9-11d1-896c-00c04fb6bfc4");

        #endregion

        internal enum URLZONE : uint
        {
            URLZONE_PREDEFINED_MIN = 0,
            URLZONE_LOCAL_MACHINE  = 0,         // local machine zone is not exposed in UI
            URLZONE_INTRANET,                   // My Intranet zone
            URLZONE_TRUSTED,                    // Trusted Web sites zone
            URLZONE_INTERNET,                   // The Internet zone
            URLZONE_UNTRUSTED,                  // Untrusted sites zone
            URLZONE_PREDEFINED_MAX = 999,

            URLZONE_USER_MIN = 1000,
            URLZONE_USER_MAX = 10000,
        }

        internal const uint SZM_CREATE = 0;
        internal const uint SZM_DELETE = 1;
        
        internal static int SetZoneMapping(URLZONE zone, String pattern, uint flags)
        {
            int result = -1;
            Type t = Type.GetTypeFromCLSID(_CLSID_SecurityManager);
            object securityManager = Activator.CreateInstance(t);
            try
            {
                IInternetSecurityManager ISM = securityManager as IInternetSecurityManager;
                if (ISM != null)
                    result = ISM.SetZoneMapping((uint)zone, pattern, flags);
            }
            finally
            {
                if (securityManager != null)
                    Marshal.ReleaseComObject(securityManager);
            }
            return result;
        }

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
