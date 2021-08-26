using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using Idera.SQLdm.Common.VMware;
using System.Runtime.Serialization;
using Idera.SQLdm.Common.Configuration;
using System.Threading;
using Idera.SQLdm.Common.Snapshots;
using Vim25Api;
using System.Diagnostics;

namespace Idera.SQLdm.Common.HyperV
{
    public class HyperVService
    {
        HyperVServiceConnection connection;
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("HyperVServiceUtil");
        private string url = null;

        AutoResetEvent EventMemory = new AutoResetEvent(false);
        AutoResetEvent EventCPU = new AutoResetEvent(false);
        AutoResetEvent EventDisk = new AutoResetEvent(false);
        AutoResetEvent EventNetwork = new AutoResetEvent(false);
        AutoResetEvent EventMemoryVM = new AutoResetEvent(false);
        AutoResetEvent EventCPUVM = new AutoResetEvent(false);
        AutoResetEvent EventDiskVM = new AutoResetEvent(false);
        AutoResetEvent EventNetworkVM = new AutoResetEvent(false);
        AutoResetEvent EventMemoryUsages = new AutoResetEvent(false);
        AutoResetEvent EventMemoryUsagesVM = new AutoResetEvent(false);
        AutoResetEvent EventConfigData = new AutoResetEvent(false);
        AutoResetEvent EventConfigDataVM = new AutoResetEvent(false);


        public string URL
        {
            get { return url; }
            set { url = value; }
        }

        public ConnectState ConnectState
        {
            get { return connection.State; }
        }

        public HyperVService(string hurl)
        {
            URL = hurl;
            connection = new HyperVServiceConnection(URL);
        }

        public void Connect(string username, string password)
        {
            try
            {
                if (url != null && username != null && password != null)
                {
                    connection.Connect(username, password);
                }
                else
                {
                    LOG.Error("HyperV - Missing the url, user or password.  All three parameters are needed to connection");
                    throw new Exception("Missing the url, user or password.  All three parameters are needed to connection");
                }
            }
            catch (Exception e)
            {
                // Log exception
                LOG.Error("HyperV - Connection to Server failed : " + connection.Address, e);
                throw e;
            }
        }

        public void Disconnect()
        {
            try
            {
                connection.Disconnect();
            }
            catch (Exception e)
            {
                // Log Exception
                throw e;
            }
        }


        public Dictionary<string, VMware.basicVMInfo> GetListOfVMs(Dictionary<string, string> filter)
        {
            if (connection.State != HyperV.ConnectState.Connected)
            {
                throw new HyperConnectionException("Not connected to HyperV Server");
            }
            Dictionary<string, basicVMInfo> result = new Dictionary<string, basicVMInfo>();

            ManagementBaseObject inParams = connection.VirtualSystemService.GetMethodParameters("GetSummaryInformation");

            UInt32[] requestedInformation = new UInt32[16];
            requestedInformation[0] = 1;    // ElementName
            requestedInformation[2] = 103;  // MemoryUsage
            requestedInformation[3] = 112;  // MemoryAvailable

            requestedInformation[4] = 4;    // Number of Processors :4
            requestedInformation[5] = 8;    // AllocatedGPU :8
            requestedInformation[6] = 100;  // EnabledState :100
            requestedInformation[7] = 101;  // ProcessorLoad :101
            requestedInformation[8] = 102;  // ProcessorLoadHistory :102
            requestedInformation[9] = 104;  // Heartbeat :104
            requestedInformation[10] = 105;  // Uptime :105
            requestedInformation[11] = 106;  // GuestOperatingSystem :106
            requestedInformation[12] = 110;  // OperationalStatus :110
            requestedInformation[13] = 111; // StatusDescriptions :111
            requestedInformation[14] = 113; // AvailableMemoryBuffer :113
            requestedInformation[15] = 2; // CreationTime :2

            inParams["RequestedInformation"] = requestedInformation;
            ManagementBaseObject outParams = connection.VirtualSystemService.InvokeMethod("GetSummaryInformation", inParams, null);
            string vmName, hostName, uuid;
            vmName = hostName = uuid = "";
            if ((UInt32)outParams["ReturnValue"] == HyperV.ReturnCode.Completed)
            {
                ManagementBaseObject[] summaryInformationArray = (ManagementBaseObject[])outParams["SummaryInformation"];
                foreach (ManagementBaseObject summaryInformation in summaryInformationArray)
                {
                    if ((null == summaryInformation["Name"]) || (summaryInformation["Name"].ToString().Length == 0))
                    {
                        break;
                    }
                    else
                    {
                        uuid = summaryInformation["Name"].ToString();
                        foreach (UInt32 requested in requestedInformation)
                        {
                            switch (requested)
                            {
                                case 1:
                                    if (summaryInformation["ElementName"] != null)
                                    {
                                        vmName = summaryInformation["ElementName"].ToString();
                                        hostName = summaryInformation["ElementName"].ToString();
                                    }
                                    break;

                            }
                        }
                    }
                    if (!result.ContainsKey(uuid))
                    {
                        result.Add(uuid, new basicVMInfo(URL, vmName, hostName, uuid));
                    }
                }

            }

            return result;
        }

        public VMwareVirtualMachine CollectVMInfo(VirtualizationConfiguration vConfig, int iterations, ScheduledRefresh previousRefresh, VMwareVirtualMachine previousVconfig, WmiConfiguration wmiConfiguration)
        {
            string instanceUuid = vConfig.InstanceUUID;

            VMwareVirtualMachine vmProperties = new VMwareVirtualMachine();
            vmProperties.InstanceUUID = vConfig.InstanceUUID;
            if (previousRefresh == null && previousVconfig == null)
            {
                return vmProperties;
            }
            string serverAddress = connection.Address;
            string virtualMachineName = vConfig.VMName;

            Stopwatch timer = new Stopwatch();
            timer.Start();
            LOG.InfoFormat("HyperV VM Collector for VM {0}, HOST {1} started", virtualMachineName, serverAddress);

            Thread tConfigData = new Thread(() => CollectConfigData(serverAddress, false, vConfig, ref vmProperties, previousRefresh, previousVconfig, null));
			Thread tConfigDataVM = new Thread(() => CollectConfigData(virtualMachineName, true, vConfig, ref vmProperties, previousRefresh, previousVconfig, wmiConfiguration));
			Thread tMemoryVM = new Thread(() => MemoryCollection(virtualMachineName, true, vConfig, ref vmProperties, previousRefresh, previousVconfig, wmiConfiguration));
			Thread tCPUVM = new Thread(() => CPUCollection(virtualMachineName, true, vConfig, ref vmProperties, previousRefresh, previousVconfig, wmiConfiguration));
			Thread tDiskVM = new Thread(() => DiskCollection(virtualMachineName, true, vConfig, ref vmProperties, previousRefresh, previousVconfig, wmiConfiguration));
			Thread tNetWorkVM = new Thread(() => NetworkCollection(virtualMachineName, serverAddress, true, vConfig, ref vmProperties, previousRefresh, previousVconfig, wmiConfiguration));
			Thread tMemory = new Thread(() => MemoryCollection(serverAddress, false, vConfig, ref vmProperties, previousRefresh, previousVconfig, null));
			Thread tCPU = new Thread(() => CPUCollection(serverAddress, false, vConfig, ref vmProperties, previousRefresh, previousVconfig, null));
			Thread tDisk = new Thread(() => DiskCollection(serverAddress, false, vConfig, ref vmProperties, previousRefresh, previousVconfig, null));
			Thread tNetwork = new Thread(() => NetworkCollection(virtualMachineName, serverAddress, false, vConfig, ref vmProperties, previousRefresh, previousVconfig, null));
			Thread tMemoryUsages = new Thread(() => MemoryUsagesCollection(serverAddress, false, vConfig, ref vmProperties, previousRefresh, previousVconfig, null));
			Thread tMemoryUsagesVM = new Thread(() => MemoryUsagesCollection(virtualMachineName, true, vConfig, ref vmProperties, previousRefresh, previousVconfig, wmiConfiguration));

            tConfigData.Start();
            tConfigDataVM.Start();
            tMemoryVM.Start();
            tMemory.Start();
            tCPU.Start();
            tCPUVM.Start();
            tDisk.Start();
            tDiskVM.Start();
            tNetwork.Start();
            tNetWorkVM.Start();
            tMemoryUsages.Start();
            tMemoryUsagesVM.Start();


            AutoResetEvent[] evs = new AutoResetEvent[12];
            evs[0] = EventMemory;
            evs[1] = EventCPU;
            evs[2] = EventDisk;
            evs[3] = EventMemoryVM;
            evs[4] = EventCPUVM;
            evs[5] = EventDiskVM;
            evs[6] = EventNetwork;
            evs[7] = EventNetworkVM;
            evs[8] = EventMemoryUsages;
            evs[9] = EventMemoryUsagesVM;
            evs[10] = EventConfigData;
            evs[11] = EventConfigDataVM;

            WaitHandle.WaitAll(evs);
            double hostAvlBytes = vmProperties.ESXHost.PerfStats.AvaialableByteHyperV.Bytes == null ? 0 : (double)vmProperties.ESXHost.PerfStats.AvaialableByteHyperV.Bytes;
            double vmAvlBytes = vmProperties.PerfStats.AvaialableByteHyperV.Bytes == null ? 0 : (double)vmProperties.PerfStats.AvaialableByteHyperV.Bytes;

            double memoryusage = ((vmProperties.ESXHost.PerfStats.MemUsage - hostAvlBytes) / vmProperties.ESXHost.PerfStats.MemUsage) * 100;
            double memoryusageVm = ((vmProperties.PerfStats.MemUsage - vmAvlBytes) / vmProperties.PerfStats.MemUsage) * 100;
            vmProperties.ESXHost.PerfStats.MemUsage = memoryusage;
            vmProperties.PerfStats.MemUsage = memoryusageVm;
            timer.Stop();
            LOG.InfoFormat("HyperV VM Collector for VM {0}, HOST {1} completed in {2} ms", virtualMachineName, serverAddress, timer.ElapsedMilliseconds);
            return vmProperties;
        }

        void CollectConfigData(string virtualMachineName, bool isVM, VirtualizationConfiguration vConfig, ref VMwareVirtualMachine vmProperties, ScheduledRefresh previousRefresh, VMwareVirtualMachine previousVconfig, WmiConfiguration wmiConfiguration)
        {
            try
            {
                LOG.Info(" HyperV - Config Information collection start for : " + virtualMachineName);
				ManagementScope _scope = null;
				if (isVM)
				{
					_scope = CreateNewManagementScopeCimv2VM(virtualMachineName, wmiConfiguration);
				}
				else
				{
					_scope = CreateNewManagementScopeCimv2(virtualMachineName, vConfig);
				}
				
				LOG.DebugFormat("ServerName: {0}, isVM: {1} URL: {2}, Username: {3}", virtualMachineName, isVM, _scope.Path, _scope.Options.Username);				
				
                SelectQuery wmiConfigQuery1 = new SelectQuery("SELECT LastBootUpTime FROM Win32_OperatingSystem");
                using (ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(_scope, wmiConfigQuery1))
                {
                    ManagementObjectCollection configUCollection = searcher1.Get();
                    foreach (ManagementObject configObject in configUCollection)
                    {
                        DateTime bootTime = ManagementDateTimeConverter.ToDateTime(configObject["LastBootUpTime"].ToString());
                        if (isVM)
                        {
                            vmProperties.BootTime = bootTime;
                        }
                        else
                        {
                            vmProperties.ESXHost.BootTime = bootTime;
                        }

                    }
                }

            }
            catch (Exception exception)
            {
                LOG.Warn(" HyperV - There was a problem collecting Config Information for : " + virtualMachineName, exception);
            }
            finally
            {
                if (isVM)
                    EventConfigDataVM.Set();
                else
                    EventConfigData.Set();
                LOG.Info(" HyperV - Exiting Config Information collection for : " + virtualMachineName);
            }
        }

        void MemoryUsagesCollection(string virtualMachineName, bool isVM, VirtualizationConfiguration vConfig, ref VMwareVirtualMachine vmProperties, ScheduledRefresh previousRefresh, VMwareVirtualMachine previousVconfig, WmiConfiguration wmiConfiguration)
        {
            try
            {
                LOG.Info(" HyperV - Memory usages collection start for : " + virtualMachineName);
                
				ManagementScope _scope = null;
				if (isVM)
				{
					_scope = CreateNewManagementScopeCimv2VM(virtualMachineName, wmiConfiguration);
				}
				else
				{
					_scope = CreateNewManagementScopeCimv2(virtualMachineName, vConfig);
				}
				LOG.DebugFormat("ServerName: {0}, isVM: {1} URL: {2}, Username: {3}", virtualMachineName, isVM, _scope.Path, _scope.Options.Username);
				
				SelectQuery wmiMemoryQuery = new SelectQuery("SELECT Name,TotalPhysicalMemory,NumberOfProcessors,DNSHostName,Domain,PowerState FROM Win32_ComputerSystem");
                using (ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(_scope, wmiMemoryQuery))
                {
                    ManagementObjectCollection memoryUCollection = searcher1.Get();
                    foreach (ManagementObject memoryObject in memoryUCollection)
                    {
                        UInt64? TotalPhysicalMemory = GetPropertyValue<UInt64>(memoryObject, "TotalPhysicalMemory");
                        string hostName = memoryObject["DNSHostName"].ToString();
                        VirtualMachinePowerState vmStatus = VirtualMachinePowerState.poweredOff;
                        HostSystemPowerState esxStatus = HostSystemPowerState.poweredOff;
                        string name = memoryObject["Name"].ToString();
                        UInt32? NumCPU = GetPropertyValue<UInt32>(memoryObject, "NumberOfProcessors");
                        string domainName = memoryObject["Domain"].ToString();
                        if (isVM)
                        {
                            if (name != null && name.Length > 0)
                            {
                                vmStatus = VirtualMachinePowerState.poweredOn;
                            }
                            vmProperties.Name = name;
                            vmProperties.Status = vmStatus;
                            vmProperties.NumCPUs = (int)NumCPU;
                            vmProperties.HostName = hostName;
                            vmProperties.PerfStats.MemUsage = (ulong)TotalPhysicalMemory;
                            vmProperties.MemSize.Bytes = (ulong)TotalPhysicalMemory;
                        }
                        else
                        {
                            if (name != null && name.Length > 0)
                            {
                                esxStatus = HostSystemPowerState.poweredOn;
                            }
                            vmProperties.ESXHost.Name = name;
                            vmProperties.ESXHost.Status = esxStatus;
                            vmProperties.ESXHost.DomainName = hostName;
                            vmProperties.ESXHost.NumCPUCores = (short)NumCPU;
                            vmProperties.ESXHost.PerfStats.MemUsage = (ulong)TotalPhysicalMemory;

                            //SQLDM-28327. Changing the Memory Size Units to Bytes from MegaBytes.
                            vmProperties.ESXHost.MemSize.Bytes = (ulong)TotalPhysicalMemory;

                           
                        }

                    }
                }

                //SQLDM-28327 Calculating the Number of Physical and logical Processors for the Virtualization Host server.
                //START
                
                 SelectQuery wmiProcessorQuery = new SelectQuery("SELECT NumberOfLogicalProcessors,NumberOfCores FROM WIN32_PROCESSOR");
                 UInt32? HostNumCPU = 0;
                 UInt32? HostNumCPUThreads = 0;
                 using (ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(_scope, wmiProcessorQuery))
                {
                    ManagementObjectCollection processorCollection = searcher1.Get();
                    foreach (ManagementObject processorObject in processorCollection)
                    {
                       
                        if (!isVM)
                        {
                            HostNumCPU += GetPropertyValue<UInt32>(processorObject, "NumberOfCores");
                            HostNumCPUThreads += GetPropertyValue<UInt32>(processorObject, "NumberOfLogicalProcessors");
                        }
                       
                    }
                    
                }
                if(!isVM){
                        vmProperties.ESXHost.NumCPUCores = (short)HostNumCPU;
                        vmProperties.ESXHost.NumCPUThreads = (short)HostNumCPUThreads;
                }

                //END


            }
            catch (Exception exception)
            {
                LOG.Warn(" HyperV - There was a problem collecting Memory usages data for : " + virtualMachineName, exception);
            }
            finally
            {
                if (isVM)
                    EventMemoryUsagesVM.Set();
                else
                    EventMemoryUsages.Set();
                LOG.Info(" HyperV - Exiting Memory usages collection for : " + virtualMachineName);
            }
        }

        void NetworkCollection(string virtualMachineName, string serverAddress, bool isVM, VirtualizationConfiguration vConfig, ref VMwareVirtualMachine vmProperties, ScheduledRefresh pRefresh, VMwareVirtualMachine previousStatistics, WmiConfiguration wmiConfiguration)
        {
            try
            {
                LOG.Info(" HyperV - Network usages collection start for : " + virtualMachineName);
				
                ManagementScope _scope = CreateNewManagementScopeCimv2(serverAddress, vConfig);
				
				LOG.DebugFormat("HostService: {0}, ServerName: {1}, isVM: {2} URL: {3}, Username: {4}", 
					serverAddress, virtualMachineName, isVM, _scope.Path, _scope.Options.Username);
					
                SelectQuery wmiNetworkyQuery;
                if (isVM)
                {
                    wmiNetworkyQuery = new SelectQuery(string.Format("SELECT * FROM Win32_PerfRawData_NvspNicStats_HyperVVirtualNetworkAdapter WHERE Name like '{0}%'", virtualMachineName));
                }
                else
                {
                    wmiNetworkyQuery = new SelectQuery("SELECT * FROM Win32_PerfRawData_Tcpip_NetworkInterface");
                }


                using (ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(_scope, wmiNetworkyQuery))
                {
                    ManagementObjectCollection networkCollection = searcher1.Get();

                    UInt32? BytesSentPerSec = 0;
                    UInt32? BytesReceivedPerSec = 0;
                    UInt64? Timestamp_PerfTime_Network = 0;
                    UInt64? Frequency_PerfTime = 0;
                    UInt64? NetUsage = 0;
                    double? delta;
                    foreach (ManagementObject networkObject in networkCollection)
                    {
                        BytesSentPerSec = GetPropertyValue<UInt32>(networkObject, "BytesSentPerSec") != null ? (BytesSentPerSec + (GetPropertyValue<UInt32>(networkObject, "BytesSentPerSec") / 1024)) : BytesSentPerSec;
                        BytesReceivedPerSec = GetPropertyValue<UInt32>(networkObject, "BytesReceivedPerSec") != null ? (BytesReceivedPerSec + (GetPropertyValue<UInt32>(networkObject, "BytesReceivedPerSec") / 1024)) : BytesReceivedPerSec;
                        if (isVM)
                        {
                            NetUsage = GetPropertyValue<UInt32>(networkObject, "BytesPersec") != null ? (NetUsage + (GetPropertyValue<UInt32>(networkObject, "BytesPersec") / 1024)) : NetUsage;
                        }
                        else
                        {
                            NetUsage = GetPropertyValue<UInt32>(networkObject, "BytesTotalPersec") != null ? (NetUsage + (GetPropertyValue<UInt32>(networkObject, "BytesTotalPersec") / 1024)) : NetUsage;
                        }
                        Timestamp_PerfTime_Network = GetPropertyValue<UInt64>(networkObject, "Timestamp_PerfTime");
                        Frequency_PerfTime = GetPropertyValue<UInt64>(networkObject, "Frequency_PerfTime");
                    }
                    if ((pRefresh != null && pRefresh.Server != null && pRefresh.Server.VMConfig != null) || previousStatistics != null)
                    {
                        if (isVM)
                        {

                            double? p_TimeStamp_PerfTime_Network_Raw = pRefresh == null ? previousStatistics.PerfStats.HyperCommonObject.Timestamp_PerfTime_Network_Raw
                                                                                 : pRefresh.Server.VMConfig.PerfStats.HyperCommonObject.Timestamp_PerfTime_Network_Raw;

                            double? p_NetRecived_Raw = pRefresh == null ? previousStatistics.PerfStats.NetReceived_Raw
                                                                                : pRefresh.Server.VMConfig.PerfStats.NetReceived_Raw;
                            double? p_NetTransmitted_Raw = pRefresh == null ? previousStatistics.PerfStats.NetTransmitted_Raw
                                                                                : pRefresh.Server.VMConfig.PerfStats.NetTransmitted_Raw;

                            double? p_NetUsage_Raw = pRefresh == null ? previousStatistics.PerfStats.NetUsage_Raw
                                                                                : pRefresh.Server.VMConfig.PerfStats.NetUsage_Raw;
                            double? Timestamp_PerfTime_Delta = OSMetrics.Calculate_Timer_Delta(p_TimeStamp_PerfTime_Network_Raw, Timestamp_PerfTime_Network);
                            vmProperties.PerfStats.NetReceived_Raw = (long)BytesReceivedPerSec;
                            vmProperties.PerfStats.NetTransmitted_Raw = (long)BytesSentPerSec;
                            vmProperties.PerfStats.NetUsage_Raw = (long)NetUsage;
                            vmProperties.PerfStats.HyperCommonObject.Timestamp_PerfTime_Network_Raw = (long)Timestamp_PerfTime_Network;
                            vmProperties.PerfStats.NetReceived = (long)OSMetrics.Calculate_PERF_COUNTER_COUNTER(p_NetRecived_Raw, BytesReceivedPerSec,
                                                          out delta, Timestamp_PerfTime_Delta, Frequency_PerfTime);
                            vmProperties.PerfStats.NetTransmitted = (long)OSMetrics.Calculate_PERF_COUNTER_COUNTER(p_NetTransmitted_Raw, BytesSentPerSec,
                                                          out delta, Timestamp_PerfTime_Delta, Frequency_PerfTime);

                            vmProperties.PerfStats.NetUsage = (long)OSMetrics.Calculate_PERF_COUNTER_COUNTER(p_NetUsage_Raw, NetUsage,
                                                          out delta, Timestamp_PerfTime_Delta, Frequency_PerfTime);

                        }
                        else
                        {
                            double? p_TimeStamp_PerfTime_Network_Raw = pRefresh == null ? previousStatistics.ESXHost.PerfStats.HyperCommonObject.Timestamp_PerfTime_Network_Raw
                                                                                : pRefresh.Server.VMConfig.ESXHost.PerfStats.HyperCommonObject.Timestamp_PerfTime_Network_Raw;

                            double? p_NetRecived_Raw = pRefresh == null ? previousStatistics.ESXHost.PerfStats.NetReceived_Raw
                                                                                : pRefresh.Server.VMConfig.ESXHost.PerfStats.NetReceived_Raw;
                            double? p_NetTransmitted_Raw = pRefresh == null ? previousStatistics.ESXHost.PerfStats.NetTransmitted_Raw
                                                                                : pRefresh.Server.VMConfig.ESXHost.PerfStats.NetTransmitted_Raw;

                            double? p_NetUsage_Raw = pRefresh == null ? previousStatistics.ESXHost.PerfStats.NetUsage_Raw
                                                                               : pRefresh.Server.VMConfig.ESXHost.PerfStats.NetUsage_Raw;

                            double? Timestamp_PerfTime_Delta = OSMetrics.Calculate_Timer_Delta(p_TimeStamp_PerfTime_Network_Raw, Timestamp_PerfTime_Network);
                            vmProperties.ESXHost.PerfStats.NetReceived_Raw = (long)BytesReceivedPerSec;
                            vmProperties.ESXHost.PerfStats.NetTransmitted_Raw = (long)BytesSentPerSec;
                            vmProperties.ESXHost.PerfStats.NetUsage_Raw = (long)NetUsage;

                            vmProperties.ESXHost.PerfStats.HyperCommonObject.Timestamp_PerfTime_Network_Raw = (long)Timestamp_PerfTime_Network;
                            vmProperties.ESXHost.PerfStats.NetReceived = (long)OSMetrics.Calculate_PERF_COUNTER_COUNTER(p_NetRecived_Raw, BytesReceivedPerSec,
                                                          out delta, Timestamp_PerfTime_Delta, Frequency_PerfTime);
                            vmProperties.ESXHost.PerfStats.NetTransmitted = (long)OSMetrics.Calculate_PERF_COUNTER_COUNTER(p_NetTransmitted_Raw, BytesSentPerSec,
                                                          out delta, Timestamp_PerfTime_Delta, Frequency_PerfTime);

                            vmProperties.ESXHost.PerfStats.NetUsage = (long)OSMetrics.Calculate_PERF_COUNTER_COUNTER(p_NetUsage_Raw, NetUsage,
                                                          out delta, Timestamp_PerfTime_Delta, Frequency_PerfTime);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LOG.Warn(" HyperV - There was a problem collecting Network data for : " + virtualMachineName, exception);
            }
            finally
            {
                if (isVM)
                    EventNetworkVM.Set();
                else
                    EventNetwork.Set();
                LOG.Info(" HyperV -  Exiting Network usages collection for : " + virtualMachineName);
            }
        }

        void MemoryCollection(string name, bool isVM, VirtualizationConfiguration vConfig, ref VMwareVirtualMachine vmProperties, ScheduledRefresh pRefresh, VMwareVirtualMachine previousStatistics, WmiConfiguration wmiConfiguration)
        {
            try
            {
                LOG.Info(" HyperV - Avaiable Memory collection start for : " + name);
                ManagementScope _scope = null;
				if(isVM)
				{
					_scope = CreateNewManagementScopeCimv2VM(name, wmiConfiguration);
				}
				else
				{
					_scope = CreateNewManagementScopeCimv2(name, vConfig);
				}
				LOG.DebugFormat("ServerName: {0}, isVM: {1} URL: {2}, Username: {3}", name, isVM, _scope.Path, _scope.Options.Username);
				
                SelectQuery wmiMemoryQuery = new SelectQuery("SELECT AvailableMBytes,PagesPersec,Frequency_PerfTime,Timestamp_PerfTime,Timestamp_Sys100NS FROM Win32_PerfRawData_PerfOS_Memory");
                using (ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(_scope, wmiMemoryQuery))
                {
                    ManagementObjectCollection memoryCollection = searcher1.Get();
                    double? delta;
                    foreach (ManagementObject memoryObject in memoryCollection)
                    {
                        UInt64? AvailableMBytes = GetPropertyValue<UInt64>(memoryObject, "AvailableMBytes");
                        UInt32? PagesPersec = GetPropertyValue<UInt32>(memoryObject, "PagesPersec");
                        UInt64? Frequency_PerfTime_Memory = GetPropertyValue<UInt64>(memoryObject, "Frequency_PerfTime");
                        UInt64? Timestamp_PerfTime_Memory = GetPropertyValue<UInt64>(memoryObject, "Timestamp_PerfTime");
                        if ((pRefresh != null && pRefresh.Server != null && pRefresh.Server.VMConfig != null) || previousStatistics != null)
                        {
                            if (isVM)
                            {
                                vmProperties.PerfStats.AvaialableByteHyperV.Megabytes = AvailableMBytes;
                                double? p_TimeStamp_PerfTime_Memory_Raw = pRefresh == null ? previousStatistics.PerfStats.HyperCommonObject.Timestamp_PerfTime_Memory_Raw
                                                                                : pRefresh.Server.VMConfig.PerfStats.HyperCommonObject.Timestamp_PerfTime_Memory_Raw;
                                double? p_Frequency_PerfTime_Memory_Raw = pRefresh == null ? previousStatistics.PerfStats.HyperCommonObject.Frequency_PerfTime_Memory_Raw
                                                                               : pRefresh.Server.VMConfig.PerfStats.HyperCommonObject.Frequency_PerfTime_Memory_Raw;
                                double? p_PagesPersec_Raw = pRefresh == null ? previousStatistics.PerfStats.PagePerSec_Raw : pRefresh.Server.VMConfig.PerfStats.PagePerSec_Raw;
                                double? TimeStamp_PerfTime_Delta = OSMetrics.Calculate_Timer_Delta(p_TimeStamp_PerfTime_Memory_Raw, Timestamp_PerfTime_Memory);
                                vmProperties.PerfStats.PagePerSec_Raw = (long)PagesPersec;
                                vmProperties.PerfStats.HyperCommonObject.Timestamp_PerfTime_Memory_Raw = Timestamp_PerfTime_Memory;
                                vmProperties.PerfStats.HyperCommonObject.Frequency_PerfTime_Memory_Raw = Frequency_PerfTime_Memory;
                                vmProperties.PerfStats.PagePerSec = (long)OSMetrics.Calculate_PERF_COUNTER_COUNTER(p_PagesPersec_Raw, PagesPersec, out delta,
                                                                                                                     TimeStamp_PerfTime_Delta,
                                                                                                                                        p_Frequency_PerfTime_Memory_Raw);

                            }
                            else
                            {
                                vmProperties.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes = AvailableMBytes;
                                double? p_TimeStamp_PerfTime_Memory_Raw = pRefresh == null ? previousStatistics.ESXHost.PerfStats.HyperCommonObject.Timestamp_PerfTime_Memory_Raw
                                                                                : pRefresh.Server.VMConfig.ESXHost.PerfStats.HyperCommonObject.Timestamp_PerfTime_Memory_Raw;
                                double? p_Frequency_PerfTime_Memory_Raw = pRefresh == null ? previousStatistics.ESXHost.PerfStats.HyperCommonObject.Frequency_PerfTime_Memory_Raw
                                                                               : pRefresh.Server.VMConfig.ESXHost.PerfStats.HyperCommonObject.Frequency_PerfTime_Memory_Raw;
                                double? p_PagesPersec_Raw = pRefresh == null ? previousStatistics.ESXHost.PerfStats.PagePerSec_Raw : pRefresh.Server.VMConfig.ESXHost.PerfStats.PagePerSec_Raw;
                                double? TimeStamp_PerfTime_Delta = OSMetrics.Calculate_Timer_Delta(p_TimeStamp_PerfTime_Memory_Raw, Timestamp_PerfTime_Memory);
                                vmProperties.ESXHost.PerfStats.PagePerSec_Raw = (long)PagesPersec;
                                vmProperties.ESXHost.PerfStats.HyperCommonObject.Timestamp_PerfTime_Memory_Raw = Timestamp_PerfTime_Memory;
                                vmProperties.ESXHost.PerfStats.HyperCommonObject.Frequency_PerfTime_Memory_Raw = Frequency_PerfTime_Memory;
                                vmProperties.ESXHost.PerfStats.PagePerSec = (long)OSMetrics.Calculate_PERF_COUNTER_COUNTER(p_PagesPersec_Raw, PagesPersec, out delta,
                                                                                                                     TimeStamp_PerfTime_Delta,
                                                                                                                                        p_Frequency_PerfTime_Memory_Raw);
                            }
                        }

                    }
                }

            }
            catch (Exception exception)
            {
                LOG.Warn(" HyperV - There was a problem collecting available memory data for : " + name, exception);
            }
            finally
            {
                if (isVM)
                    EventMemoryVM.Set();
                else
                    EventMemory.Set();
                LOG.Info(" HyperV -  Exiting available memory collection for : " + name);
            }


        }

        void CPUCollection(string name, bool isVM, VirtualizationConfiguration vConfig, ref VMwareVirtualMachine vmProperties, ScheduledRefresh pRefresh, VMwareVirtualMachine previousStatistics, WmiConfiguration wmiConfiguration)
        {
            try
            {
                
                LOG.Info(" HyperV - CPU usages collection start for : " + name);
                ManagementScope _scope = null;
				if (isVM)
				{
					_scope = CreateNewManagementScopeCimv2VM(name, wmiConfiguration);
				}
				else
				{
					_scope = CreateNewManagementScopeCimv2(name, vConfig);
				}
				LOG.DebugFormat("ServerName: {0}, isVM: {1} URL: {2}, Username: {3}", name, isVM, _scope.Path, _scope.Options.Username);
				
                SelectQuery wmiCPUQuery = new SelectQuery("SELECT PercentProcessorTime,PercentPrivilegedTime,PercentUserTime,Timestamp_Sys100NS FROM Win32_PerfRawData_PerfOS_Processor WHERE Name = '_Total'");

                if ((pRefresh != null && pRefresh.Server != null && pRefresh.Server.VMConfig != null) || previousStatistics != null)
                {

                    if (isVM)
                    {

                        using (ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(_scope, wmiCPUQuery))
                        {
                            ManagementObjectCollection cpuCollection = searcher1.Get();
                            foreach (ManagementObject cpuObject in cpuCollection)
                            {
                                UInt64? PercentProcessorTime = GetPropertyValue<UInt64>(cpuObject, "PercentProcessorTime");
                                UInt64? PercentUserTime = GetPropertyValue<UInt64>(cpuObject, "PercentUserTime");
                                UInt64? Timestamp_Sys100NS = GetPropertyValue<UInt64>(cpuObject, "Timestamp_Sys100NS");
                                double? delta;

                                vmProperties.PerfStats.HyperCommonObject.TimeStamp_Sys100NS_Raw = Timestamp_Sys100NS;
                                double? p_TimeStamp_Sys100NS_Raw = pRefresh == null ? previousStatistics.PerfStats.HyperCommonObject.TimeStamp_Sys100NS_Raw : pRefresh.Server.VMConfig.PerfStats.HyperCommonObject.TimeStamp_Sys100NS_Raw;
                                double? TimeStamp_Sys100NS_Delta = OSMetrics.Calculate_Timer_Delta(p_TimeStamp_Sys100NS_Raw, Timestamp_Sys100NS);
                                double? p_CpuUsage_Raw = pRefresh == null ? previousStatistics.PerfStats.CpuUsage_Raw : pRefresh.Server.VMConfig.PerfStats.CpuUsage_Raw;
                                vmProperties.PerfStats.CpuUsage = (UInt64)OSMetrics.Calculate_PERF_100NSEC_TIMER_INV(p_CpuUsage_Raw, (UInt64)PercentProcessorTime, out delta, TimeStamp_Sys100NS_Delta);
                                vmProperties.PerfStats.CpuUsage_Raw = (UInt64)PercentProcessorTime;

                            }
                        }
                    }
                    //SQLDM-28327. Calculating the CPU Usage using the new wmi counter Win32_PerfRawData_HvStats_HyperVHypervisorLogicalProcessor
                    else
                    {

                     
                        UInt64? cpuUsage = 0;

                        SelectQuery HostCPUUsage = new SelectQuery("SELECT * FROM Win32_PerfFormattedData_PerfOS_Processor WHERE Name = '_Total'");
                        using (ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(_scope, HostCPUUsage))
                        {
                            ManagementObjectCollection cpuCollection1 = searcher1.Get();
                            foreach (ManagementObject cpuObject1 in cpuCollection1)
                            {
                                cpuUsage = GetPropertyValue<UInt64>(cpuObject1, "PercentProcessorTime");
                            }
                        }
                        vmProperties.ESXHost.PerfStats.CpuUsage = (UInt64)cpuUsage;
                    }
                }
            }//END.
            catch (Exception exception)
            {
                LOG.Warn(" HyperV - There was a problem collecting CPU usages data for : " + name, exception);
            }
            finally
            {
                if (isVM)
                    EventCPUVM.Set();
                else
                    EventCPU.Set();
                LOG.Info(" HyperV -  Exiting CPU usages collection for : " + name);
            }
        }


        void DiskCollection(string name, bool isVM, VirtualizationConfiguration vConfig, ref VMwareVirtualMachine vmProperties, ScheduledRefresh pRefresh, VMwareVirtualMachine previousStatistics, WmiConfiguration wmiConfiguration)
        {
            try
            {
                LOG.Info(" HyperV - Disk data collection start for : " + name);
                ManagementScope _scope = null;
				if (isVM)
				{
					_scope = CreateNewManagementScopeCimv2VM(name, wmiConfiguration);
				}
				else
				{
					_scope = CreateNewManagementScopeCimv2(name, vConfig);
				}
				LOG.DebugFormat("ServerName: {0}, isVM: {1} URL: {2}, Username: {3}", name, isVM, _scope.Path, _scope.Options.Username);

                SelectQuery wmiDiskQuery = new SelectQuery("SELECT * FROM Win32_PerfRawData_PerfDisk_PhysicalDisk WHERE Name = '_Total'");
                using (ManagementObjectSearcher searcher1 = new ManagementObjectSearcher(_scope, wmiDiskQuery))
                {
                    ManagementObjectCollection diskCollection = searcher1.Get();
                    foreach (ManagementObject diskObject in diskCollection)
                    {
                        UInt32? DiskReadsPersec = GetPropertyValue<UInt32>(diskObject, "DiskReadsPersec");
                        UInt32? DiskWritesPersec = GetPropertyValue<UInt32>(diskObject, "DiskWritesPersec");
                        UInt64? Timestamp_PerfTime = GetPropertyValue<UInt64>(diskObject, "Timestamp_PerfTime");
                        UInt64? Frequency_PerfTime = GetPropertyValue<UInt64>(diskObject, "Frequency_PerfTime");
                        double? delta;

                        if ((pRefresh != null && pRefresh.Server != null && pRefresh.Server.VMConfig != null) || previousStatistics != null)
                        {
                            if (isVM)
                            {
                                double? p_TimeStamp_PerfTime_Raw = pRefresh == null ? previousStatistics.PerfStats.HyperCommonObject.Timestamp_PerfTime_Raw
                                                                                     : pRefresh.Server.VMConfig.PerfStats.HyperCommonObject.Timestamp_PerfTime_Raw;

                                double? p_DiskReadsPerSec_Raw = pRefresh == null ? previousStatistics.PerfStats.DiskRead_Raw
                                                                                    : pRefresh.Server.VMConfig.PerfStats.DiskRead_Raw;
                                double? p_DiskWritePerSec_Raw = pRefresh == null ? previousStatistics.PerfStats.DiskWrite_Raw
                                                                                    : pRefresh.Server.VMConfig.PerfStats.DiskWrite_Raw;
                                double? Timestamp_PerfTime_Delta = OSMetrics.Calculate_Timer_Delta(p_TimeStamp_PerfTime_Raw, Timestamp_PerfTime);
                                vmProperties.PerfStats.DiskRead_Raw = (long)DiskReadsPersec;
                                vmProperties.PerfStats.DiskWrite_Raw = (long)DiskWritesPersec;
                                vmProperties.PerfStats.HyperCommonObject.Timestamp_PerfTime_Raw = (long)Timestamp_PerfTime;
                                vmProperties.PerfStats.DiskRead = (long)OSMetrics.Calculate_PERF_COUNTER_COUNTER(p_DiskReadsPerSec_Raw, DiskReadsPersec,
                                                              out delta, Timestamp_PerfTime_Delta, Frequency_PerfTime);
                                vmProperties.PerfStats.DiskWrite = (long)OSMetrics.Calculate_PERF_COUNTER_COUNTER(p_DiskWritePerSec_Raw, DiskWritesPersec,
                                                              out delta, Timestamp_PerfTime_Delta, Frequency_PerfTime);
                            }
                            else
                            {

                                double? p_TimeStamp_PerfTime_Raw = pRefresh == null ? previousStatistics.ESXHost.PerfStats.HyperCommonObject.Timestamp_PerfTime_Raw
                                                                                    : pRefresh.Server.VMConfig.ESXHost.PerfStats.HyperCommonObject.Timestamp_PerfTime_Raw;

                                double? p_DiskReadsPerSec_Raw = pRefresh == null ? previousStatistics.ESXHost.PerfStats.DiskRead_Raw
                                                                                    : pRefresh.Server.VMConfig.ESXHost.PerfStats.DiskRead_Raw;
                                double? p_DiskWritePerSec_Raw = pRefresh == null ? previousStatistics.ESXHost.PerfStats.DiskWrite_Raw
                                                                                    : pRefresh.Server.VMConfig.ESXHost.PerfStats.DiskWrite_Raw;
                                double? Timestamp_PerfTime_Delta = OSMetrics.Calculate_Timer_Delta(p_TimeStamp_PerfTime_Raw, Timestamp_PerfTime);
                                vmProperties.ESXHost.PerfStats.DiskRead_Raw = (long)DiskReadsPersec;
                                vmProperties.ESXHost.PerfStats.DiskWrite_Raw = (long)DiskWritesPersec;
                                vmProperties.ESXHost.PerfStats.HyperCommonObject.Timestamp_PerfTime_Raw = (long)Timestamp_PerfTime;
                                vmProperties.ESXHost.PerfStats.DiskRead = (long)OSMetrics.Calculate_PERF_COUNTER_COUNTER(p_DiskReadsPerSec_Raw, DiskReadsPersec,
                                                              out delta, Timestamp_PerfTime_Delta, Frequency_PerfTime);
                                vmProperties.ESXHost.PerfStats.DiskWrite = (long)OSMetrics.Calculate_PERF_COUNTER_COUNTER(p_DiskWritePerSec_Raw, DiskWritesPersec,
                                                              out delta, Timestamp_PerfTime_Delta, Frequency_PerfTime);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LOG.Warn(" HyperV - There was a problem collecting disk usages data for : " + name, exception);
            }
            finally
            {
                if (isVM)
                    EventDiskVM.Set();
                else
                    EventDisk.Set();
                LOG.Info(" HyperV - Exiting Disk usages collection for : " + name);
            }

        }

        private static T? GetPropertyValue<T>(ManagementBaseObject mbo, string propertyName) where T : struct
        {
            try
            {
                var o = mbo.GetPropertyValue(propertyName);
                if (o == null) return null;
                if (o is T)
                    return (T)o;
                o = Convert.ChangeType(o, typeof(T));
                if (o == null) return null;
                return (T)o;
            }
            catch (Exception)
            {
                return null;
            }
        }
		
		private ConnectionOptions CreateConnectionOptions(string machineName, WmiConfiguration wmiConfig)
		{
		    
		    var opts = new ConnectionOptions();
		    //TODO need to use configuration WMI timeout from collection services config file
		    //opts.Timeout = CollectionServiceConfiguration.WMIQueryTimeout; 

		    if (!wmiConfig.DirectWmiConnectAsCollectionService)
		    {
		        if (!String.IsNullOrEmpty(wmiConfig.DirectWmiUserName) && !String.IsNullOrEmpty(wmiConfig.DirectWmiPassword))
		        {
		            var nameparts = wmiConfig.DirectWmiUserName.Split('\\');
		            var domain = (nameparts.Length == 1) ? machineName : nameparts[0];
		            opts.Username = (nameparts.Length > 1) ? nameparts[1] : nameparts[0];
		            opts.Password = wmiConfig.DirectWmiPassword;
		            
		            if (!string.IsNullOrEmpty(domain))
		            {
		                if (domain.Contains(":"))
		                    opts.Authority = domain;
		                else
		                    opts.Authority = "ntlmdomain:" + domain;
		            }
		        }
		    }
		    return opts;
		}
 
		private ManagementScope CreateNewManagementScopeCimv2VM(string virtualMachine, WmiConfiguration wmiConfiguration)
		{
		    string connectionUrl = string.Format(@"\\{0}\root\cimv2", virtualMachine.ToString());
		    ManagementScope scope = new ManagementScope(connectionUrl);
		    ConnectionOptions options = new ConnectionOptions();
		    if (wmiConfiguration.DirectWmiEnabled)
		    {
		        options = CreateConnectionOptions(virtualMachine, wmiConfiguration);
		        options.Impersonation = ImpersonationLevel.Impersonate;
		        options.Authentication = AuthenticationLevel.PacketPrivacy;
		    }
		    scope.Options = options;    
		    return scope;
		}
		
		
        private ManagementScope CreateNewManagementScopeCimv2(string serverAddress, VirtualizationConfiguration vConfig)
        {
            string connectionUrl = string.Format(@"\\{0}\root\cimv2", serverAddress.ToString());
            ManagementScope scope = new ManagementScope(connectionUrl);

            //Use the user credentials only if it is not a local host or local machine
            if (!HyperVServiceConnection.IsLocalHost(serverAddress.ToString()))
            {
                ConnectionOptions options = new ConnectionOptions
                {
                    Username = vConfig.VCUser,
                    Password = vConfig.VCPassword,
                    Impersonation = ImpersonationLevel.Impersonate,
                    Authentication = AuthenticationLevel.PacketPrivacy
                };
                scope.Options = options;
            }

            return scope;
        }

        public ManagementObjectSearcher CollectCustomCounterValue(string Wmiclass, string counterType, VirtualizationConfiguration vConfig, string whereClause)
        {
            string serverAddress = connection.Address;
            if (counterType.Equals("VM", StringComparison.InvariantCultureIgnoreCase))
            {
                serverAddress = vConfig.VMName;
            }
            ManagementScope _scope = CreateNewManagementScopeCimv2(serverAddress, vConfig);
            SelectQuery wmiQuery;
            if (whereClause.Equals(""))
            {
                wmiQuery = new SelectQuery(String.Format("SELECT * FROM  {0}", Wmiclass));
            }
            else
            {
                wmiQuery = new SelectQuery(String.Format("SELECT * FROM  {0} WHERE {1}", Wmiclass, whereClause));
            }


            using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(_scope, wmiQuery))
            {
                return managementObjectSearcher;
            }

        }
    }

    public class Utility
    {
        public static ManagementObject GetServiceObject(ManagementScope scope, string serviceName)
        {

            scope.Connect();
            ManagementPath wmiPath = new ManagementPath(serviceName);
            ManagementClass serviceClass = new ManagementClass(scope, wmiPath, null);
            ManagementObjectCollection services = serviceClass.GetInstances();

            ManagementObject serviceObject = null;

            foreach (ManagementObject service in services)
            {
                serviceObject = service;
            }
            return serviceObject;
        }
    }

    public static class ReturnCode
    {
        public const UInt32 Completed = 0;
        public const UInt32 Started = 4096;
        public const UInt32 Failed = 32768;
        public const UInt32 AccessDenied = 32769;
        public const UInt32 NotSupported = 32770;
        public const UInt32 Unknown = 32771;
        public const UInt32 Timeout = 32772;
        public const UInt32 InvalidParameter = 32773;
        public const UInt32 SystemInUse = 32774;
        public const UInt32 InvalidState = 32775;
        public const UInt32 IncorrectDataType = 32776;
        public const UInt32 SystemNotAvailable = 32777;
        public const UInt32 OutofMemory = 32778;
    }
    public class HyperConnectionException : System.Exception
    {
        public HyperConnectionException() { }

        public HyperConnectionException(string message) : base(message) { }

        protected HyperConnectionException(SerializationInfo info, StreamingContext context) : base(info, context) { }


    }

}
