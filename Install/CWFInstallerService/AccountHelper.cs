using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace CWFInstallerService
{
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

        [DllImport("Kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal static extern bool CloseHandle(IntPtr handle);
    }

    public class ImpersonationContext : IDisposable
    {
        public delegate void Run();

        private readonly string domain;
        private readonly string userName;
        private readonly string password;
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

        public WindowsIdentity Identity { get { return identity; } }

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
            var id = identity;
            var current = WindowsIdentity.GetCurrent(false);
            if (id == null)
                id = current;

            if (id.User.Equals(current.User))
            {
                runnable.Invoke();
                return;
            }

            using (var context = id.Impersonate())
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

    class AccountHelper
    {
        public static void IsUserLocalAdministrator(string domain, string user, string password)
        {
            using (var ctx = new ImpersonationContext(domain, user, password))
            {
                // Validate password.
                try
                {
                    ctx.LogonUser();
                }
                catch (Exception ex)
                {
                    throw new InvalidCredentialsException();
                }

                // Check if user is local admin.
                bool isAdmin = false;
                try
                {
                    ctx.RunAs(() =>
                    {
                        WindowsPrincipal wp = new WindowsPrincipal(ctx.Identity);
                        isAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);
                    });
                    if (!isAdmin) throw new InsufficientPermissionsException();
                }
                catch (Exception ex)
                {
                    throw new FailedToVerifyCredentialsException();
                }
            }
        }
    }
}
