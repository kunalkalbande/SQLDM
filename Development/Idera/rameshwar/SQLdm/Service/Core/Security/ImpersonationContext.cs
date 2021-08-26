using System;
using System.Runtime.ConstrainedExecution;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Idera.SQLdm.Service.Core.Security
{
    public class ImpersonationContext : IDisposable
    {
        public delegate void Run();

        private string domain;
        private string userName;
        private string password;
        private WindowsIdentity identity;

        public ImpersonationContext(string domain, string userID, string password)
        {
            this.domain = domain;
            // 
            if (String.IsNullOrEmpty(domain) && !userID.Contains("@"))
            {
                // default to domain of current user (only good for remote machine if remote machine trusts domain)
                domain = Environment.UserDomainName;
                if (String.IsNullOrEmpty(domain))
                    // default to local machine name (no good for access to a remote machine)
                    domain = Environment.MachineName;
            }

            this.userName = userID;
            this.password = password;
        }

        public bool IsLoggedOn { get { return identity != null; } }

        public void LogonUser()
        {
            if (identity != null)
            {
                identity.Dispose();
                identity = null;
            }

            if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
                return;

            IntPtr userPtr = IntPtr.Zero;
            IntPtr idPtr = IntPtr.Zero;

            try
            {
                bool rc = Native.LogonUser(
                    userName,
                    domain,
                    password,
                    Native.LOGON32_LOGON_NETWORK_CLEARTEXT,
                    Native.LOGON32_PROVIDER_WINNT50,
                    out userPtr);
                if (false == rc)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastError);
                }

                // duplicate token with impersonation enabled
                rc = Native.DuplicateToken(userPtr, 2, out idPtr);
                if (false == rc)
                {
                    int lastError = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastError);
                }

                identity = new WindowsIdentity(idPtr);
            }
            finally
            {
                if (userPtr != IntPtr.Zero)
                    Native.CloseHandle(userPtr);
                if (idPtr != IntPtr.Zero)
                    Native.CloseHandle(idPtr);
            }
        }

        public void RunAs(Run runnable)
        {
            WindowsIdentity id = identity;
            WindowsIdentity current = WindowsIdentity.GetCurrent(false);
            if (id == null)
                id = current;

            if (id.User.Equals(current.User))
            {
                runnable.Invoke();
                return;
            }

            using (WindowsImpersonationContext context = id.Impersonate())
            {
                try
                {
                    runnable.Invoke();
                }
                catch
                {
                    context.Undo();
                    throw;
                }
                finally
                {
                    context.Undo();
                }
            }
        }

        public void Dispose()
        {
            if (identity != null)
            {
                identity.Dispose();
                identity = null;
            }
        }
    }

    #region Native Methods

    internal class Native
    {
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_LOGON_NETWORK = 3;
        public const int LOGON32_LOGON_BATCH = 4;
        public const int LOGON32_LOGON_SERVICE = 5;
        public const int LOGON32_LOGON_UNLOCK = 7;
        public const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
        public const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        public const int LOGON32_PROVIDER_DEFAULT = 0;
        public const int LOGON32_PROVIDER_WINNT50 = 3;
        public const int LOGON32_PROVIDER_WINNT40 = 2;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool LogonUser(String lpszUserName, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal extern static bool DuplicateToken(IntPtr ExistingTokenHandle, int SECURITY_IMPERSONATION_LEVEL, out IntPtr DuplicateTokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool RevertToSelf();

        [DllImport("advapi32.DLL", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ImpersonateLoggedOnUser(IntPtr hToken);

        public const uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public const uint STANDARD_RIGHTS_READ = 0x00020000;
        public const uint TOKEN_ASSIGN_PRIMARY = 0x0001;
        public const uint TOKEN_DUPLICATE = 0x0002;
        public const uint TOKEN_IMPERSONATE = 0x0004;
        public const uint TOKEN_QUERY = 0x0008;
        public const uint TOKEN_QUERY_SOURCE = 0x0010;
        public const uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        public const uint TOKEN_ADJUST_GROUPS = 0x0040;
        public const uint TOKEN_ADJUST_DEFAULT = 0x0080;
        public const uint TOKEN_ADJUST_SESSIONID = 0x0100;
        public const uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
        public const uint TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
            TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
            TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
            TOKEN_ADJUST_SESSIONID);

        [DllImport("Kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal static extern bool CloseHandle(IntPtr handle);

        [DllImport("Advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal static extern bool AdjustTokenPrivileges(
            [In]     IntPtr TokenHandle,
            [In]     bool DisableAllPrivileges,
            [In]     ref TOKEN_PRIVILEGE NewState,
            [In]     uint BufferLength,
            [In, Out] ref TOKEN_PRIVILEGE PreviousState,
            [In, Out] ref uint ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, ref Int64 lpLuid);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct LUID
        {
            internal long luid;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct LUID_AND_ATTRIBUTES
        {
            internal LUID Luid;
            internal uint Attributes;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct TOKEN_PRIVILEGE
        {
            internal uint PrivilegeCount;
            internal LUID_AND_ATTRIBUTES Privilege;
        }

        public const long SE_PRIVILEGE_ENABLED_BY_DEFAULT = (0x00000001L);
        public const long SE_PRIVILEGE_ENABLED = (0x00000002L);
        public const long SE_PRIVILEGE_REMOVED = (0X00000004L);
        public const long SE_PRIVILEGE_USED_FOR_ACCESS = (0x80000000L);

        public const string SE_CREATE_TOKEN_NAME = "SeCreateTokenPrivilege";
        public const string SE_ASSIGNPRIMARYTOKEN_NAME = "SeAssignPrimaryTokenPrivilege";
        public const string SE_LOCK_MEMORY_NAME = "SeLockMemoryPrivilege";
        public const string SE_INCREASE_QUOTA_NAME = "SeIncreaseQuotaPrivilege";
        public const string SE_UNSOLICITED_INPUT_NAME = "SeUnsolicitedInputPrivilege";
        public const string SE_MACHINE_ACCOUNT_NAME = "SeMachineAccountPrivilege";
        public const string SE_TCB_NAME = "SeTcbPrivilege";
        public const string SE_SECURITY_NAME = "SeSecurityPrivilege";
        public const string SE_TAKE_OWNERSHIP_NAME = "SeTakeOwnershipPrivilege";
        public const string SE_LOAD_DRIVER_NAME = "SeLoadDriverPrivilege";
        public const string SE_SYSTEM_PROFILE_NAME = "SeSystemProfilePrivilege";
        public const string SE_SYSTEMTIME_NAME = "SeSystemtimePrivilege";
        public const string SE_PROF_SINGLE_PROCESS_NAME = "SeProfileSingleProcessPrivilege";
        public const string SE_INC_BASE_PRIORITY_NAME = "SeIncreaseBasePriorityPrivilege";
        public const string SE_CREATE_PAGEFILE_NAME = "SeCreatePagefilePrivilege";
        public const string SE_CREATE_PERMANENT_NAME = "SeCreatePermanentPrivilege";
        public const string SE_BACKUP_NAME = "SeBackupPrivilege";
        public const string SE_RESTORE_NAME = "SeRestorePrivilege";
        public const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        public const string SE_DEBUG_NAME = "SeDebugPrivilege";
        public const string SE_AUDIT_NAME = "SeAuditPrivilege";
        public const string SE_SYSTEM_ENVIRONMENT_NAME = "SeSystemEnvironmentPrivilege";
        public const string SE_CHANGE_NOTIFY_NAME = "SeChangeNotifyPrivilege";
        public const string SE_REMOTE_SHUTDOWN_NAME = "SeRemoteShutdownPrivilege";
        public const string SE_UNDOCK_NAME = "SeUndockPrivilege";
        public const string SE_SYNC_AGENT_NAME = "SeSyncAgentPrivilege";
        public const string SE_ENABLE_DELEGATION_NAME = "SeEnableDelegationPrivilege";
        public const string SE_MANAGE_VOLUME_NAME = "SeManageVolumePrivilege";
        public const string SE_IMPERSONATE_NAME = "SeImpersonatePrivilege";
        public const string SE_CREATE_GLOBAL_NAME = "SeCreateGlobalPrivilege";
    }

    #endregion

}
