using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using Idera.SQLdm.Common.Snapshots;
using Vim25Api;
using System.Runtime.Serialization;


namespace Idera.SQLdm.Common.VMware
{
    public class ServiceUtil
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("VmServiceUtil");
        private ServiceConnection connection;

        private string url = null;

        private NameValueCollection propList = null;
        private List<string> dmCounterList = null;

        private Hashtable counterInfoMap;
        private Hashtable counters;

        public delegate VMwareVirtualMachine VMCollectionMethod(string instanceUuid);

        #region Constructors

        public ServiceUtil(string vcURL)
        {
            URL = vcURL;
            connection = new ServiceConnection(URL);
            initPropLists();
        }

        #endregion

        #region Properties

        public ConnectState ConnectState
        {
            get { return connection.State; }
        }

        public ServiceContent ServiceContent
        {
            get { return connection.ServiceContent; }
        }

        public string URL
        {
            get { return url; }
            set { url = value; }
        }

        public VimService Service
        {
            get
            {
                if (connection.State == VMware.ConnectState.Connected)
                    return connection.Service;
                else
                    return null;
            }
        }

        public ManagedObjectReference SearchIndex
        {
            get { return ServiceContent.searchIndex; }
        }

        public ManagedObjectReference PropCollector
        {
            get { return ServiceContent.propertyCollector; }
        }

        public ManagedObjectReference RootFolder
        {
            get { return ServiceContent.rootFolder; }
        }

        public ManagedObjectReference PerfManager
        {
            get { return ServiceContent.perfManager; }
        }

        #endregion

        #region Connect/Disconnect

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
                    throw new Exception("Missing the url, user or password.  All three parameters are needed to connection");
                }
            }
            catch (Exception e)
            {
                // Log exception
                throw e;
            }
        }

        public void Connect(Cookie cookie)
        {
            try
            {
                if (url != null)
                {
                    connection.Connect(cookie);
                }
                else
                {
                    throw new Exception("Missing the url.  Need the URL of the Virtualization Host server in order to connect");
                }
            }
            catch (Exception e)
            {
                // Log Exception
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

        #endregion

        #region Initializers for Collections

        // Sets the list of properties that we're going to go after for each object type
        private void initPropLists()
        {
            if (propList == null)
                propList = new NameValueCollection();
            else
                propList.Clear();


            propList.Add("VirtualMachine", "runtime.bootTime");
            propList.Add("VirtualMachine", "runtime.powerState");
            propList.Add("VirtualMachine", "guest.hostName");
            propList.Add("VirtualMachine", "summary.config.instanceUuid");
            propList.Add("VirtualMachine", "summary.config.name");
            propList.Add("VirtualMachine", "summary.config.numCpu");
            propList.Add("VirtualMachine", "summary.config.memorySizeMB");
            propList.Add("VirtualMachine", "resourceConfig.cpuAllocation.limit");
            propList.Add("VirtualMachine", "resourceConfig.cpuAllocation.reservation");
            propList.Add("VirtualMachine", "resourceConfig.memoryAllocation.limit");
            propList.Add("VirtualMachine", "resourceConfig.memoryAllocation.reservation");
            propList.Add("HostSystem", "summary.hardware");
            propList.Add("HostSystem", "summary.config.name");
            propList.Add("HostSystem", "runtime.powerState");
            propList.Add("HostSystem", "config.network.dnsConfig.hostName");
            propList.Add("HostSystem", "config.network.dnsConfig.domainName");
            propList.Add("HostSystem", "runtime.bootTime");

        }

        private void initPerfCounterLists()
        {
            dmCounterList = new List<string>();

            dmCounterList.Add("cpu.usage.average");
            dmCounterList.Add("cpu.usagemhz.average");
            dmCounterList.Add("cpu.used.summation");
            dmCounterList.Add("cpu.ready.summation");
            dmCounterList.Add("cpu.swapwait.summation");
            dmCounterList.Add("mem.swapinRate.average");
            dmCounterList.Add("mem.swapoutRate.average");
            dmCounterList.Add("mem.swapped.average");
            dmCounterList.Add("mem.active.average");
            dmCounterList.Add("mem.consumed.average");
            dmCounterList.Add("mem.granted.average");
            dmCounterList.Add("mem.vmmemctl.average");
            dmCounterList.Add("mem.usage.average");
            dmCounterList.Add("disk.read.average");
            dmCounterList.Add("disk.write.average");
            dmCounterList.Add("disk.usage.average");
            //dmCounterList.Add("disk.deviceLatency.average");
            //dmCounterList.Add("disk.kernelLatency.average");
            //dmCounterList.Add("disk.queueLatency.average");
            //dmCounterList.Add("disk.totalLatency.average");
            dmCounterList.Add("net.usage.average");
            dmCounterList.Add("net.transmitted.average");
            dmCounterList.Add("net.received.average");
        }

        private void initPerfCounters()
        {
            initPerfCounterLists();
            counterInfoMap = new Hashtable();
            counters = new Hashtable();

            PropertyFilterSpec[] pfSpecArray = buildPropertyFilterSpecArray(PerfManager, new string[] { "perfCounter" }, null);

            ObjectContent[] objContentArray = Service.RetrieveProperties(PropCollector, pfSpecArray);

            if (objContentArray != null)
            {
                foreach (ObjectContent oc in objContentArray)
                {
                    if (oc.propSet != null)
                    {
                        foreach (DynamicProperty dp in oc.propSet)
                        {
                            PerfCounterInfo[] pciArray = (PerfCounterInfo[])dp.val;
                            foreach (PerfCounterInfo pci in pciArray)
                            {
                                string fullCounter = pci.groupInfo.key.Trim() + "." + pci.nameInfo.key.Trim() + "." + pci.rollupType.ToString();
                                if (dmCounterList.Contains(fullCounter))
                                {
                                    counterInfoMap.Add(pci.key, pci);
                                    counters.Add(fullCounter, pci.key);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Load all the perf counter info objects into a dictionary
        /// </summary>
        /// <param name="serviceUtil"></param>
        /// <returns></returns>
        internal Dictionary<int, PerfCounterInfo> GetPerfCounterInfo()
        {
            Dictionary<int, PerfCounterInfo> result = new Dictionary<int, PerfCounterInfo>(500);

            PropertyFilterSpec[] pfSpecArray = buildPropertyFilterSpecArray(PerfManager, new string[] { "perfCounter" }, null);
            ObjectContent[] objContentArray = Service.RetrieveProperties(PropCollector, pfSpecArray);
            if (objContentArray != null)
            {
                foreach (ObjectContent oc in objContentArray)
                {
                    if (oc.propSet != null)
                    {
                        foreach (DynamicProperty dp in oc.propSet)
                        {
                            PerfCounterInfo[] pciArray = (PerfCounterInfo[])dp.val;
                            foreach (PerfCounterInfo pci in pciArray)
                            {
                                if (pci.rollupType == PerfSummaryType.average)
                                if (!result.ContainsKey(pci.key))
                                {
                                    result.Add(pci.key, pci);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        public Dictionary<string, basicVMInfo> GetListOfVMs(Dictionary<string, string> filter)
        {
            if (connection.State != VMware.ConnectState.Connected)
                throw new vCenterConnectionException("Not connected to Virtualization Host Server");

            // Define the properties to pull from the VMs
            List<string> vmPropList = new List<string>();
            vmPropList.Add("name");
            vmPropList.Add("config.template");
            vmPropList.Add("guest.hostName");
            vmPropList.Add("config.instanceUuid");
            vmPropList.AddRange(filter.Keys);

            PropertyFilterSpec[] pfSpecArray = buildPropertyFilterSpecArray(RootFolder, vmPropList.ToArray(), "VirtualMachine");

            ObjectContent[] listOfVMs = Service.RetrieveProperties(PropCollector, pfSpecArray);

            Dictionary<string, basicVMInfo> result = new Dictionary<string, basicVMInfo>();

            string vmName, hostName, uuid;
            bool matchFilter, template;

            foreach (ObjectContent oc in listOfVMs)
            {
                if (oc.propSet != null)
                {
                    vmName = hostName = uuid = "";
                    template = false;
                    matchFilter = true;
                    foreach (DynamicProperty pc in oc.propSet)
                    {
                        switch (pc.name)
                        {
                            case "name":
                                vmName = Uri.UnescapeDataString((string)pc.val);
                                break;
                            case "guest.hostName":
                                hostName = (string)pc.val;
                                break;
                            case "config.instanceUuid":
                                uuid = (string)pc.val;
                                break;
                            case "config.template":
                                template = (bool)pc.val;
                                break;
                            default:
                                if ((filter != null) && (filter.Count > 0))
                                {
                                    if (!pc.val.ToString().ToLower().Contains(filter[pc.name].ToLower()))
                                        matchFilter = false;
                                }
                                break;
                        }
                    }
                    if (!template && matchFilter)
                    {
                        if (!result.ContainsKey(uuid))
                        {
                            result.Add(uuid, new basicVMInfo(URL, vmName, hostName, uuid));
                        }
                        else
                        {
                            LOG.WarnFormat("VM Warning: VM [{0}] has the same UUID as VM [{1}].  VM [{0}] will be dropped.", vmName, result[uuid].VMName);
                        }

                    }
                }
            }

            return result;
        }

        public VMwareVirtualMachine CollectVMInfo(string instanceUuid, int iterations)
        {
            ManagedObjectReference vmMoRef = getManagedObject(instanceUuid, true, VIMSearchType.UUID);
            LOG.DebugFormat("VM information.- instanceUuid: {0}. iterations: {1}", instanceUuid, iterations.ToString());

            ManagedObjectReference esxMoRef = getESXHost(vmMoRef);

            VMwareVirtualMachine vmData = getConfigInfo(vmMoRef, esxMoRef, iterations);

            return vmData;
        }

        private VMwareVirtualMachine getConfigInfo(ManagedObjectReference vmMoRef, ManagedObjectReference esxMoRef, int iterations)
        {
            using (LOG.DebugCall("getConfigInfo")) 
            {

                VMwareVirtualMachine vmProperties = new VMwareVirtualMachine();

                try
                {
                    ObjectContent[] vmContent = getProperties(vmMoRef, propList.GetValues(vmMoRef.type));
                    ObjectContent[] esxContent = getProperties(esxMoRef, propList.GetValues(esxMoRef.type));

                    if (vmContent != null && vmContent[0].propSet != null)
                    {
                        LOG.DebugFormat("VM Properties Return: [{0}]", vmContent[0].propSet.Length.ToString());

                        foreach (DynamicProperty dp in vmContent[0].propSet)
                        {
                            LOG.VerboseFormat("VM.{0}: [{1}]", dp.name, dp.val.ToString());
                            switch (dp.name)
                            {
                                case "runtime.bootTime":
                                    vmProperties.BootTime = (DateTime) dp.val;
                                    LOG.DebugFormat("runtime.bootTime: [{0}]", vmProperties.BootTime.ToString());
                                    break;
                                case "runtime.powerState":
                                    vmProperties.Status = (VirtualMachinePowerState) dp.val;
                                    LOG.DebugFormat("runtime.powerState: [{0}]", vmProperties.Status.ToString());
                                    break;
                                case "guest.hostName":
                                    vmProperties.HostName = (string) dp.val;
                                    LOG.DebugFormat("guest.hostName: [{0}]", vmProperties.HostName);
                                    break;
                                case "summary.config.instanceUuid":
                                    vmProperties.InstanceUUID = (string) dp.val;
                                    LOG.DebugFormat("summary.config.instanceUuid: [{0}]", vmProperties.InstanceUUID);
                                    break;
                                case "summary.config.name":
                                    vmProperties.Name = (string) dp.val;
                                    LOG.DebugFormat("summary.config.name: [{0}]", vmProperties.Name);
                                    break;
                                case "summary.config.numCpu":
                                    vmProperties.NumCPUs = (int) dp.val;
                                    LOG.DebugFormat("summary.config.numCpu: [{0}]", vmProperties.NumCPUs.ToString());
                                    break;
                                case "summary.config.memorySizeMB":
                                    //vmProperties.MemSize = new FileSize();
                                    vmProperties.MemSize.Megabytes = (int) dp.val;
                                    LOG.DebugFormat("summary.config.memorySizeMB: [{0}]",
                                                    vmProperties.MemSize.Megabytes.ToString());
                                    break;
                                case "resourceConfig.cpuAllocation.limit":
                                    vmProperties.CPULimit = (long) dp.val;
                                    LOG.DebugFormat("resourceConfig.cpuAllocation.limit: [{0}]",
                                                    vmProperties.CPULimit.ToString());
                                    break;
                                case "resourceConfig.cpuAllocation.reservation":
                                    vmProperties.CPUReserve = (long) dp.val;
                                    LOG.DebugFormat("resourceConfig.cpuAllocation.reservation: [{0}]",
                                                    vmProperties.CPUReserve.ToString());
                                    break;
                                case "resourceConfig.memoryAllocation.limit":
                                    ///vmProperties.MemLimit = new FileSize();
                                    vmProperties.MemLimit.Megabytes = (long) dp.val;
                                    LOG.DebugFormat("resourceConfig.memoryAllocation.limit: [{0}]",
                                                    vmProperties.MemLimit.Megabytes.ToString());
                                    break;
                                case "resourceConfig.memoryAllocation.reservation":
                                    //vmProperties.MemReserve = new FileSize();
                                    vmProperties.MemReserve.Megabytes = (long) dp.val;
                                    LOG.DebugFormat("resourceConfig.memoryAllocation.reservation: [{0}]",
                                                   vmProperties.MemReserve.Megabytes.ToString());
                                    break;
                            }
                        }
                    }

                    string machineName = "";
                    string domainName = "";

                    if (esxContent != null && esxContent[0].propSet != null)
                    {
                        LOG.DebugFormat("ESX Properties Return: [{0}]", esxContent[0].propSet.Length.ToString());

                        foreach (DynamicProperty dp in esxContent[0].propSet)
                        {
                            LOG.VerboseFormat("ESX.{0}: [{1}]", dp.name, dp.val.ToString());
                            switch (dp.name)
                            {
                                case "summary.hardware":
                                    vmProperties.ESXHost.HardwareSummary = (HostHardwareSummary) dp.val;
                                    LOG.DebugFormat("summary.hardware: [{0}]", dp.val.ToString());
                                    break;
                                case "summary.config.name":
                                    vmProperties.ESXHost.Name = (string) dp.val;
                                    LOG.DebugFormat("summary.config.name: [{0}]", vmProperties.ESXHost.Name);
                                    break;
                                case "runtime.powerState":
                                    vmProperties.ESXHost.Status = (HostSystemPowerState) dp.val;
                                    LOG.DebugFormat("runtime.powerState: [{0}]", vmProperties.ESXHost.Status.ToString());
                                    break;
                                case "config.network.dnsConfig.hostName":
                                    machineName = (string) dp.val;
                                    LOG.DebugFormat("config.network.dnsConfig.hostName: [{0}]", machineName);
                                    break;
                                case "config.network.dnsConfig.domainName":
                                    domainName = (string) dp.val;
                                    LOG.DebugFormat("config.network.dnsConfig.domainName: [{0}]", domainName);
                                    break;
                                case "runtime.bootTime":
                                    vmProperties.ESXHost.BootTime = (DateTime) dp.val;
                                    LOG.DebugFormat("runtime.bootTime: [{0}]", vmProperties.ESXHost.BootTime.ToString());
                                    break;
                            }
                        }
                        vmProperties.ESXHost.DomainName = machineName.Trim() + "." + domainName.Trim();
                        LOG.DebugFormat("ESXHost.DomainName: [{0}]", vmProperties.ESXHost.DomainName);
                    }

                    getPerfStats(vmProperties, vmMoRef, esxMoRef, iterations);
                }
                catch (Exception e)
                {
                    LOG.Warn("There was a problem collecting VM Data. ", e);
                    //we should not make vmProperties as null because it contains the VM config details also.
                    //vmProperties = null;
                }
                return vmProperties;
            }
        }

        internal PerfEntityMetricBase[] getPerfStats(ManagedObjectReference moRef, PerfMetricId metric, int iterations)
        {
            PerfQuerySpec querySpec = buildPerfQuerySpec(moRef, new PerfMetricId[] { metric }, 1);

            return Service.QueryPerf(PerfManager, new PerfQuerySpec[] { querySpec });

        }

        private void getPerfStats(VMwareVirtualMachine vmInfo, ManagedObjectReference vmMoRef, ManagedObjectReference esxMoRef, int iterations)
        {
            
            initPerfCounters();
            PerfQuerySpec[] pqSpecArray = buildPerfQuerySpecArray(vmMoRef, esxMoRef, iterations);

            PerfEntityMetricBase[] perfStats = Service.QueryPerf(PerfManager, pqSpecArray);

            foreach (PerfEntityMetricBase stat in perfStats)
            {
                switch (stat.entity.type)
                {
                    case "VirtualMachine":
                        try
                        {
                            parsePerfStats(vmInfo.PerfStats, stat);
                        }
                        catch (Exception)
                        {
                            LOG.Debug("Error Collecting VM Perf Stats");
                            throw;
                        }
                        break;
                    case "HostSystem":
                        try
                        {
                            parsePerfStats(vmInfo.ESXHost.PerfStats, stat);
                        }
                        catch (Exception)
                        {
                            LOG.Debug("Error collecting Host Perf Stats");
                            throw;
                        }
                        break;
                }
            }
        }

        private void parsePerfStats(PerfStatistics perfStats, PerfEntityMetricBase stat)
        {
            PerfMetricSeries[] vals = ((PerfEntityMetric)stat).value;

            LOG.DebugFormat("Number of {0} perfstats [{1}]", stat.entity.type, vals.Length.ToString() );
            foreach (PerfMetricSeries pms in vals)
            {
                long value = -999;
                PerfCounterInfo pci = (PerfCounterInfo)counterInfoMap[pms.id.counterId];
                string fullCounter = pci.groupInfo.key.Trim() + "." + pci.nameInfo.key.Trim() + "." + pci.rollupType.ToString();
                PerfMetricId pmi = pms.id;
                if (string.IsNullOrEmpty(pmi.instance))
                {
                    var val = pms as PerfMetricIntSeries;
                    if (val != null)
                    {
                        var longVals = val.value;
                        value = calcAverage(longVals);
                    }

                    if (value < 0)
                    {
                        Exception except = new Exception("Negative values received from Virtualization Host");
                        throw except;
                    }

                    LOG.VerboseFormat("{0} : [{1}]", fullCounter, value.ToString());
                    switch (fullCounter)
                    {
                        case "cpu.usage.average":
                            perfStats.CpuUsage = (value / 100.0);
                            break;
                        case "cpu.usagemhz.average":
                            perfStats.CpuUsageMHz = value;
                            break;
                        case "cpu.ready.summation":
                            perfStats.CpuReady = value;
                            break;
                        case "cpu.swapwait.summation":
                            perfStats.CpuSwapWait = value;
                            break;
                        case "mem.swapinRate.average":
                            perfStats.MemSwapInRate = value;
                            break;
                        case "mem.swapoutRate.average":
                            perfStats.MemSwapOutRate = value;
                            break;
                        case "mem.swapped.average":
                            //perfStats.MemSwapped = new FileSize(value);
                            perfStats.MemSwapped.Kilobytes = value;
                            break;
                        case "mem.active.average":
                            //perfStats.MemActive = new FileSize(value);
                            perfStats.MemActive.Kilobytes = value;
                            break;
                        case "mem.consumed.average":
                            //perfStats.MemConsumed = new FileSize(value);
                            perfStats.MemConsumed.Kilobytes = value;
                            break;
                        case "mem.granted.average":
                            //perfStats.MemGranted = new FileSize(value);
                            perfStats.MemGranted.Kilobytes = value;
                            break;
                        case "mem.vmmemctl.average":
                            //perfStats.MemBallooned = new FileSize(value);
                            perfStats.MemBallooned.Kilobytes = value;
                            break;
                        case "mem.usage.average":
                            perfStats.MemUsage = (value / 100.0);
                            break;
                        case "disk.read.average":
                            perfStats.DiskRead = value;
                            break;
                        case "disk.write.average":
                            perfStats.DiskWrite = value;
                            break;
                        //case "disk.deviceLatency.average":
                        //    perfStats.DiskDeviceLatency = value;
                        //    break;
                        //case "disk.kernelLatency.average":
                        //    perfStats.DiskKernelLatency = value;
                        //    break;
                        //case "disk.queueLatency.average":
                        //    perfStats.DiskQueueLatency = value;
                        //    break;
                        //case "disk.totalLatency.average":
                        //    perfStats.DiskTotalLatency = value;
                        //    break;
                        case "disk.usage.average":
                            perfStats.DiskUsage = value;
                            break;
                        case "net.usage.average":
                            perfStats.NetUsage = value;
                            break;
                        case "net.transmitted.average":
                            perfStats.NetTransmitted = value;
                            break;
                        case "net.received.average":
                            perfStats.NetReceived = value;
                            break;
                    }
                }
            }
        }

        #region Helper Methods

        private ObjectContent[] getProperties(ManagedObjectReference moRef, string[] properties)
        {
            using (LOG.DebugCall("getProperties"))
            {
                if (connection.State != VMware.ConnectState.Connected)
                {
                    throw new vCenterConnectionException("Not connected to Virtualization Host Server");
                }

                PropertyFilterSpec[] pfSpecArray = buildPropertyFilterSpecArray(moRef, properties, null);

                ObjectContent[] results = Service.RetrieveProperties(PropCollector, pfSpecArray);

                return results;
            }
        }

        public ManagedObjectReference getManagedObject(string searchParm, Boolean vm, VIMSearchType searchType)
        {
            Boolean isVM = true;
            VIMSearchType searchBy = VIMSearchType.UUID;
            ManagedObjectReference retVal = null;

            if (connection.State != VMware.ConnectState.Connected)
                throw new vCenterConnectionException("Not Connected to Virtualization Host Server");
            if (searchParm == null)
                throw new ArgumentNullException("searchParm");
            if (vm != null)
                isVM = vm;
            if (searchType != null)
                searchBy = searchType;

            switch (searchBy)
            {
                case VIMSearchType.DatastorePath:
                    retVal = Service.FindByDatastorePath(SearchIndex, null, searchParm);
                    break;
                case VIMSearchType.DNSName:
                    retVal = Service.FindByDnsName(SearchIndex, null, searchParm, isVM);
                    break;
                case VIMSearchType.InventoryPath:
                    retVal = Service.FindByInventoryPath(SearchIndex, searchParm);
                    break;
                case VIMSearchType.IP:
                    retVal = Service.FindByIp(SearchIndex, null, searchParm, isVM);
                    break;
                case VIMSearchType.UUID:
                    retVal = Service.FindByUuid(SearchIndex, null, searchParm, isVM, isVM, isVM);
                    break;
            }

            return retVal;
        }

        internal ManagedObjectReference getESXHost(ManagedObjectReference vmMoRef)
        {
            ManagedObjectReference esxHost = null;

            ObjectContent[] vmProps = getProperties(vmMoRef, new string[] { "runtime.host", });

            if ((vmProps != null) && (vmProps.Length > 0))
            {
                if (vmProps[0].propSet.Length > 0)
                {
                    esxHost = (ManagedObjectReference)vmProps[0].propSet[0].val;
                }
            }

            return esxHost;
        }

        internal static long calcAverage(long[] input)
        {
            long sum = 0;
            foreach (long num in input)
            {
                sum += num;
            }

            return sum / input.LongLength;
        }

        #endregion

        #region PropertyFilterSpecs

        private PropertyFilterSpec[] buildPropertyFilterSpecArray(ManagedObjectReference moRef, string[] properties, string objectType)
        {
            using (LOG.DebugCall("buildPropertyFilterSpecArray"))
            {
                PropertySpec pSpec = buildPropSpec(objectType == null ? moRef.type : objectType, properties);
                PropertySpec[] pSpecArray = new PropertySpec[] { pSpec };

                ObjectSpec oSpec = buildObjSpec(moRef);
                if (objectType != null)
                {
                    oSpec.selectSet = BuildVMTraversal();
                    LOG.DebugFormat("objectType is not null and its value is: {0}", objectType);
                }
                ObjectSpec[] oSpecArray = new ObjectSpec[] { oSpec };

                PropertyFilterSpec pfSpec = buildPropFilterSpec(oSpecArray, pSpecArray);

                return new PropertyFilterSpec[] { pfSpec };
            }
        }

        private PropertySpec buildPropSpec(string pSpecType, string[] properties)
        {
            PropertySpec pSpec = new PropertySpec();
            pSpec.type = pSpecType;
            pSpec.all = properties.Length == 0 ? true : false;
            pSpec.pathSet = properties;

            return pSpec;
        }

        private ObjectSpec buildObjSpec(ManagedObjectReference moRef)
        {
            ObjectSpec oSpec = new ObjectSpec();
            oSpec.obj = moRef;
            return oSpec;
        }

        private PropertyFilterSpec buildPropFilterSpec(ObjectSpec[] oSpecArray, PropertySpec[] pSpecArray)
        {
            using (LOG.DebugCall("buildPropFilterSpec"))
            {
                PropertyFilterSpec pfSpec = new PropertyFilterSpec();
                pfSpec.objectSet = oSpecArray;
                pfSpec.propSet = pSpecArray;

                return pfSpec;
            }
        }

        public SelectionSpec[] BuildVMTraversal()
        {
            using (LOG.DebugCall("BuildVMTraversal"))
            {
                //Traverse vm Folder branch
                var dcToVmf = new TraversalSpec();
                dcToVmf.name = "dcToVmf";
                dcToVmf.type = "Datacenter";
                dcToVmf.path = "vmFolder";
                dcToVmf.skip = false;
                dcToVmf.skipSpecified = true;
                dcToVmf.selectSet = new SelectionSpec[] { new SelectionSpec() };
                dcToVmf.selectSet[0].name = "visitFolders";

                // traverse through VirtualApp.VM
                var visitVAppVM = new TraversalSpec();
                visitVAppVM.name = "visitVAppVMs";
                visitVAppVM.type = "VirtualApp";
                visitVAppVM.path = "vm";

                // traverse through VirtualApp.resourcePool
                var visitVApps = new TraversalSpec();
                visitVApps.name = "visitVApps";
                visitVApps.type = "VirtualApp";
                visitVApps.path = "resourcePool";
                visitVApps.selectSet = new[] { new SelectionSpec(), new SelectionSpec() };
                visitVApps.selectSet[0].name = "visitVApps";
                visitVApps.selectSet[1].name = "visitVAppVMs";

                // Traverse through the folders
                var visitFolders = new TraversalSpec();
                visitFolders.name = "visitFolders";
                visitFolders.type = "Folder";
                visitFolders.path = "childEntity";
                visitFolders.skip = false;
                visitFolders.skipSpecified = true;
                visitFolders.selectSet = new SelectionSpec[] { new SelectionSpec(), new SelectionSpec(), new SelectionSpec(), new SelectionSpec() };
                visitFolders.selectSet[0].name = "visitFolders";
                visitFolders.selectSet[1].name = "dcToVmf";
                visitFolders.selectSet[2].name = "visitVApps";
                visitFolders.selectSet[3].name = "visitVAppVMs";
                visitFolders.skip = false;
                visitFolders.skipSpecified = true;

                return new SelectionSpec[] { visitFolders, dcToVmf, visitVApps, visitVAppVM };
            }
        }

        #endregion

        #region PerfQuerySpecs
        private PerfQuerySpec[] buildPerfQuerySpecArray(ManagedObjectReference vmMoRef, ManagedObjectReference esxMoRef, int iterations)
        {
            PerfQuerySpec vmQuerySpec = buildPerfQuerySpec(vmMoRef, getPerfMetricIDs(vmMoRef), iterations);
            PerfQuerySpec esxQuerySpec = buildPerfQuerySpec(esxMoRef, getPerfMetricIDs(esxMoRef), iterations);

            return new PerfQuerySpec[] { vmQuerySpec, esxQuerySpec };
        }

        internal PerfQuerySpec buildPerfQuerySpec(ManagedObjectReference moRef, PerfMetricId[] pmIDs, int iterations)
        {
            PerfQuerySpec pqSpec = new PerfQuerySpec();
            pqSpec.entity = moRef;
            pqSpec.maxSample = iterations < 1 ? 1 : iterations;

            pqSpec.maxSampleSpecified = true;
            pqSpec.metricId = pmIDs;
            pqSpec.intervalId = 20;
            pqSpec.intervalIdSpecified = true;

            return pqSpec;
        }

        internal PerfMetricId[] getPerfMetricIDs(ManagedObjectReference moRef)
        {
            return getPerfMetricIDs(moRef, counterInfoMap);
        }

        internal PerfMetricId[] getPerfMetricIDs(ManagedObjectReference moRef, IDictionary includeFilter)
        {
            return getPerfMetricIDs(moRef, includeFilter, null);
        }

        internal PerfMetricId[] getPerfMetricIDs(ManagedObjectReference moRef, IDictionary includeFilter, Comparison<PerfMetricId> sortComparison)
        {
            List<PerfMetricId> retVal = new List<PerfMetricId>();
            PerfMetricId[] pmIDs = Service.QueryAvailablePerfMetric(PerfManager, moRef, DateTime.MinValue, false, DateTime.MaxValue, false, 20, true);

            for (int i = 0; i < pmIDs.Length; i++)
            {
                if (includeFilter.Contains(pmIDs[i].counterId))
                    retVal.Add(pmIDs[i]);
            }
            
            if (sortComparison != null)
                retVal.Sort(sortComparison);

            return retVal.ToArray();
        }

        internal PerfMetricId[] getAllPerfMetricIDs(ManagedObjectReference moRef)
        {
            return Service.QueryAvailablePerfMetric(PerfManager, moRef, DateTime.MinValue, false, DateTime.MaxValue, false, 20, true);
        }

        #endregion
    }

    [Serializable]
    public class vCenterConnectionException : System.Exception
    {
        public vCenterConnectionException() { }

        public vCenterConnectionException(string message) : base(message) { }

        protected vCenterConnectionException(SerializationInfo info, StreamingContext context) : base(info, context) { }


    }
}
