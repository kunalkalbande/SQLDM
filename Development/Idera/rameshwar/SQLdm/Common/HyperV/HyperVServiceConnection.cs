using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Net;

namespace Idera.SQLdm.Common.HyperV
{
    class HyperVServiceConnection
    {
        private string hUserName;
        private string hyperVAddress;
        private string hyperVPassword;
        protected ConnectState state;
        ManagementObject virtualSystemService = null;

        public string Password
        {
            get { return hyperVPassword; }
            set { hyperVPassword = value; }
        }
        

        public string Address
        {
            get { return hyperVAddress; }
            set { hyperVAddress = value; }
        }


        public string UserName
        {
            get { return hUserName; }
            set { hUserName = value; }
        }

        public ConnectState State
        {
            get { return state; }
        }

        public HyperVServiceConnection(string url)
        {
            state = ConnectState.Disconnected;
            Address = url;
        }

        public ManagementObject VirtualSystemService
        {
            get { return virtualSystemService; }
        }

        public static bool IsLocalHost(string host)
        {
            try
            {
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                foreach (IPAddress hostIP in hostIPs)
                {
                    if (IPAddress.IsLoopback(hostIP)) return true;
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            return false;
        }

        public void Connect(string username, string password)
        {
            bool WMIFailed = false;

            UserName = username;
            Password = password;
            ConnectionOptions connOpts = new ConnectionOptions();
            connOpts.Username = UserName;
            connOpts.Password = Password;

            try
            {
                string connectionUrl = string.Format(@"\\{0}\root\virtualization\V2", Address.ToString());
                ManagementScope scope;
                //Use the user credentials only if it is not a local host or local machine
                if (IsLocalHost(Address))
                    scope = new ManagementScope(connectionUrl);
                else
                    scope = new ManagementScope(connectionUrl, connOpts);
                virtualSystemService = Utility.GetServiceObject(scope, "Msvm_VirtualSystemManagementService");
                state = ConnectState.Connected;
            }
            catch (Exception exp)
            {
                WMIFailed = true;
            }

            //Retry the connection using the old WMI Namespace for Virtualization (root\virtualization)
            // Most of the cases the above WMI namespace will work, in case that fails we do one try with root\virtualization namespace.
            if (WMIFailed)
            {
                try
                {
                    string connectionUrlOld = string.Format(@"\\{0}\root\virtualization", Address.ToString());
                    ManagementScope scope;
                    //Use the user credentials only if it is not a local host or local machine
                    if (IsLocalHost(Address))
                        scope = new ManagementScope(connectionUrlOld);
                    else
                        scope = new ManagementScope(connectionUrlOld, connOpts);
                    virtualSystemService = Utility.GetServiceObject(scope, "Msvm_VirtualSystemManagementService");
                    state = ConnectState.Connected;
                }
                catch (Exception exp)
                {
                    throw exp;
                }
            }

        }

        public void Disconnect()
        {
            if (virtualSystemService != null)
            {
                virtualSystemService.Dispose();
                virtualSystemService = null;
                state = ConnectState.Disconnected;
            }
        }
    }
}
