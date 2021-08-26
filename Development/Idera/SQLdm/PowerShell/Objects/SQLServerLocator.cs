//------------------------------------------------------------------------------
// <copyright file="SQLServerLocator.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell.Objects
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using Commands;

    internal class SQLServerLocator : IDisposable
    {
        private SQLBrowserQueryResponseServer server;

        public void Dispose()
        {
            if (server != null)
            {
                server.Stop();
                server = null;
            }
        }

        public SqlServerInstanceInfo[] Get()
        {
            return Get(null);
        }

        public SqlServerInstanceInfo[] Get(string domainName)
        {
            server = new SQLBrowserQueryResponseServer(0);
            server.Start();

            int port = server.Port;

            List<SqlServerInstanceInfo> result = new List<SqlServerInstanceInfo>();

            // blast out a SqlBrowser query to each server supposedly hosting an SQL Server
            foreach (NetworkServiceLocator.SERVER_INFO_101 info in NetworkServiceLocator.GetServiceHosts(NetworkServiceLocator.SV_101_TYPES.SV_TYPE_SQLSERVER, domainName))
            {
                Debug.Print("{0} maj={1} min={2}", info.sv101_name, info.sv101_version_major, info.sv101_version_minor);
                IPAddress[] addresses;
                try
                {
                    addresses = Dns.GetHostAddresses(info.sv101_name);
                } catch (Exception e)
                {
                    continue;
                }
                if (addresses.Length > 0)
                {
                    for (int i = 0; i < addresses.Length; i++)
                    {
                        try
                        {
                            SendBrowserQueryRequest(addresses[i]);
                 //           break;
                        }
                        catch (Exception e)
                        {
                                                    
                        }
                    }
                }
                System.Threading.Thread.Sleep(10);
            }

            foreach (SqlServerInstanceInfo ssi in server.WaitForResponses(TimeSpan.FromSeconds(3)))
            {
                result.Add(ssi);
            }

            return result.ToArray();
        }

        private void SendBrowserQueryRequest(IPAddress address)
        {
            try
            {
                server.SendQuery(address);
            }
            catch (SocketException socex)
            {
                const int WSAETIMEDOUT = 10060; // Connection timed out. 
                const int WSAEHOSTUNREACH = 10065; // No route to host. 
                // Re-throw if it's not a timeout.
                Console.WriteLine("{0} {1}", socex.ErrorCode, socex.Message);
                throw;
            }
        }
    }


    internal class SQLBrowserQueryResponseServer
    {
        private Dictionary<string, SqlServerInstanceInfo> instanceInfo;
        private int bufferSize = 1024;
        private Socket socket;
        private object sync = new object();
        private bool done;

        internal SQLBrowserQueryResponseServer(int port)
        {
            instanceInfo = new Dictionary<string, SqlServerInstanceInfo>();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ExclusiveAddressUse = false;

            IPEndPoint lcl = new IPEndPoint(IPAddress.Any, port);
            socket.Bind(lcl);
        }

        internal int Port
        {
            get { return ((IPEndPoint) socket.LocalEndPoint).Port; }
        }

        internal void SendQuery(IPAddress address)
        {
            byte[] msg = new byte[] { 0x02 };
            IPEndPoint ep = new IPEndPoint(address, 1434);
            socket.SendTo(msg, ep);
        }

        internal void Start()
        {
            // start 3 async receives
            BeginReceive();
            BeginReceive();
            BeginReceive();
            BeginReceive();
            BeginReceive();
            BeginReceive();
        }

        internal void Stop()
        {
            done = true;
            try { socket.Close(); } catch (Exception e) { /* */ }
        }

        internal IEnumerable<SqlServerInstanceInfo> WaitForResponses(TimeSpan waitTime)
        {
            System.Threading.Thread.Sleep(waitTime);

            IEnumerable<SqlServerInstanceInfo> result = null;
            lock(sync)
            {
                done = true;
                result = instanceInfo.Values;
                instanceInfo = null;
            }
            return result;
        }

        internal void BeginReceive()
        {
            SQLBrowserQueryResponseState state = new SQLBrowserQueryResponseState(socket, bufferSize);
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    socket.BeginReceiveFrom(state.buffer,
                                            0,
                                            bufferSize,
                                            SocketFlags.None,
                                            ref state.endpoint,
                                            OnDataReceived,
                                            state);
                    Debug.Print("BeginReceive started...");
                    break;
                }
                catch (Exception e)
                {
                    Debug.Print("BeginReceive attempt {0}: {1}", i, e.ToString());
                }
            }
        }

        private void OnDataReceived(IAsyncResult ar)
        {
            SQLBrowserQueryResponseState state = ar.AsyncState as SQLBrowserQueryResponseState;
            int bytesReceived = 0;
            try
            {
                bytesReceived = socket.EndReceiveFrom(ar, ref state.endpoint);
            } catch (Exception e)
            {
                Debug.Print(e.Message);
                if (!done)
                {   // start a new reveive operation
                    BeginReceive();
                    return;
                }
            }
            lock (sync)
            {
                // if instanceInfo is gone then we don't need to parse the recevied response
                if (instanceInfo == null)
                    return;
            }

            if (!done)
            {   // start a new reveive operation
                BeginReceive();
            }
            string data = System.Text.ASCIIEncoding.ASCII.GetString(state.buffer, 3, BitConverter.ToInt16(state.buffer, 1));
            string[] parts = data.Split(new char[] {';'}, StringSplitOptions.None);

            string serverName = String.Empty;
            string instanceName = String.Empty;
            string version = String.Empty;

            for (int i = 0; i < parts.Length; i++)
            {
                switch (parts[i].ToLower())
                {
                    case "servername":
                        serverName = parts[++i];
                        break;
                    case "instancename":
                        instanceName = parts[++i];
                        break;
                    case "version":
                        version = parts[++i];
                        break;
                    case "":
                        lock (sync)
                        {
                            if (instanceInfo != null && !String.IsNullOrEmpty(serverName) && !String.IsNullOrEmpty(instanceName))
                            {
                                SqlServerInstanceInfo instance = 
                                    new SqlServerInstanceInfo(serverName, instanceName, version);
                                serverName = instanceName = version = String.Empty;
                                if (!instanceInfo.ContainsKey(instance.Name))
                                    instanceInfo.Add(instance.Name, instance);
                            }
                        }
                        continue;
                    default:
                        ++i;
                        break;
                }
            }
        }

        internal class SQLBrowserQueryResponseState
        {
            internal readonly Socket socket;
            internal readonly byte[] buffer;
            internal EndPoint endpoint;

            internal SQLBrowserQueryResponseState(Socket socket, int bufferSize)
            {
                this.socket = socket;
                this.buffer = new byte[bufferSize];

                int port = ((IPEndPoint) socket.LocalEndPoint).Port;
                this.endpoint = new IPEndPoint(IPAddress.Any, port);
            }
        }
    }



    internal static class NetworkServiceLocator
    {
        public static SERVER_INFO_101[] GetServiceHosts(SV_101_TYPES ServerType)
        {
            return GetServiceHosts(ServerType, null);           
        }

        public static SERVER_INFO_101[] GetServiceHosts(SV_101_TYPES ServerType, string domainName)
        {
            int entriesread = 0, totalentries = 0;
            List<SERVER_INFO_101> result = new List<SERVER_INFO_101>();
            string serverName = null;
            if (domainName.Equals("dev", StringComparison.CurrentCultureIgnoreCase))
                serverName = "DEVDC-02";

            do
            {
                // Buffer to store the available servers
                // Filled by the NetServerEnum function
                IntPtr buf = new IntPtr();

                SERVER_INFO_101 server;
                int ret = NetServerEnum(
                        serverName, 
                        101, 
                        out buf, 
                        -1,
                        ref entriesread, 
                        ref totalentries,
                        ServerType, 
                        domainName, 
                        IntPtr.Zero);

                // if the function returned any data, fill the tree view
                if (ret == ERROR_SUCCESS || ret == ERROR_MORE_DATA || entriesread > 0)
                {
                    IntPtr ptr = buf;

                    for (int i = 0; i < entriesread; i++)
                    {
                        // cast pointer to a SERVER_INFO_101 structure
                        server = (SERVER_INFO_101)Marshal.PtrToStructure(ptr, typeof(SERVER_INFO_101));

                        // point to the next structure
                        ptr = new IntPtr(ptr.ToInt64() + Marshal.SizeOf(server));

                        // add the machine name and comment to the arrayList. 
                        //You could return the entire structure here if desired
                        result.Add(server);
                    }
                }

                // free the buffer 
                NetApiBufferFree(buf);

            }
            while (entriesread < totalentries && entriesread != 0);

            return result.ToArray();
        }


        // constants
        public const uint ERROR_SUCCESS = 0;
        public const uint ERROR_MORE_DATA = 234;

        [Flags]
        public enum SV_101_TYPES : uint
        {
            SV_TYPE_WORKSTATION = 0x00000001,
            SV_TYPE_SERVER = 0x00000002,
            SV_TYPE_SQLSERVER = 0x00000004,
            SV_TYPE_DOMAIN_CTRL = 0x00000008,
            SV_TYPE_DOMAIN_BAKCTRL = 0x00000010,
            SV_TYPE_TIME_SOURCE = 0x00000020,
            SV_TYPE_AFP = 0x00000040,
            SV_TYPE_NOVELL = 0x00000080,
            SV_TYPE_DOMAIN_MEMBER = 0x00000100,
            SV_TYPE_PRINTQ_SERVER = 0x00000200,
            SV_TYPE_DIALIN_SERVER = 0x00000400,
            SV_TYPE_XENIX_SERVER = 0x00000800,
            SV_TYPE_SERVER_UNIX = 0x00000800,
            SV_TYPE_NT = 0x00001000,
            SV_TYPE_WFW = 0x00002000,
            SV_TYPE_SERVER_MFPN = 0x00004000,
            SV_TYPE_SERVER_NT = 0x00008000,
            SV_TYPE_POTENTIAL_BROWSER = 0x00010000,
            SV_TYPE_BACKUP_BROWSER = 0x00020000,
            SV_TYPE_MASTER_BROWSER = 0x00040000,
            SV_TYPE_DOMAIN_MASTER = 0x00080000,
            SV_TYPE_SERVER_OSF = 0x00100000,
            SV_TYPE_SERVER_VMS = 0x00200000,
            SV_TYPE_WINDOWS = 0x00400000,
            SV_TYPE_DFS = 0x00800000,
            SV_TYPE_CLUSTER_NT = 0x01000000,
            SV_TYPE_TERMINALSERVER = 0x02000000,
            SV_TYPE_CLUSTER_VS_NT = 0x04000000,
            SV_TYPE_DCE = 0x10000000,
            SV_TYPE_ALTERNATE_XPORT = 0x20000000,
            SV_TYPE_LOCAL_LIST_ONLY = 0x40000000,
            SV_TYPE_DOMAIN_ENUM = 0x80000000,
            SV_TYPE_ALL = 0xFFFFFFFF
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct SERVER_INFO_101
        {
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 sv101_platform_id;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string sv101_name;

            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 sv101_version_major;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 sv101_version_minor;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.U4)]
            public UInt32 sv101_type;
            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
            public string sv101_comment;
        };

        public enum PLATFORM_ID
        {
            PLATFORM_ID_DOS = 300,
            PLATFORM_ID_OS2 = 400,
            PLATFORM_ID_NT = 500,
            PLATFORM_ID_OSF = 600,
            PLATFORM_ID_VMS = 700
        }

        [DllImport("netapi32.dll", EntryPoint = "NetServerEnum")]
        public static extern int NetServerEnum([MarshalAs(UnmanagedType.LPWStr)]string servername,
           int level,
           out IntPtr bufptr,
           int prefmaxlen,
           ref int entriesread,
           ref int totalentries,
           SV_101_TYPES servertype,
           [MarshalAs(UnmanagedType.LPWStr)]string domain,
           IntPtr resume_handle);

        [DllImport("netapi32.dll", EntryPoint = "NetApiBufferFree")]
        public static extern int NetApiBufferFree(IntPtr buffer);
    }
}
