using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.ComponentModel;

namespace Idera.SQLdoctor.Common.Helpers
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class SERVICE_STATUS_PROCESS
    {
        public int serviceType;
        public int currentState;
        public int controlsAccepted;
        public int win32ExitCode;
        public int serviceSpecificExitCode;
        public int checkPoint;
        public int waitHint;
        public int processID;
        public int serviceFlags;
    }

    class ServiceHelpers
    {
        #region GetServiceProcessId

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool QueryServiceStatusEx(IntPtr serviceHandle, int infoLevel, IntPtr buffer, int bufferSize, out int bytesNeeded);

        public static int GetServiceProcessId(string serviceName)
        {
            SERVICE_STATUS_PROCESS ssp = new SERVICE_STATUS_PROCESS();
            using (ServiceController sc = new ServiceController(serviceName))
            {
                int bufferSize = Marshal.SizeOf(ssp);
                int bytesNeeded = bufferSize;
                IntPtr hService = sc.ServiceHandle.DangerousGetHandle();
                IntPtr hBuffer = Marshal.AllocHGlobal(bufferSize);
                try
                {

                    bool rc = QueryServiceStatusEx(hService, 0, hBuffer, bufferSize, out bytesNeeded);
                    if (rc)
                        Marshal.PtrToStructure(hBuffer, ssp);
                }
                finally
                {
                    if (hBuffer != IntPtr.Zero)
                        Marshal.FreeHGlobal(hBuffer);
                }
            }

            return ssp.processID;
        }
        #endregion

        #region GetParentProcessId

        [Flags]
        private enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PROCESSENTRY32
        {
            const int MAX_PATH = 260;
            internal UInt32 dwSize;
            internal UInt32 cntUsage;
            internal UInt32 th32ProcessID;
            internal IntPtr th32DefaultHeapID;
            internal UInt32 th32ModuleID;
            internal UInt32 cntThreads;
            internal UInt32 th32ParentProcessID;
            internal Int32 pcPriClassBase;
            internal UInt32 dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string szExeFile;
        }

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr CreateToolhelp32Snapshot([In]UInt32 dwFlags, [In]UInt32 th32ProcessID);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32First([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32Next([In]IntPtr hSnapshot, ref PROCESSENTRY32 lppe);


        /// <summary>
        /// Scans the process list for the given process id and returns the id of the process that started it.
        /// If processId is 0 the the parent of the current process is returned.
        /// </summary>
        /// <param name="processId">The process id to look for or 0 for the current process.  </param>
        /// <returns>The parent process id or -1 if not found</returns>
        public static int GetParentProcessId(int processId, ref Dictionary<int,string> processMap)
        {
            int result = -1;

            if (processId == 0)
                processId = Process.GetCurrentProcess().Id;

            if (processMap != null && processMap.Count > 0)
                processMap.Clear();

            PROCESSENTRY32 procEntry = new PROCESSENTRY32();
            procEntry.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));

            IntPtr plist = CreateToolhelp32Snapshot((uint)SnapshotFlags.Process, 0);
            using (SafeFileHandle hlist = new SafeFileHandle(plist, true))
            {
                if (!hlist.IsInvalid)
                {
                    bool c = Process32First(plist, ref procEntry);
                    if (!c)
                    {
                        int err = Marshal.GetLastWin32Error();
                        throw new Win32Exception(err);
                    }
                    do
                    {
                        int pid = (int)procEntry.th32ProcessID;
                        if (processMap != null)
                        {
                            if (processMap.ContainsKey(pid))
                                processMap.Remove(pid);
                            processMap.Add(pid, procEntry.szExeFile);
                        }

                        if (procEntry.th32ProcessID == processId)
                        {
                            result = (int)procEntry.th32ParentProcessID;
                            if (processMap == null)
                                break;
                        }

                        c = Process32Next(plist, ref procEntry);
                        if (!c)
                        {
                            int err = Marshal.GetLastWin32Error();
                            if (err == 18)
                                break;
                            throw new Win32Exception(err);
                        }
                    } while (c); 
                }
            }

            return result;
        }

        #endregion

        #region ValidateUser

        internal static int ValidateUser(string userId, string password, bool logonAsBatch)
        {
            if (String.IsNullOrEmpty(password))
                throw new ArgumentException("Password is required", "password");

            int hresult = 0;
            StringBuilder uid = new StringBuilder(256);
            StringBuilder domain = new StringBuilder(256);

            hresult = CredUIParseUserName(userId, uid, 256, domain, 256);
            if (hresult != 0)
            {
                Win32Exception hrException = new Win32Exception(hresult);
                throw new ArgumentException("User id not in expected format", hrException);
            }
            
            IntPtr phToken = IntPtr.Zero;
            int logonType = (int)(logonAsBatch ? LogonType.LOGON32_LOGON_BATCH : LogonType.LOGON32_LOGON_NETWORK);
            bool credsValid = LogonUser(uid.ToString(), domain.ToString(), password, logonType, 0, out phToken);
            if (credsValid)
            {
                SafeFileHandle safeHandle = new SafeFileHandle(phToken, true);
                safeHandle.Dispose();
            }
            else
                hresult = Marshal.GetLastWin32Error();

            return hresult;
        }

        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        private static extern int CredUIParseUserName(string domainUser, StringBuilder user, int cchUser, StringBuilder domain, int cchDomain);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken
            );

        public enum LogonType : int
        {
            LOGON32_LOGON_INTERACTIVE = 2,
            LOGON32_LOGON_NETWORK = 3,
            LOGON32_LOGON_BATCH = 4,
            LOGON32_LOGON_SERVICE = 5,
            LOGON32_LOGON_UNLOCK = 7,
            LOGON32_LOGON_NETWORK_CLEARTEXT = 8,
            LOGON32_LOGON_NEW_CREDENTIALS = 9,
        }

        public enum LogonProvider : int
        {
            LOGON32_PROVIDER_DEFAULT = 0,
        }

        #endregion

    }
}
