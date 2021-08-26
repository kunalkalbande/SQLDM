using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;

namespace Idera.SQLdm.ManagementService.WebClient
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
}
