//------------------------------------------------------------------------------
// <copyright file="NetworkMonitor.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Net.NetworkInformation;
using BBS.TracerX;
using System.Net;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Services
{
    public class NetworkInfo
    {
        private static Logger LOG = Logger.GetLogger("NetworkInfo");

        private PhysicalAddress pa;
        private OperationalStatus status;
        private NetworkInterfaceType iftype;
        private string desc;
        private Set<IPAddress> addresses;

        public NetworkInfo(NetworkInterface ni)
        {
            this.pa = ni.GetPhysicalAddress();
            status = ni.OperationalStatus;
            iftype = ni.NetworkInterfaceType;
            desc = ni.Description;

            LOG.InfoFormat("{0} interface [{1}] '{2}' is currently {3}.", iftype, pa, desc, status);

            StringBuilder sb = new StringBuilder();
            addresses = new Set<IPAddress>();
            foreach (UnicastIPAddressInformation addr in ni.GetIPProperties().UnicastAddresses)
            {
                if (status == OperationalStatus.Up)
                {
                    sb.AppendLine("Available IP Address: " + addr.Address.ToString());
                    addresses.Add(addr.Address);
                }
            }
            if (sb.Length > 0)
                LOG.Info(sb.ToString());
            else
                LOG.Error("No available IP addresses available");
        }

        public bool Update(NetworkInterface ni)
        {
            bool result = false;
            desc = ni.Description;

            if (ni.OperationalStatus != status)
            {
                result = true;
                LOG.InfoFormat("{0} interface [{1}] status changed to {2} from {3}.", iftype, pa, ni.OperationalStatus, status);
                status = ni.OperationalStatus;
            }

            if (ni.NetworkInterfaceType != iftype)
            {
                result = true;
                iftype = ni.NetworkInterfaceType;
                LOG.InfoFormat("{0} interface [{1}] status changed to an {2} interface.", iftype, pa, ni.NetworkInterfaceType);
            }

            StringBuilder sb = new StringBuilder();
            Set<IPAddress> currentList = new Set<IPAddress>(addresses);
            foreach (UnicastIPAddressInformation addr in ni.GetIPProperties().UnicastAddresses)
            {
                IPAddress ip = addr.Address;
                if (addresses.Contains(ip))
                {
                    currentList.Remove(ip);
                }
                else
                {
                    result = true;
                    sb.AppendLine("Address added: " + ip.ToString());
                    addresses.Add(ip);
                }        
            }
            foreach (IPAddress ip in currentList)
            {
                result = true;
                addresses.Remove(ip);
                sb.AppendLine("Address removed: " + ip.ToString());
            }

            LOG.WarnFormat(sb.ToString());
            return result;
        }

        public PhysicalAddress PhysicalAddress { get { return pa; } }
    }

    public class NetworkMonitor
    {
        private static Logger LOG = Logger.GetLogger("NetworkMonitor");
        private static NetworkMonitor instance = new NetworkMonitor();
        private Dictionary<PhysicalAddress, NetworkInfo> networkInfoMap;

        private NetworkMonitor()
        {
            networkInfoMap = new Dictionary<PhysicalAddress, NetworkInfo>();
            UpdateNetworkInfo();

            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        }

        void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            using (LOG.InfoCall("NetworkChange_NetworkAvailabilityChanged"))
            {
                UpdateNetworkInfo();
            }
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            using (LOG.InfoCall("NetworkChange_NetworkAddressChanged"))
            {
                UpdateNetworkInfo();
            }
        }

        void UpdateNetworkInfo()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach(NetworkInterface n in adapters)
            {
                PhysicalAddress pa = n.GetPhysicalAddress();
                NetworkInfo ni = null;
                if (!networkInfoMap.TryGetValue(pa, out ni))
                {
                    ni = new NetworkInfo(n);
                    networkInfoMap.Add(pa, ni);
                }
                else
                {
                    ni.Update(n);
                }
            }
        }

        public static NetworkMonitor Default
        {
            get { return instance; }
        }
    }
}
