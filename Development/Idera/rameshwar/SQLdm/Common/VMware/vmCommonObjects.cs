using System;
using System.Data;
using System.Data.SqlTypes;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Security.Encryption;
using Idera.SQLdm.Common.Snapshots;
using Vim25Api;
using Idera.SQLdm.Common.HyperV;

namespace Idera.SQLdm.Common.VMware
{
    class vmCommonObjects
    {
    }

    public enum ConnectState
    {
        Connected,
        Disconnected
    }

    public enum VIMSearchType
    {
        DatastorePath,
        DNSName,
        InventoryPath,
        IP,
        UUID
    }

    [Serializable]
    public class vCenterHosts:IAuditable
    {
        private const string CipherInstance = "Idera.SQLdm.Common.vmWare";

        private int hostID = -1;
        private string name;
        private string address;
        private string username;
        private string encryptedPassword;
        private string serverType;

        #region Constructors

        public vCenterHosts()
        {
        }

        public vCenterHosts(int pHostID, string pName, string pAddress, string pUsername, string pEncryptedPassword, string pServerType)
        {
            hostID = pHostID;
            name = pName;
            address = pAddress;
            username = pUsername;
            encryptedPassword = pEncryptedPassword;
            serverType = pServerType;
        }


        #endregion

        #region Properties

        [AuditableAttribute(false)] 
        public int vcHostID
        {
            get { return hostID; }
            set { hostID = value; }
        }

        [AuditableAttribute("Virtualization Host Name")] 
        public string vcName
        {
            get { return name; }
            set { name = value; }
        }

        [AuditableAttribute("Virtualization Host Address")] 
        public string vcAddress
        {
            get { return address; }
            set { address = value; }
        }

        [AuditableAttribute("Virtualization Host User")] 
        public string vcUser
        {
            get { return username; }
            set { username = value; }
        }

        [AuditableAttribute("Virtualization Host Password", true)] 
        public string vcPassword
        {
            get { return Cipher.DecryptPassword(CipherInstance, encryptedPassword); }
            set { encryptedPassword = Cipher.EncryptPassword(CipherInstance, value); }
        }

        [AuditableAttribute(false)]
        public string vcEncryptedPassword
        {
            get { return encryptedPassword; }
            set { encryptedPassword = value; }
        }

        [AuditableAttribute("Virtualization Type")]
        public string ServerType
        {
            get { return serverType; }
            set { serverType = value; }
        }



        #endregion

        /// <summary>
        /// Return a new AuditableEntity with Name and vcAddres, vcUser in the metadata
        /// </summary>
        /// <returns>Return a new AuditableEntity</returns>
        public AuditableEntity GetAuditableEntity()
        {
            AuditableEntity entity=new AuditableEntity();            
            entity.Name = vcName;
            entity.AddMetadataProperty("Virtualization Host Address", vcAddress);
            entity.AddMetadataProperty("Virtualization Host User", vcUser);

            return entity;
        }

        /// <summary>
        /// Creates a new AuditableEntity with the list of values changed, compare the current object and get the diferences
        /// </summary>
        /// <param name="oldValue">Old Value to compare wiht this object</param>
        /// <returns>Return a new AuditableEntity with the list of values changed</returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            AuditableEntity entityResult = new AuditableEntity();
            entityResult.Name = this.vcName;

            PropertiesComparer comparer = new PropertiesComparer();
            var propertiesChanged = comparer.GetNewProperties(oldValue, this);
            foreach (var property in propertiesChanged)
            {
                entityResult.AddMetadataProperty(property.Name, property.Value);
            }

            return entityResult;
        }
    }

    public class basicVMInfo
    {
        private string vcAddress;
        private string vmName;
        private string domainName;
        private string instanceUUID;

        public basicVMInfo(string address, string name, string fqdm, string UUID)
        {
            HostAddress = address;
            VMName = name;
            DomainName = fqdm;
            InstanceUUID = UUID;
        }

        public string HostAddress
        {
            get { return vcAddress; }
            set { vcAddress = value; }
        }

        public string VMName
        {
            get { return vmName; }
            set { vmName = value; }
        }

        public string DomainName
        {
            get { return domainName; }
            set { domainName = value; }
        }

        public string MachineName
        {
            get
            {
                if (domainName.Contains("."))
                {
                    string[] parsedName;
                    parsedName = domainName.Split(new char[] { '.' });
                    return parsedName[0];
                }
                else
                    return domainName;
            }
        }

        public string InstanceUUID
        {
            get { return instanceUUID; }
            set { instanceUUID = value; }
        }
    }

    public class VmConfigInfo
    {
        private vCenterHosts vcInfo;
        private string instanceUUID;

        public VmConfigInfo(string uuid, vCenterHosts vc)
        {
            instanceUUID = uuid;
            vcInfo = vc;
        }

        public VmConfigInfo(string uuid, int hostid, string name, string vcurl, string user, string password)
        {
            instanceUUID = uuid;
            vcInfo = new vCenterHosts(hostid, name, vcurl, user, password, null);
        }

        public string InstanceUUID
        {
            get { return instanceUUID; }
        }

        public string VCAddress
        {
            get { return vcInfo.vcAddress; }
        }

        public string VCUser
        {
            get { return vcInfo.vcUser; }
        }

        public string VCPassword
        {
            get { return vcInfo.vcPassword; }
        }
    }

    [Serializable]
    public class ESXHostServer
    {
        private string name;
        private string domainName;
        private HostSystemPowerState status;             // runtime.powerState
        private DateTime boottime;
        private FileSize memSize;
        private HostHardwareSummary hardwareSummary = null;
        private PerfStatistics esxPerfStats;

        #region Constructors

        public ESXHostServer()
        {
            this.status = HostSystemPowerState.unknown;
            this.boottime = (DateTime) SqlDateTime.MinValue;
            this.memSize = new FileSize();
            this.esxPerfStats = new PerfStatistics();
        }

        #endregion

        #region Properties

        public HostHardwareSummary HardwareSummary
        {
            set 
            { 
                hardwareSummary = value;
                memSize.Bytes = hardwareSummary.memorySize;
            }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string DomainName
        {
            get { return domainName; }
            set { domainName = value; }
        }

        public HostSystemPowerState Status
        {
            get { return status; }
            set { status = value; }
        }

        public DateTime BootTime
        {
            get { return boottime; }
            set { boottime = value; }
        }

        public int CPUMHz
        {
            get { return hardwareSummary == null ? -1 : hardwareSummary.cpuMhz; }
            set
            {
                if (hardwareSummary == null)
                    hardwareSummary = new HostHardwareSummary();

                hardwareSummary.cpuMhz = value;
            }
        }

        public short NumCPUCores
        {
            get { return hardwareSummary == null ? (short)-1 : hardwareSummary.numCpuCores; }
            set
            {
                if (hardwareSummary == null)
                    hardwareSummary = new HostHardwareSummary();

                hardwareSummary.numCpuCores = value;
            }
        }

        public short NumCPUPkgs
        {
            get { return hardwareSummary == null ? (short)-1 : hardwareSummary.numCpuPkgs; }
            set
            {
                if (hardwareSummary == null)
                    hardwareSummary = new HostHardwareSummary();

                hardwareSummary.numCpuPkgs = value;
            }
        }

        public short NumCPUThreads
        {
            get { return hardwareSummary == null ? (short)-1 : hardwareSummary.numCpuThreads; }
            set
            {
                if (hardwareSummary == null)
                    hardwareSummary = new HostHardwareSummary();

                hardwareSummary.numCpuThreads = value;
            }
        }

        public int NumNICs
        {
            get { return hardwareSummary == null ? -1 : hardwareSummary.numNics; }
            set
            {
                if (hardwareSummary == null)
                    hardwareSummary = new HostHardwareSummary();

                hardwareSummary.numNics = value;
            }
        }

        public FileSize MemSize
        {
            get { return memSize; }
            set { memSize = value; }
        }

        public PerfStatistics PerfStats
        {
            set { esxPerfStats = value; }
            get { return esxPerfStats; }
        }

        #endregion
    }

    [Serializable]
    public class VMwareVirtualMachine
    {
        private string instanceUUID;
        private string name;
        private VirtualMachinePowerState status;
        private string hostname;
        private DateTime boottime = (DateTime)SqlDateTime.MinValue;
        private int numCPUs;
        private long cpuLimit;
        private long cpuReserve;
        private FileSize memSize = new FileSize();
        private FileSize memLimit = new FileSize();
        private FileSize memReserve = new FileSize();
        private ESXHostServer esxHost;
        private PerfStatistics vmPerfStats;

        #region Constructors

        public VMwareVirtualMachine()
        {
            this.status = VirtualMachinePowerState.poweredOff;
            esxHost = new ESXHostServer();
            vmPerfStats = new PerfStatistics();
        }

        public VMwareVirtualMachine(DataRow vmData)
        {
            if (vmData == null)
            {
                throw new ArgumentNullException("vmData");
            }

            esxHost = new ESXHostServer();
            vmPerfStats = new PerfStatistics();

            object value = vmData["vmUUID"];
            if (value != DBNull.Value)
                InstanceUUID = (string)value;

            value = vmData["vmHeartBeat"];
            if (value != DBNull.Value)
                Status = (VirtualMachinePowerState)value;

            value = vmData["vmName"];
            if (value != DBNull.Value)
                Name = (string)value;

            value = vmData["vmBootTime"];
            if (value != DBNull.Value)
                BootTime = (DateTime)value;

            value = vmData["vmCPULimit"];
            if (value != DBNull.Value)
                CPULimit = (long)value;

            value = vmData["vmCPUReserve"];
            if (value != DBNull.Value)
                CPUReserve = (long)value;

            value = vmData["vmDomainName"];
            if (value != DBNull.Value)
                HostName = (string)value;

            value = vmData["vmMemLimit"];
            if (value != DBNull.Value)
                MemLimit.Kilobytes = (long)value;

            value = vmData["vmMemReserve"];
            if (value != DBNull.Value)
                MemReserve.Kilobytes = (long)value;

            value = vmData["vmMemSize"];
            if (value != DBNull.Value)
                MemSize.Kilobytes = (long)value;

            value = vmData["vmNumCPUs"];
            if (value != DBNull.Value)
                NumCPUs = (int)value;

            value = vmData["vmCPUReady"];
            if (value != DBNull.Value)
                PerfStats.CpuReady = (long)value;

            value = vmData["vmCPUSwapWait"];
            if (value != DBNull.Value)
                PerfStats.CpuSwapWait = (long)value;

            value = vmData["vmCPUUsage"];
            if (value != DBNull.Value)
                PerfStats.CpuUsage = (double)value;

            value = vmData["vmCPUUsageMHz"];
            if (value != DBNull.Value)
                PerfStats.CpuUsageMHz = (int)value;

            value = vmData["vmDiskRead"];
            if (value != DBNull.Value)
                PerfStats.DiskRead = (long)value;

            value = vmData["vmDiskUsage"];
            if (value != DBNull.Value)
                PerfStats.DiskUsage = (long)value;

            value = vmData["vmDiskWrite"];
            if (value != DBNull.Value)
                PerfStats.DiskWrite = (long)value;

            value = vmData["vmMemActive"];
            if (value != DBNull.Value)
                PerfStats.MemActive.Kilobytes = (long)value;

            value = vmData["vmMemBallooned"];
            if (value != DBNull.Value)
                PerfStats.MemBallooned.Kilobytes = (long)value;

            value = vmData["vmMemConsumed"];
            if (value != DBNull.Value)
                PerfStats.MemConsumed.Kilobytes = (long)value;

            value = vmData["vmMemGranted"];
            if (value != DBNull.Value)
                PerfStats.MemGranted.Kilobytes = (long)value;

            value = vmData["vmMemSwapInRate"];
            if (value != DBNull.Value)
                PerfStats.MemSwapInRate = (long)value;

            value = vmData["vmMemSwapOutRate"];
            if (value != DBNull.Value)
                PerfStats.MemSwapOutRate = (long)value;

            value = vmData["vmMemSwapped"];
            if (value != DBNull.Value)
                PerfStats.MemSwapped.Kilobytes = (long)value;

            value = vmData["vmMemUsage"];
            if (value != DBNull.Value)
                PerfStats.MemUsage = (double)value;

            value = vmData["vmNetReceived"];
            if (value != DBNull.Value)
                PerfStats.NetReceived = (long)value;

            value = vmData["vmNetTransmitted"];
            if (value != DBNull.Value)
                PerfStats.NetTransmitted = (long)value;

            value = vmData["vmNetUsage"];
            if (value != DBNull.Value)
                PerfStats.NetUsage = (long)value;

            value = vmData["esxHostName"];
            if (value != DBNull.Value)
                ESXHost.Name = (string)value;

            value = vmData["esxStatus"];
            if (value != DBNull.Value)
                ESXHost.Status = (HostSystemPowerState)value;

            value = vmData["esxBootTime"];
            if (value != DBNull.Value)
                ESXHost.BootTime = (DateTime)value;

            value = vmData["esxCPUMHz"];
            if (value != DBNull.Value)
                ESXHost.CPUMHz = (int)value;

            value = vmData["esxDomainName"];
            if (value != DBNull.Value)
                ESXHost.DomainName = (string)value;

            value = vmData["esxMemSize"];
            if (value != DBNull.Value)
                ESXHost.MemSize.Kilobytes = (long)value;

            value = vmData["esxNumCPUCores"];
            if (value != DBNull.Value)
                ESXHost.NumCPUCores = (short)value;

            value = vmData["esxNumCPUPkgs"];
            if (value != DBNull.Value)
                ESXHost.NumCPUPkgs = (short)value;

            value = vmData["esxNumCPUThreads"];
            if (value != DBNull.Value)
                ESXHost.NumCPUThreads = (short)value;

            value = vmData["esxNumNICs"];
            if (value != DBNull.Value)
                ESXHost.NumNICs = (int)value;

            value = vmData["esxCPUUsage"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.CpuUsage = (double)value;

            value = vmData["esxCPUUsageMHz"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.CpuUsageMHz = (int)value;

            value = vmData["esxDeviceLatency"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.DiskDeviceLatency = (long)value;

            value = vmData["esxKernelLatency"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.DiskKernelLatency = (long)value;

            value = vmData["esxQueueLatency"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.DiskQueueLatency = (long)value;

            value = vmData["esxTotalLatency"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.DiskTotalLatency = (long)value;

            value = vmData["esxDiskRead"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.DiskRead = (long)value;

            value = vmData["esxDiskWrite"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.DiskWrite = (long)value;

            value = vmData["esxDiskUsage"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.DiskUsage = (long)value;

            value = vmData["esxMemActive"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.MemActive.Kilobytes = (long)value;

            value = vmData["esxMemBallooned"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.MemBallooned.Kilobytes = (long)value;

            value = vmData["esxMemConsumed"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.MemConsumed.Kilobytes = (long)value;

            value = vmData["esxMemGranted"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.MemGranted.Kilobytes = (long)value;

            value = vmData["esxMemSwapInRate"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.MemSwapInRate = (long)value;

            value = vmData["esxMemSwapOutRate"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.MemSwapOutRate = (long)value;

            value = vmData["esxMemUsage"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.MemUsage = (double)value;

            value = vmData["esxNetReceived"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.NetReceived = (long)value;

            value = vmData["esxNetTransmitted"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.NetTransmitted = (long)value;

            value = vmData["esxNetUsage"];
            if (value != DBNull.Value)
                ESXHost.PerfStats.NetUsage = (long)value;
        }

        #endregion

        #region Properties

        public string InstanceUUID
        {
            get { return instanceUUID; }
            set { instanceUUID = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public VirtualMachinePowerState Status
        {
            get { return status; }
            set { status = value; }
        }

        public string HostName
        {
            get { return hostname; }
            set { hostname = value; }
        }

        public DateTime BootTime
        {
            get { return boottime; }
            set { boottime = value; }
        }

        public int NumCPUs
        {
            get { return numCPUs; }
            set { numCPUs = value; }
        }

        public long CPULimit
        {
            get { return cpuLimit; }
            set { cpuLimit = value; }
        }

        public long CPUReserve
        {
            get { return cpuReserve; }
            set { cpuReserve = value; }
        }

        public FileSize MemSize
        {
            get { return memSize; }
            set { memSize = value; }
        }

        public FileSize MemLimit
        {
            get { return memLimit; }
            set { memLimit = value; }
        }

        public FileSize MemReserve
        {
            get { return memReserve; }
            set { memReserve = value; }
        }

        public ESXHostServer ESXHost
        {
            get { return esxHost; }
            set { esxHost = value; }
        }

        public PerfStatistics PerfStats
        {
            set { vmPerfStats = value; }
            get { return vmPerfStats; }
        }
        #endregion
    }

    [Serializable]
    public class PerfStatistics
    {
        private double cpuUsage;
        private double cpuUsage_Raw;
        private long cpuUsageMHz;
        private long cpuReady;
        private long cpuSwapWait;
        private long memSwapInRate;
        private long memSwapOutRate;
        private FileSize memSwapped = new FileSize();
        private FileSize memActive = new FileSize();
        private FileSize memConsumed = new FileSize();
        private FileSize memGranted = new FileSize();
        private FileSize memBallooned = new FileSize();
        private double memUsage;
        private long diskRead;
        private long diskRead_Raw;
        private long diskWrite;
        private long diskWrite_Raw;
        private long diskUsage;
        private long diskDeviceLatency;
        private long diskKernelLatency;
        private long diskQueueLatency;
        private long diskTotalLatency;
        private long netUsage;
        private long netUsage_Raw;
        private long netReceived;
        private long netRecived_Raw;
        private long netTransmitted;
        private long netTransmitted_Raw;
        private long pagePerSec;
        private long pagePerSec_Raw;
        private FileSize availableByteHyperV = new FileSize();
        private HyperVCommonObjects hyperCommonObject;

        //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
        private double? vmCPUUtilizationBaselineMean;
        private double? vmCPUUtilizationAsBaselinePerc;
        //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline

        #region Constructors

        public PerfStatistics()
        {
            this.hyperCommonObject = new HyperVCommonObjects();
        }

        #endregion


        #region Properties

        //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
        public double? VmCPUUtilizationBaselineMean
        {
            get { return vmCPUUtilizationBaselineMean; }
            set { vmCPUUtilizationBaselineMean = value; }
        }
        public double? VmCPUUtilizationAsBaselinePerc
        {
            get { return vmCPUUtilizationAsBaselinePerc; }
            set { vmCPUUtilizationAsBaselinePerc = value; }
        }
        //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline

        public double CpuUsage
        {
            get { return cpuUsage; }
            set { cpuUsage = value; }
        }

        public double CpuUsage_Raw
        {
            get { return cpuUsage_Raw; }
            set { cpuUsage_Raw = value; }
        }

        public long PagePerSec
        {
            get { return pagePerSec; }
            set { pagePerSec = value; }
        }

        public long PagePerSec_Raw
        {
            get { return pagePerSec_Raw; }
            set { pagePerSec_Raw = value; }
        }
        public long CpuUsageMHz
        {
            get { return cpuUsageMHz; }
            set { cpuUsageMHz = value; }
        }

        public long CpuReady
        {
            get { return cpuReady; }
            set { cpuReady = value; }
        }

        public long CpuSwapWait
        {
            get { return cpuSwapWait; }
            set { cpuSwapWait = value; }
        }

        public long MemSwapInRate
        {
            get { return memSwapInRate; }
            set { memSwapInRate = value; }
        }

        public long MemSwapOutRate
        {
            get { return memSwapOutRate; }
            set { memSwapOutRate = value; }
        }

        public FileSize MemSwapped
        {
            get { return memSwapped; }
            set { memSwapped = value; }
        }

        public FileSize MemActive
        {
            get { return memActive; }
            set { memActive = value; }
        }

        public FileSize MemConsumed
        {
            get { return memConsumed; }
            set { memConsumed = value; }
        }

        public FileSize MemGranted
        {
            get { return memGranted; }
            set { memGranted = value; }
        }

        public FileSize MemBallooned
        {
            get { return memBallooned; }
            set { memBallooned = value; }
        }

        public FileSize AvaialableByteHyperV
        {
            get { return availableByteHyperV; }
            set { availableByteHyperV = value; }
        }

        public double MemUsage
        {
            get { return memUsage; }
            set { memUsage = value; }
        }

        public long DiskRead
        {
            get { return diskRead; }
            set { diskRead = value; }
        }

        public long DiskRead_Raw
        {
            get { return diskRead_Raw; }
            set { diskRead_Raw = value; }
        }

        public long DiskWrite
        {
            get { return diskWrite; }
            set { diskWrite = value; }
        }

        public long DiskWrite_Raw
        {
            get { return diskWrite_Raw; }
            set { diskWrite_Raw = value; }
        }

        public long DiskDeviceLatency
        {
            get { return diskDeviceLatency; }
            set { diskDeviceLatency = value; }
        }

        public long DiskKernelLatency
        {
            get { return diskKernelLatency; }
            set { diskKernelLatency = value; }
        }

        public long DiskQueueLatency
        {
            get { return diskQueueLatency; }
            set { diskQueueLatency = value; }
        }

        public long DiskTotalLatency
        {
            get { return diskTotalLatency; }
            set { diskTotalLatency = value; }
        }

        public long DiskUsage
        {
            get { return diskUsage; }
            set { diskUsage = value; }
        }

        public long NetUsage
        {
            get { return netUsage; }
            set { netUsage = value; }
        }
        public long NetUsage_Raw
        {
            get { return netUsage_Raw; }
            set { netUsage_Raw = value; }
        }

        public long NetReceived
        {
            get { return netReceived; }
            set { netReceived = value; }
        }

        public long NetReceived_Raw
        {
            get { return netRecived_Raw; }
            set { netRecived_Raw = value; }
        }
        public long NetTransmitted
        {
            get { return netTransmitted; }
            set { netTransmitted = value; }
        }

        public long NetTransmitted_Raw
        {
            get { return netTransmitted_Raw; }
            set { netTransmitted_Raw = value; }
        }

        public HyperVCommonObjects HyperCommonObject
        {
            set { hyperCommonObject = value; }
            get { return hyperCommonObject; }
        }
        #endregion
    }
}
