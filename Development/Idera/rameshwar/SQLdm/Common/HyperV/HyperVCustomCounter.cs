using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.HyperV
{
    class HyperVCustomCounter
    {
        public HyperVCustomCounter()
        {
            if (this.HyperVCustomCounterObjects.Count == 0)
            {
                fillCounterObjectList();
            }

        }
        //public const string HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY =  "HyperV Virtual Machine Health Summary";
        //public const string HYPERV_HYPERVISOR = "HyperV Hypervisor";
        public const string PROCESSOR = "CPU";
        public const string HYPERV_HYPERVISOR_LOGICAL_PRODESSOR = "HyperV Hypervisor Logical Processor";
        public const string HYPERV_HYPERVISIOR_ROOT_VIRTUAL_PROCESSOR = "HyperV Hypervisor Root Virtual Processor";
        public const string HYPERV_HYPERVISIOR_VIRTUAL_PROCESSOR = "HyperV Hypervisor Virtual Processor";
        public const string MEMORY = "Memory";
        //public const string HYPERV_HYPERVISIOR_PARTITION = "HyperV Hypervisor Partition";
        //public const string HYPERV_ROOT_PARTITION = "HyperV Root Partition";
        public const string HYPERV_VM_VID_PARTITION = "HyperV VM Vid Partition";
        //public const string NETWORK_INTERFACE = "Network Interface";
        //public const string HYPERV_VIRTUAL_SWITCH = "HyperV Virtual Switch";
        public const string HYPERV_LEGACY_NETWORK_ADAPTER = "HyperV Legacy Network Adapter";
        public const string HYPERV_VIRTUAL_NETWORK_ADAPTER = "HyperV Virtual Network Adapter";
        public const string PHYSICAL_DISK = "Physical Disk";
        public const string HYPERV_VIRTUAL_STORAGE_DEVICE = "HyperV Virtual Storage Device";
        public const string HYPERV_VIRTUAL_IDE_CONTROLLER = "HyperV Virtual IDE Controller";

        /// <summary>
        /// counter object
        /// </summary>
        //public static CustomCounterBasicInfo HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO;
        //public static CustomCounterBasicInfo HYPERV_HYPERVISOR_INFO;
        public static CustomCounterBasicInfo PROCESSOR_INFO;
        //public static CustomCounterBasicInfo HYPERV_HYPERVISOR_LOGICAL_PROCESSOR_INFO;
        //public static CustomCounterBasicInfo HYPERV_HYPERVISIOR_ROOT_VIRTUAL_PROCESSOR_INFO;
        //public static CustomCounterBasicInfo HYPERV_HYPERVISIOR_VIRTUAL_PROCESSOR_INFO;
        public static CustomCounterBasicInfo MEMORY_INFO;
        //public static CustomCounterBasicInfo HYPERV_HYPERVISIOR_PARTITION_INFO;
        //public static CustomCounterBasicInfo HYPERV_ROOT_PARTITION_INFO;
        //public static CustomCounterBasicInfo HYPERV_VM_VID_PARTITION_INFO;
        public static CustomCounterBasicInfo NETWORK_INTERFACE_INFO;
        //public static CustomCounterBasicInfo HYPERV_VIRTUAL_SWITCH_INFO;
        //public static CustomCounterBasicInfo HYPERV_LEGACY_NETWORK_ADAPTER_INFO;
        //public static CustomCounterBasicInfo HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO;
        public static CustomCounterBasicInfo PHYSICAL_DISK_INFO;
        public static CustomCounterBasicInfo HYPERV_VIRTUAL_STORAGE_DEVICE_INFO;
        public static CustomCounterBasicInfo HYPERV_VIRTUAL_IDE_CONTROLLER_INFO;




        public static Dictionary<string, CustomCounterBasicInfo> hyperVCustomCounterObjects = new Dictionary<string, CustomCounterBasicInfo>();

        public Dictionary<string, CustomCounterBasicInfo> HyperVCustomCounterObjects
        {
            get { return hyperVCustomCounterObjects; }
        }

        static void fillCounterObjectList()
        {

            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO.Group = "health";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO.GroupLabel = "Health";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO.CounterKey = 0;
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO.Counter = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO.CounterLabel = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO.Instance = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO.UOM = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO.Singleton = true;
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_COUNTER_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_COUNTER_LIST.Add("Type", new List<string>() { "VM","Host"});
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_COUNTER_LIST.Add("Health Ok",new List<string>());
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_COUNTER_LIST.Add("Health Critical", new List<string>());
            //HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO.CounterList = HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_COUNTER_LIST;

            //CustomCounterBasicInfo HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO.Group = "health";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO.GroupLabel = "Health Ok";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO.CounterKey = 1;
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO.Counter = "health ok";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO.CounterLabel = "Health Ok";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO.Instance = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO.UOM = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO.Singleton = true;
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO.WmiClass = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_COUNTER_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_COUNTER_LIST.Add("Type", new List<string>() { "VM", "Host" });
            //HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO.CounterList = HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_COUNTER_LIST;

            //CustomCounterBasicInfo HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO.Group = "health";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO.GroupLabel = "Health Critical";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO.CounterKey = 2;
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO.Counter = "health critical";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO.CounterLabel = "Health Critical";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO.Instance = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO.UOM = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO.Singleton = true;
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO.WmiClass = "";
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_COUNTER_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_COUNTER_LIST.Add("Type", new List<string>() { "VM", "Host" });
            //HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO.CounterList = HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_COUNTER_LIST;

            //HYPERV_HYPERVISOR_INFO = new CustomCounterBasicInfo();
            //HYPERV_HYPERVISOR_INFO.Group = "health-hvisior";
            //HYPERV_HYPERVISOR_INFO.GroupLabel = "Health";
            //HYPERV_HYPERVISOR_INFO.GroupSummary = "";
            //HYPERV_HYPERVISOR_INFO.CounterKey = 3;
            //HYPERV_HYPERVISOR_INFO.Counter = "";
            //HYPERV_HYPERVISOR_INFO.CounterLabel = "";
            //HYPERV_HYPERVISOR_INFO.CounterSummary = "";
            //HYPERV_HYPERVISOR_INFO.Instance = "";
            //HYPERV_HYPERVISOR_INFO.UOM = "";
            //HYPERV_HYPERVISOR_INFO.Singleton = true;
            //Dictionary<string, List<string>> HYPERV_HYPERVISOR_LIST = new Dictionary<string, List<string>>();
            //HYPERV_HYPERVISOR_LIST.Add("Type", new List<string>() { "Host" });
            //HYPERV_HYPERVISOR_INFO.CounterList = HYPERV_HYPERVISOR_LIST;

            // Storage : Physical Disk
            PHYSICAL_DISK_INFO = new CustomCounterBasicInfo();
            PHYSICAL_DISK_INFO.Group = "disk";
            PHYSICAL_DISK_INFO.GroupLabel = "Disk";
            PHYSICAL_DISK_INFO.GroupSummary = "";
            PHYSICAL_DISK_INFO.CounterKey = 4;
            PHYSICAL_DISK_INFO.Counter = "";
            PHYSICAL_DISK_INFO.CounterLabel = "";
            PHYSICAL_DISK_INFO.CounterSummary = "";
            PHYSICAL_DISK_INFO.Instance = "";
            PHYSICAL_DISK_INFO.UOM = "";
            PHYSICAL_DISK_INFO.Singleton = true;
            Dictionary<string, List<string>> PHYSICAL_DISK_LIST = new Dictionary<string, List<string>>();
            PHYSICAL_DISK_LIST.Add("Type", new List<string>() { "VM", "Host" });
            PHYSICAL_DISK_LIST.Add("Current Disk Queue Length", new List<string>());
            PHYSICAL_DISK_LIST.Add("Disk Bytes / Sec", new List<string>());
            PHYSICAL_DISK_LIST.Add("Disk Transfers / Sec", new List<string>());
            PHYSICAL_DISK_INFO.CounterList = PHYSICAL_DISK_LIST;

            CustomCounterBasicInfo PHYSICAL_DISK_TPS_INFO = new CustomCounterBasicInfo();
            PHYSICAL_DISK_TPS_INFO.Group = "disk";
            PHYSICAL_DISK_TPS_INFO.GroupLabel = "Disk Transfers / Sec";
            PHYSICAL_DISK_TPS_INFO.GroupSummary = "";
            PHYSICAL_DISK_TPS_INFO.CounterKey = 5;
            PHYSICAL_DISK_TPS_INFO.Counter = "Disk Transfers / Sec";
            PHYSICAL_DISK_TPS_INFO.CounterLabel = "Disk Transfers / Sec";
            PHYSICAL_DISK_TPS_INFO.CounterSummary = "";
            PHYSICAL_DISK_TPS_INFO.Instance = "";
            PHYSICAL_DISK_TPS_INFO.UOM = "";
            PHYSICAL_DISK_TPS_INFO.Singleton = true;
            PHYSICAL_DISK_TPS_INFO.WmiClass = "Win32_perfformatteddata_perfdisk_LogicalDisk";
            PHYSICAL_DISK_TPS_INFO.SearcherKey = "DiskTransfersPerSec";
            PHYSICAL_DISK_TPS_INFO.WhereClause = "Name = '_Total'";
            Dictionary<string, List<string>> PHYSICAL_DISK_TPS_LIST = new Dictionary<string, List<string>>();
            PHYSICAL_DISK_TPS_LIST.Add("Type", new List<string>() { "VM", "Host" });
            PHYSICAL_DISK_TPS_INFO.CounterList = PHYSICAL_DISK_TPS_LIST;

            CustomCounterBasicInfo PHYSICAL_DISK_Q_LEN_INFO = new CustomCounterBasicInfo();
            PHYSICAL_DISK_Q_LEN_INFO.Group = "disk";
            PHYSICAL_DISK_Q_LEN_INFO.GroupLabel = "Current Disk Queue Length";
            PHYSICAL_DISK_Q_LEN_INFO.GroupSummary = "";
            PHYSICAL_DISK_Q_LEN_INFO.CounterKey = 6;
            PHYSICAL_DISK_Q_LEN_INFO.Counter = "Current Disk Queue Length";
            PHYSICAL_DISK_Q_LEN_INFO.CounterLabel = "Current Disk Queue Length";
            PHYSICAL_DISK_Q_LEN_INFO.CounterSummary = "";
            PHYSICAL_DISK_Q_LEN_INFO.Instance = "";
            PHYSICAL_DISK_Q_LEN_INFO.UOM = "";
            PHYSICAL_DISK_Q_LEN_INFO.Singleton = true;
            PHYSICAL_DISK_Q_LEN_INFO.WmiClass = "Win32_perfformatteddata_perfdisk_LogicalDisk";
            PHYSICAL_DISK_Q_LEN_INFO.SearcherKey = "CurrentDiskQueueLength";
            PHYSICAL_DISK_Q_LEN_INFO.WhereClause = "Name = '_Total'";
            Dictionary<string, List<string>> PHYSICAL_DISK_Q_LEN_LIST = new Dictionary<string, List<string>>();
            PHYSICAL_DISK_Q_LEN_LIST.Add("Type", new List<string>() { "VM", "Host" });
            PHYSICAL_DISK_Q_LEN_INFO.CounterList = PHYSICAL_DISK_Q_LEN_LIST;

            CustomCounterBasicInfo PHYSICAL_DISK_BPS_INFO = new CustomCounterBasicInfo();
            PHYSICAL_DISK_BPS_INFO.Group = "disk";
            PHYSICAL_DISK_BPS_INFO.GroupLabel = "Disk Bytes / Sec";
            PHYSICAL_DISK_BPS_INFO.GroupSummary = "";
            PHYSICAL_DISK_BPS_INFO.CounterKey = 7;
            PHYSICAL_DISK_BPS_INFO.Counter = "Disk Bytes / Sec";
            PHYSICAL_DISK_BPS_INFO.CounterLabel = "Disk Bytes / Sec";
            PHYSICAL_DISK_BPS_INFO.CounterSummary = "";
            PHYSICAL_DISK_BPS_INFO.Instance = "";
            PHYSICAL_DISK_BPS_INFO.UOM = "";
            PHYSICAL_DISK_BPS_INFO.Singleton = true;
            PHYSICAL_DISK_BPS_INFO.WmiClass = "Win32_perfformatteddata_perfdisk_LogicalDisk";
            PHYSICAL_DISK_BPS_INFO.SearcherKey = "DiskBytesPerSec";
            PHYSICAL_DISK_BPS_INFO.WhereClause = "Name = '_Total'";
            Dictionary<string, List<string>> PHYSICAL_DISK_BPS_LIST = new Dictionary<string, List<string>>();
            PHYSICAL_DISK_BPS_LIST.Add("Type", new List<string>() { "VM", "Host" });
            PHYSICAL_DISK_BPS_INFO.CounterList = PHYSICAL_DISK_BPS_LIST;

            //Storage : HyperV Virtual Storage Device
            HYPERV_VIRTUAL_STORAGE_DEVICE_INFO = new CustomCounterBasicInfo();
            HYPERV_VIRTUAL_STORAGE_DEVICE_INFO.Group = "disk-vsd";
            HYPERV_VIRTUAL_STORAGE_DEVICE_INFO.GroupLabel = "Disk";
            HYPERV_VIRTUAL_STORAGE_DEVICE_INFO.GroupSummary = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_INFO.CounterKey = 8;
            HYPERV_VIRTUAL_STORAGE_DEVICE_INFO.Counter = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_INFO.CounterLabel = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_INFO.CounterSummary = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_INFO.Instance = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_INFO.UOM = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_INFO.Singleton = true;
            Dictionary<string, List<string>> HYPERV_VIRTUAL_STORAGE_DEVICE_LIST = new Dictionary<string, List<string>>();
            HYPERV_VIRTUAL_STORAGE_DEVICE_LIST.Add("Type", new List<string>() { "VM" });
            HYPERV_VIRTUAL_STORAGE_DEVICE_LIST.Add("Error Count", new List<string>());
            HYPERV_VIRTUAL_STORAGE_DEVICE_LIST.Add("Flush Count", new List<string>());
            HYPERV_VIRTUAL_STORAGE_DEVICE_LIST.Add("Read Bytes / Sec", new List<string>());
            HYPERV_VIRTUAL_STORAGE_DEVICE_LIST.Add("Write Bytes / Sec", new List<string>());
            HYPERV_VIRTUAL_STORAGE_DEVICE_LIST.Add("Read Count", new List<string>());
            HYPERV_VIRTUAL_STORAGE_DEVICE_LIST.Add("Write Count", new List<string>());
            HYPERV_VIRTUAL_STORAGE_DEVICE_INFO.CounterList = HYPERV_VIRTUAL_STORAGE_DEVICE_LIST;

            //CustomCounterBasicInfo HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO.Group = "disk-vsd";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO.GroupLabel = "Error Count";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO.CounterKey = 9;
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO.Counter = "Error Count";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO.CounterLabel = "Error Count";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO.Instance = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO.UOM = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO.Singleton = true;
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO.WmiClass = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_STORAGE_DEVICE_EC_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO.CounterList = HYPERV_VIRTUAL_STORAGE_DEVICE_EC_LIST;

            //CustomCounterBasicInfo HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO.Group = "disk-vsd";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO.GroupLabel = "Flush Count";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO.CounterKey = 10;
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO.Counter = "Flush Count";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO.CounterLabel = "Flush Count";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO.Instance = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO.UOM = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO.Singleton = true;
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO.WmiClass = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_STORAGE_DEVICE_FC_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO.CounterList = HYPERV_VIRTUAL_STORAGE_DEVICE_FC_LIST;

            CustomCounterBasicInfo HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO = new CustomCounterBasicInfo();
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.Group = "disk-vsd";
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.GroupLabel = "Read Bytes / Sec";
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.GroupSummary = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.CounterKey = 11;
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.Counter = "Read Bytes / Sec";
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.CounterLabel = "Read Bytes / Sec";
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.CounterSummary = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.Instance = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.UOM = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.Singleton = true;
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.WmiClass = "Win32_PerfFormattedData_PerfDisk_LogicalDisk";
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.SearcherKey = "DiskReadBytesPerSec";
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.WhereClause = "Name = '_Total'";
            Dictionary<string, List<string>> HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_LIST = new Dictionary<string, List<string>>();
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_LIST.Add("Type", new List<string>() { "VM" });
            HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO.CounterList = HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_LIST;

            CustomCounterBasicInfo HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO = new CustomCounterBasicInfo();
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.Group = "disk-vsd";
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.GroupLabel = "Write Bytes / Sec";
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.GroupSummary = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.CounterKey = 12;
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.Counter = "Write Bytes / Sec";
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.CounterLabel = "Write Bytes / Sec";
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.CounterSummary = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.Instance = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.UOM = "";
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.Singleton = true;
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.WmiClass = "Win32_PerfFormattedData_PerfDisk_LogicalDisk";
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.SearcherKey = "DiskWriteBytesPerSec";
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.WhereClause = "Name = '_Total'";
            Dictionary<string, List<string>> HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_LIST = new Dictionary<string, List<string>>();
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_LIST.Add("Type", new List<string>() { "VM" });
            HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO.CounterList = HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_LIST;

            //CustomCounterBasicInfo HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO.Group = "disk-vsd";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO.GroupLabel = "Read Count";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO.CounterKey = 13;
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO.Counter = "Read Count";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO.CounterLabel = "Read Count";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO.Instance = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO.UOM = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO.Singleton = true;
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO.WmiClass = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_STORAGE_DEVICE_RC_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO.CounterList = HYPERV_VIRTUAL_STORAGE_DEVICE_RC_LIST;

            //CustomCounterBasicInfo HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO.Group = "disk-vsd";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO.GroupLabel = "Write Count";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO.CounterKey = 14;
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO.Counter = "Write Count";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO.CounterLabel = "Write Count";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO.Instance = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO.UOM = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO.Singleton = true;
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO.WmiClass = "";
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_STORAGE_DEVICE_WC_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO.CounterList = HYPERV_VIRTUAL_STORAGE_DEVICE_WC_LIST;

            //Storage : HyperV Virtual IDE Controller
            HYPERV_VIRTUAL_IDE_CONTROLLER_INFO = new CustomCounterBasicInfo();
            HYPERV_VIRTUAL_IDE_CONTROLLER_INFO.Group = "disk-ide";
            HYPERV_VIRTUAL_IDE_CONTROLLER_INFO.GroupLabel = "Disk";
            HYPERV_VIRTUAL_IDE_CONTROLLER_INFO.GroupSummary = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_INFO.CounterKey = 15;
            HYPERV_VIRTUAL_IDE_CONTROLLER_INFO.Counter = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_INFO.CounterLabel = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_INFO.CounterSummary = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_INFO.Instance = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_INFO.UOM = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_INFO.Singleton = true;
            Dictionary<string, List<string>> HYPERV_VIRTUAL_IDE_CONTROLLER_LIST = new Dictionary<string, List<string>>();
            HYPERV_VIRTUAL_IDE_CONTROLLER_LIST.Add("Type", new List<string>() { "VM" });
            HYPERV_VIRTUAL_IDE_CONTROLLER_LIST.Add("Read Bytes / Sec", new List<string>());
            HYPERV_VIRTUAL_IDE_CONTROLLER_LIST.Add("Write Bytes / Sec", new List<string>());
            HYPERV_VIRTUAL_IDE_CONTROLLER_LIST.Add("Read Sectors / Sec", new List<string>());
            HYPERV_VIRTUAL_IDE_CONTROLLER_LIST.Add("Write Sectors / Sec", new List<string>());
            HYPERV_VIRTUAL_IDE_CONTROLLER_INFO.CounterList = HYPERV_VIRTUAL_IDE_CONTROLLER_LIST;

            CustomCounterBasicInfo HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO = new CustomCounterBasicInfo();
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.Group = "disk-ide";
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.GroupLabel = "Read Bytes / Sec";
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.GroupSummary = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.CounterKey = 16;
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.Counter = "Read Bytes / Sec";
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.CounterLabel = "Read Bytes / Sec";
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.CounterSummary = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.Instance = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.UOM = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.Singleton = true;
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.WmiClass = "Win32_PerfFormattedData_PerfDisk_LogicalDisk";
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.SearcherKey = "DiskReadBytesPerSec";
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.WhereClause = "Name = '_Total'";
            Dictionary<string, List<string>> HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_LIST = new Dictionary<string, List<string>>();
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_LIST.Add("Type", new List<string>() { "VM" });
            HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO.CounterList = HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_LIST;

            CustomCounterBasicInfo HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO = new CustomCounterBasicInfo();
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.Group = "disk-ide";
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.GroupLabel = "Write Bytes / Sec";
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.GroupSummary = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.CounterKey = 17;
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.Counter = "Write Bytes / Sec";
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.CounterLabel = "Write Bytes / Sec";
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.CounterSummary = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.Instance = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.UOM = "";
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.Singleton = true;
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.WmiClass = "Win32_PerfFormattedData_PerfDisk_LogicalDisk";
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.SearcherKey = "DiskWriteBytesPerSec";
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.WhereClause = "Name = '_Total'";
            Dictionary<string, List<string>> HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_LIST = new Dictionary<string, List<string>>();
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_LIST.Add("Type", new List<string>() { "VM" });
            HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO.CounterList = HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_LIST;

            //CustomCounterBasicInfo HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO.Group = "disk-ide";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO.GroupLabel = "Read Sectors / Sec";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO.CounterKey = 18;
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO.Counter = "Read Sectors / Sec";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO.CounterLabel = "Read Sectors / Sec";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO.Instance = "";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO.UOM = "";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO.Singleton = true;
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO.WmiClass = "";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO.CounterList = HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_LIST;

            //CustomCounterBasicInfo HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO.Group = "disk-ide";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO.GroupLabel = "Write Sectors / Sec";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO.CounterKey = 19;
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO.Counter = "Write Sectors / Sec";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO.CounterLabel = "Write Sectors / Sec";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO.Instance = "";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO.UOM = "";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO.Singleton = true;
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO.WmiClass = "";
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO.CounterList = HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_LIST;

            // Networking : Network Interface
            NETWORK_INTERFACE_INFO = new CustomCounterBasicInfo();
            NETWORK_INTERFACE_INFO.Group = "net";
            NETWORK_INTERFACE_INFO.GroupLabel = "NetWork";
            NETWORK_INTERFACE_INFO.GroupSummary = "";
            NETWORK_INTERFACE_INFO.CounterKey = 20;
            NETWORK_INTERFACE_INFO.Counter = "";
            NETWORK_INTERFACE_INFO.CounterLabel = "";
            NETWORK_INTERFACE_INFO.CounterSummary = "";
            NETWORK_INTERFACE_INFO.Instance = "";
            NETWORK_INTERFACE_INFO.UOM = "";
            NETWORK_INTERFACE_INFO.Singleton = true;
            Dictionary<string, List<string>> NETWORK_INTERFACE_LIST = new Dictionary<string, List<string>>();
            NETWORK_INTERFACE_LIST.Add("Type", new List<string>() { "VM" });
            NETWORK_INTERFACE_LIST.Add("Bytes Sent / Sec", new List<string>());
            NETWORK_INTERFACE_LIST.Add("Bytes Received / Sec", new List<string>());
            NETWORK_INTERFACE_LIST.Add("Packets Sent / Sec", new List<string>());
            NETWORK_INTERFACE_LIST.Add("Packets Received / Sec", new List<string>());
            NETWORK_INTERFACE_LIST.Add("Packets Outbound Errors", new List<string>());
            NETWORK_INTERFACE_LIST.Add("Packets Receive Errors", new List<string>());
            NETWORK_INTERFACE_INFO.CounterList = NETWORK_INTERFACE_LIST;

            CustomCounterBasicInfo NETWORK_INTERFACE_BTSPS_INFO = new CustomCounterBasicInfo();
            NETWORK_INTERFACE_BTSPS_INFO.Group = "net";
            NETWORK_INTERFACE_BTSPS_INFO.GroupLabel = "Bytes Sent / Sec";
            NETWORK_INTERFACE_BTSPS_INFO.GroupSummary = "";
            NETWORK_INTERFACE_BTSPS_INFO.CounterKey = 21;
            NETWORK_INTERFACE_BTSPS_INFO.Counter = "Bytes Sent / Sec";
            NETWORK_INTERFACE_BTSPS_INFO.CounterLabel = "Bytes Sent / Sec";
            NETWORK_INTERFACE_BTSPS_INFO.CounterSummary = "";
            NETWORK_INTERFACE_BTSPS_INFO.Instance = "";
            NETWORK_INTERFACE_BTSPS_INFO.UOM = "";
            NETWORK_INTERFACE_BTSPS_INFO.Singleton = true;
            NETWORK_INTERFACE_BTSPS_INFO.WmiClass = "Win32_PerfFormattedData_Tcpip_NetworkInterface";
            NETWORK_INTERFACE_BTSPS_INFO.WhereClause = "name like 'Microsoft Hyper-V%' or name like 'Microsoft Virtual Machine%'";
            NETWORK_INTERFACE_BTSPS_INFO.SearcherKey = "BytesSentPersec";
            Dictionary<string, List<string>> NETWORK_INTERFACE_BTSPS_LIST = new Dictionary<string, List<string>>();
            NETWORK_INTERFACE_BTSPS_LIST.Add("Type", new List<string>() { "VM" });
            NETWORK_INTERFACE_BTSPS_INFO.CounterList = NETWORK_INTERFACE_BTSPS_LIST;

            CustomCounterBasicInfo NETWORK_INTERFACE_BTRPS_INFO = new CustomCounterBasicInfo();
            NETWORK_INTERFACE_BTRPS_INFO.Group = "net";
            NETWORK_INTERFACE_BTRPS_INFO.GroupLabel = "Bytes Received / Sec";
            NETWORK_INTERFACE_BTRPS_INFO.GroupSummary = "";
            NETWORK_INTERFACE_BTRPS_INFO.CounterKey = 22;
            NETWORK_INTERFACE_BTRPS_INFO.Counter = "Bytes Received / Sec";
            NETWORK_INTERFACE_BTRPS_INFO.CounterLabel = "Bytes Received / Sec";
            NETWORK_INTERFACE_BTRPS_INFO.CounterSummary = "";
            NETWORK_INTERFACE_BTRPS_INFO.Instance = "";
            NETWORK_INTERFACE_BTRPS_INFO.UOM = "";
            NETWORK_INTERFACE_BTRPS_INFO.Singleton = true;
            NETWORK_INTERFACE_BTRPS_INFO.WmiClass = "Win32_PerfFormattedData_Tcpip_NetworkInterface";
            NETWORK_INTERFACE_BTRPS_INFO.WhereClause = "name like 'Microsoft Hyper-V%' or name like 'Microsoft Virtual Machine%'";
            NETWORK_INTERFACE_BTRPS_INFO.SearcherKey = "BytesReceivedPersec";
            Dictionary<string, List<string>> NETWORK_INTERFACE_BTRPS_LIST = new Dictionary<string, List<string>>();
            NETWORK_INTERFACE_BTRPS_LIST.Add("Type", new List<string>() { "VM" });
            NETWORK_INTERFACE_BTRPS_INFO.CounterList = NETWORK_INTERFACE_BTRPS_LIST;

            CustomCounterBasicInfo NETWORK_INTERFACE_PSPS_INFO = new CustomCounterBasicInfo();
            NETWORK_INTERFACE_PSPS_INFO.Group = "net";
            NETWORK_INTERFACE_PSPS_INFO.GroupLabel = "Packets Sent / Sec";
            NETWORK_INTERFACE_PSPS_INFO.GroupSummary = "";
            NETWORK_INTERFACE_PSPS_INFO.CounterKey = 23;
            NETWORK_INTERFACE_PSPS_INFO.Counter = "Packets Sent / Sec";
            NETWORK_INTERFACE_PSPS_INFO.CounterLabel = "Packets Sent / Sec";
            NETWORK_INTERFACE_PSPS_INFO.CounterSummary = "";
            NETWORK_INTERFACE_PSPS_INFO.Instance = "";
            NETWORK_INTERFACE_PSPS_INFO.UOM = "";
            NETWORK_INTERFACE_PSPS_INFO.Singleton = true;
            NETWORK_INTERFACE_PSPS_INFO.WmiClass = "Win32_PerfFormattedData_Tcpip_NetworkInterface";
            NETWORK_INTERFACE_PSPS_INFO.SearcherKey = "PacketsSentPersec";
            NETWORK_INTERFACE_PSPS_INFO.WhereClause = "name like 'Microsoft Hyper-V%' or name like 'Microsoft Virtual Machine%'";
            Dictionary<string, List<string>> NETWORK_INTERFACE_PSPS_LIST = new Dictionary<string, List<string>>();
            NETWORK_INTERFACE_PSPS_LIST.Add("Type", new List<string>() { "VM" });
            NETWORK_INTERFACE_PSPS_INFO.CounterList = NETWORK_INTERFACE_PSPS_LIST;

            CustomCounterBasicInfo NETWORK_INTERFACE_PRPS_INFO = new CustomCounterBasicInfo();
            NETWORK_INTERFACE_PRPS_INFO.Group = "net";
            NETWORK_INTERFACE_PRPS_INFO.GroupLabel = "Packets Received / Sec";
            NETWORK_INTERFACE_PRPS_INFO.GroupSummary = "";
            NETWORK_INTERFACE_PRPS_INFO.CounterKey = 24;
            NETWORK_INTERFACE_PRPS_INFO.Counter = "Packets Received / Sec";
            NETWORK_INTERFACE_PRPS_INFO.CounterLabel = "PacketsReceived / Sec";
            NETWORK_INTERFACE_PRPS_INFO.CounterSummary = "";
            NETWORK_INTERFACE_PRPS_INFO.Instance = "";
            NETWORK_INTERFACE_PRPS_INFO.UOM = "";
            NETWORK_INTERFACE_PRPS_INFO.Singleton = true;
            NETWORK_INTERFACE_PRPS_INFO.WmiClass = "Win32_PerfFormattedData_Tcpip_NetworkInterface";
            NETWORK_INTERFACE_PRPS_INFO.SearcherKey = "PacketsReceivedPersec";
            NETWORK_INTERFACE_PRPS_INFO.WhereClause = "name like 'Microsoft Hyper-V%' or name like 'Microsoft Virtual Machine%'";
            Dictionary<string, List<string>> NETWORK_INTERFACE_PRPS_LIST = new Dictionary<string, List<string>>();
            NETWORK_INTERFACE_PRPS_LIST.Add("Type", new List<string>() { "VM" });
            NETWORK_INTERFACE_PRPS_INFO.CounterList = NETWORK_INTERFACE_PRPS_LIST;

            CustomCounterBasicInfo NETWORK_INTERFACE_POE_INFO = new CustomCounterBasicInfo();
            NETWORK_INTERFACE_POE_INFO.Group = "net";
            NETWORK_INTERFACE_POE_INFO.GroupLabel = "Packets Outbound Errors";
            NETWORK_INTERFACE_POE_INFO.GroupSummary = "";
            NETWORK_INTERFACE_POE_INFO.CounterKey = 25;
            NETWORK_INTERFACE_POE_INFO.Counter = "Packets Outbound Errors";
            NETWORK_INTERFACE_POE_INFO.CounterLabel = "Packets Outbound Errors";
            NETWORK_INTERFACE_POE_INFO.CounterSummary = "";
            NETWORK_INTERFACE_POE_INFO.Instance = "";
            NETWORK_INTERFACE_POE_INFO.UOM = "";
            NETWORK_INTERFACE_POE_INFO.Singleton = true;
            NETWORK_INTERFACE_POE_INFO.WmiClass = "Win32_PerfFormattedData_Tcpip_NetworkInterface";
            NETWORK_INTERFACE_POE_INFO.SearcherKey = "PacketsOutboundErrors";
            NETWORK_INTERFACE_POE_INFO.WhereClause = "name like 'Microsoft Hyper-V%' or name like 'Microsoft Virtual Machine%'";
            Dictionary<string, List<string>> NETWORK_INTERFACE_POE_LIST = new Dictionary<string, List<string>>();
            NETWORK_INTERFACE_POE_LIST.Add("Type", new List<string>() { "VM" });
            NETWORK_INTERFACE_POE_INFO.CounterList = NETWORK_INTERFACE_POE_LIST;

            CustomCounterBasicInfo NETWORK_INTERFACE_PRE_INFO = new CustomCounterBasicInfo();
            NETWORK_INTERFACE_PRE_INFO.Group = "net";
            NETWORK_INTERFACE_PRE_INFO.GroupLabel = "Packets Receive Errors";
            NETWORK_INTERFACE_PRE_INFO.GroupSummary = "";
            NETWORK_INTERFACE_PRE_INFO.CounterKey = 26;
            NETWORK_INTERFACE_PRE_INFO.Counter = "Packets Receive Errors";
            NETWORK_INTERFACE_PRE_INFO.CounterLabel = "Packets Receive Errors";
            NETWORK_INTERFACE_PRE_INFO.CounterSummary = "";
            NETWORK_INTERFACE_PRE_INFO.Instance = "";
            NETWORK_INTERFACE_PRE_INFO.UOM = "";
            NETWORK_INTERFACE_PRE_INFO.Singleton = true;
            NETWORK_INTERFACE_PRE_INFO.WmiClass = "Win32_PerfFormattedData_Tcpip_NetworkInterface";
            NETWORK_INTERFACE_PRE_INFO.SearcherKey = "PacketsReceivedErrors";
            NETWORK_INTERFACE_PRE_INFO.WhereClause = "name like 'Microsoft Hyper-V%' or name like 'Microsoft Virtual Machine%'";
            Dictionary<string, List<string>> NETWORK_INTERFACE_PRE_LIST = new Dictionary<string, List<string>>();
            NETWORK_INTERFACE_PRE_LIST.Add("Type", new List<string>() { "VM" });
            NETWORK_INTERFACE_PRE_INFO.CounterList = NETWORK_INTERFACE_PRE_LIST;

            //Networking : HyperV Virtual Switch
            //HYPERV_VIRTUAL_SWITCH_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_SWITCH_INFO.Group = "net-vs";
            //HYPERV_VIRTUAL_SWITCH_INFO.GroupLabel = "NetWork";
            //HYPERV_VIRTUAL_SWITCH_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_SWITCH_INFO.CounterKey = 25;
            //HYPERV_VIRTUAL_SWITCH_INFO.Counter = "";
            //HYPERV_VIRTUAL_SWITCH_INFO.CounterLabel = "";
            //HYPERV_VIRTUAL_SWITCH_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_SWITCH_INFO.Instance = "";
            //HYPERV_VIRTUAL_SWITCH_INFO.UOM = "";
            //HYPERV_VIRTUAL_SWITCH_INFO.Singleton = true;
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_SWITCH_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_SWITCH_LIST.Add("Type", new List<string>() { "VM"});
            //HYPERV_VIRTUAL_SWITCH_LIST.Add("Bytes/Sec", new List<string>());
            //HYPERV_VIRTUAL_SWITCH_LIST.Add("Packets/Sec", new List<string>());
            //HYPERV_VIRTUAL_SWITCH_INFO.CounterList = HYPERV_VIRTUAL_SWITCH_LIST;

            //CustomCounterBasicInfo HYPERV_VIRTUAL_SWITCH_BPS_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_SWITCH_BPS_INFO.Group = "net-vs";
            //HYPERV_VIRTUAL_SWITCH_BPS_INFO.GroupLabel = "Bytes/Sec";
            //HYPERV_VIRTUAL_SWITCH_BPS_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_SWITCH_BPS_INFO.CounterKey = 26;
            //HYPERV_VIRTUAL_SWITCH_BPS_INFO.Counter = "";
            //HYPERV_VIRTUAL_SWITCH_BPS_INFO.CounterLabel = "Bytes/Sec";
            //HYPERV_VIRTUAL_SWITCH_BPS_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_SWITCH_BPS_INFO.Instance = "";
            //HYPERV_VIRTUAL_SWITCH_BPS_INFO.UOM = "";
            //HYPERV_VIRTUAL_SWITCH_BPS_INFO.Singleton = true;
            //HYPERV_VIRTUAL_SWITCH_BPS_INFO.WmiClass = "";
            //HYPERV_VIRTUAL_SWITCH_BPS_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_SWITCH_BPS_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_SWITCH_BPS_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_VIRTUAL_SWITCH_BPS_INFO.CounterList = HYPERV_VIRTUAL_SWITCH_BPS_LIST;

            //CustomCounterBasicInfo HYPERV_VIRTUAL_SWITCH_PPS_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_SWITCH_PPS_INFO.Group = "net-vs";
            //HYPERV_VIRTUAL_SWITCH_PPS_INFO.GroupLabel = "Packets/Sec";
            //HYPERV_VIRTUAL_SWITCH_PPS_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_SWITCH_PPS_INFO.CounterKey = 27;
            //HYPERV_VIRTUAL_SWITCH_PPS_INFO.Counter = "Packets/Sec";
            //HYPERV_VIRTUAL_SWITCH_PPS_INFO.CounterLabel = "Packets/Sec";
            //HYPERV_VIRTUAL_SWITCH_PPS_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_SWITCH_PPS_INFO.Instance = "";
            //HYPERV_VIRTUAL_SWITCH_PPS_INFO.UOM = "";
            //HYPERV_VIRTUAL_SWITCH_PPS_INFO.Singleton = true;
            //HYPERV_VIRTUAL_SWITCH_PPS_INFO.WmiClass = "";
            //HYPERV_VIRTUAL_SWITCH_PPS_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_SWITCH_PPS_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_SWITCH_PPS_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_VIRTUAL_SWITCH_PPS_INFO.CounterList = HYPERV_VIRTUAL_SWITCH_PPS_LIST;

            //Networking : HyperV Legacy Network Adapter
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO = new CustomCounterBasicInfo();
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO.Group = "net-lna";
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO.GroupLabel = "NetWork";
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO.GroupSummary = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO.CounterKey = 28;
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO.Counter = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO.CounterLabel = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO.CounterSummary = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO.Instance = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO.UOM = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO.Singleton = true;
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO.WmiClass = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_LEGACY_NETWORK_ADAPTER_LIST = new Dictionary<string, List<string>>();
            //HYPERV_LEGACY_NETWORK_ADAPTER_LIST.Add("Type", new List<string>() { "VM"});
            //HYPERV_LEGACY_NETWORK_ADAPTER_LIST.Add("Bytes Sent / Sec", new List<string>());
            //HYPERV_LEGACY_NETWORK_ADAPTER_LIST.Add("Bytes Received / Sec", new List<string>());
            //HYPERV_LEGACY_NETWORK_ADAPTER_INFO.CounterList = HYPERV_LEGACY_NETWORK_ADAPTER_LIST;

            //CustomCounterBasicInfo HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO = new CustomCounterBasicInfo();
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO.Group = "net-lna";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO.GroupLabel = "Bytes Sent / Sec";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO.GroupSummary = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO.CounterKey = 29;
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO.Counter = "Bytes Sent / Sec";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO.CounterLabel = "Bytes Sent / Sec";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO.CounterSummary = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO.Instance = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO.UOM = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO.Singleton = true;
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO.WmiClass = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_LIST = new Dictionary<string, List<string>>();
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO.CounterList = HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_LIST;

            //CustomCounterBasicInfo HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO = new CustomCounterBasicInfo();
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO.Group = "net-lna";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO.GroupLabel = "Bytes Received / Sec";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO.GroupSummary = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO.CounterKey = 30;
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO.Counter = "Bytes Received / Sec";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO.CounterLabel = "Bytes Received / Sec";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO.CounterSummary = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO.Instance = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO.UOM = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO.Singleton = true;
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO.WmiClass = "";
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_LIST = new Dictionary<string, List<string>>();
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO.CounterList = HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_LIST;

            //Networking : HyperV Virtual Network Adapter
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO.Group = "net-vna";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO.GroupLabel = "NetWork";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO.CounterKey = 31;
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO.Counter = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO.CounterLabel = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO.Instance = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO.UOM = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO.Singleton = true;
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO.WmiClass = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_NETWORK_ADAPTER_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_LIST.Add("Type", new List<string>() { "VM"});
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_LIST.Add("Bytes/Sec", new List<string>());
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_LIST.Add("Packets/Sec", new List<string>());
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO.CounterList = HYPERV_VIRTUAL_NETWORK_ADAPTER_LIST;


            //CustomCounterBasicInfo HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO.Group = "net-vna";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO.GroupLabel = "Bytes/Sec";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO.CounterKey = 32;
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO.Counter = "Bytes/Sec";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO.CounterLabel = "Bytes/Sec";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO.Instance = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO.UOM = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO.Singleton = true;
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO.WmiClass = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO.CounterList = HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_LIST;


            //CustomCounterBasicInfo HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO = new CustomCounterBasicInfo();
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO.Group = "net-vna";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO.GroupLabel = "Packets/Sec";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO.GroupSummary = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO.CounterKey = 33;
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO.Counter = "Packets/Sec";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO.CounterLabel = "Packets/Sec";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO.CounterSummary = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO.Instance = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO.UOM = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO.Singleton = true;
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO.WmiClass = "";
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO.CounterList = HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_LIST;

            //Memory : Memory
            MEMORY_INFO = new CustomCounterBasicInfo();
            MEMORY_INFO.Group = "mem";
            MEMORY_INFO.GroupLabel = "Memory";
            MEMORY_INFO.GroupSummary = "";
            MEMORY_INFO.CounterKey = 34;
            MEMORY_INFO.Counter = "";
            MEMORY_INFO.CounterLabel = "";
            MEMORY_INFO.CounterSummary = "";
            MEMORY_INFO.Instance = "";
            MEMORY_INFO.UOM = "";
            MEMORY_INFO.Singleton = true;
            Dictionary<string, List<string>> MEMORY_LIST = new Dictionary<string, List<string>>();
            MEMORY_LIST.Add("Type", new List<string>() { "VM", "Host" });
            MEMORY_LIST.Add("Available Bytes", new List<string>());
            MEMORY_LIST.Add("Pages / Sec", new List<string>());
            MEMORY_INFO.CounterList = MEMORY_LIST;

            CustomCounterBasicInfo MEMORY_AVAILABLE_BYTE_INFO = new CustomCounterBasicInfo();
            MEMORY_AVAILABLE_BYTE_INFO.Group = "mem";
            MEMORY_AVAILABLE_BYTE_INFO.GroupLabel = "Available Bytes";
            MEMORY_AVAILABLE_BYTE_INFO.GroupSummary = "";
            MEMORY_AVAILABLE_BYTE_INFO.CounterKey = 35;
            MEMORY_AVAILABLE_BYTE_INFO.Counter = "Available Bytes";
            MEMORY_AVAILABLE_BYTE_INFO.CounterLabel = "Available Bytes";
            MEMORY_AVAILABLE_BYTE_INFO.CounterSummary = "";
            MEMORY_AVAILABLE_BYTE_INFO.Instance = "";
            MEMORY_AVAILABLE_BYTE_INFO.UOM = "MB";
            MEMORY_AVAILABLE_BYTE_INFO.Singleton = true;
            MEMORY_AVAILABLE_BYTE_INFO.WmiClass = "Win32_PerfFormattedData_PerfOS_Memory";
            MEMORY_AVAILABLE_BYTE_INFO.SearcherKey = "AvailableMBytes";
            Dictionary<string, List<string>> MEMORY_AVAILABLE_BYTE_LIST = new Dictionary<string, List<string>>();
            MEMORY_AVAILABLE_BYTE_LIST.Add("Type", new List<string>() { "VM", "Host" });
            MEMORY_AVAILABLE_BYTE_INFO.CounterList = MEMORY_AVAILABLE_BYTE_LIST;

            CustomCounterBasicInfo MEMORY_PAGE_PER_SEC_INFO = new CustomCounterBasicInfo();
            MEMORY_PAGE_PER_SEC_INFO.Group = "mem";
            MEMORY_PAGE_PER_SEC_INFO.GroupLabel = "Pages / Sec";
            MEMORY_PAGE_PER_SEC_INFO.GroupSummary = "";
            MEMORY_PAGE_PER_SEC_INFO.CounterKey = 36;
            MEMORY_PAGE_PER_SEC_INFO.Counter = "Pages / Sec";
            MEMORY_PAGE_PER_SEC_INFO.CounterLabel = "Pages / Sec";
            MEMORY_PAGE_PER_SEC_INFO.CounterSummary = "";
            MEMORY_PAGE_PER_SEC_INFO.Instance = "";
            MEMORY_PAGE_PER_SEC_INFO.UOM = "";
            MEMORY_PAGE_PER_SEC_INFO.Singleton = true;
            MEMORY_PAGE_PER_SEC_INFO.WmiClass = "Win32_PerfFormattedData_PerfOS_Memory";
            MEMORY_PAGE_PER_SEC_INFO.SearcherKey = "PagesPerSec";
            Dictionary<string, List<string>> MEMORY_PAGE_PER_SEC_LIST = new Dictionary<string, List<string>>();
            MEMORY_PAGE_PER_SEC_LIST.Add("Type", new List<string>() { "VM", "Host" });
            MEMORY_PAGE_PER_SEC_INFO.CounterList = MEMORY_PAGE_PER_SEC_LIST;

            //Memory : HyperV Hypervisor Partition
            //HYPERV_HYPERVISIOR_PARTITION_INFO = new CustomCounterBasicInfo();
            //HYPERV_HYPERVISIOR_PARTITION_INFO.Group = "mem-hvisior";
            //HYPERV_HYPERVISIOR_PARTITION_INFO.GroupLabel = "Memory";
            //HYPERV_HYPERVISIOR_PARTITION_INFO.GroupSummary = "";
            //HYPERV_HYPERVISIOR_PARTITION_INFO.CounterKey = 37;
            //HYPERV_HYPERVISIOR_PARTITION_INFO.Counter = "";
            //HYPERV_HYPERVISIOR_PARTITION_INFO.CounterLabel = "";
            //HYPERV_HYPERVISIOR_PARTITION_INFO.CounterSummary = "";
            //HYPERV_HYPERVISIOR_PARTITION_INFO.Instance = "";
            //HYPERV_HYPERVISIOR_PARTITION_INFO.UOM = "";
            //HYPERV_HYPERVISIOR_PARTITION_INFO.Singleton = true;
            //Dictionary<string, List<string>> HYPERV_HYPERVISIOR_PARTITION_LIST = new Dictionary<string, List<string>>();
            //HYPERV_HYPERVISIOR_PARTITION_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_HYPERVISIOR_PARTITION_LIST.Add("1G GPA Pages", new List<string>());
            //HYPERV_HYPERVISIOR_PARTITION_LIST.Add("2M GPA Pages", new List<string>());
            //HYPERV_HYPERVISIOR_PARTITION_LIST.Add("Deposited Pages", new List<string>());
            //HYPERV_HYPERVISIOR_PARTITION_LIST.Add("Virtual Processors", new List<string>());
            //HYPERV_HYPERVISIOR_PARTITION_INFO.CounterList = HYPERV_HYPERVISIOR_PARTITION_LIST;

            //CustomCounterBasicInfo HYPERV_HYPERVISIOR_PARTITION_1GP_INFO = new CustomCounterBasicInfo();
            //HYPERV_HYPERVISIOR_PARTITION_1GP_INFO.Group = "mem-hvisior";
            //HYPERV_HYPERVISIOR_PARTITION_1GP_INFO.GroupLabel = "1G GPA Pages";
            //HYPERV_HYPERVISIOR_PARTITION_1GP_INFO.GroupSummary = "";
            //HYPERV_HYPERVISIOR_PARTITION_1GP_INFO.CounterKey = 38;
            //HYPERV_HYPERVISIOR_PARTITION_1GP_INFO.Counter = "1G GPA Pages";
            //HYPERV_HYPERVISIOR_PARTITION_1GP_INFO.CounterLabel = "1G GPA Pages";
            //HYPERV_HYPERVISIOR_PARTITION_1GP_INFO.CounterSummary = "";
            //HYPERV_HYPERVISIOR_PARTITION_1GP_INFO.Instance = "";
            //HYPERV_HYPERVISIOR_PARTITION_1GP_INFO.UOM = "";
            //HYPERV_HYPERVISIOR_PARTITION_1GP_INFO.Singleton = true;
            //HYPERV_HYPERVISIOR_PARTITION_1GP_INFO.WmiClass = "";
            //HYPERV_HYPERVISIOR_PARTITION_1GP_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_HYPERVISIOR_PARTITION_1GP_LIST = new Dictionary<string, List<string>>();
            //HYPERV_HYPERVISIOR_PARTITION_1GP_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_HYPERVISIOR_PARTITION_1GP_INFO.CounterList = HYPERV_HYPERVISIOR_PARTITION_1GP_LIST;

            //CustomCounterBasicInfo HYPERV_HYPERVISIOR_PARTITION_2GP_INFO = new CustomCounterBasicInfo();
            //HYPERV_HYPERVISIOR_PARTITION_2GP_INFO.Group = "mem-hvisior";
            //HYPERV_HYPERVISIOR_PARTITION_2GP_INFO.GroupLabel = "2M GPA Pages";
            //HYPERV_HYPERVISIOR_PARTITION_2GP_INFO.GroupSummary = "";
            //HYPERV_HYPERVISIOR_PARTITION_2GP_INFO.CounterKey = 39;
            //HYPERV_HYPERVISIOR_PARTITION_2GP_INFO.Counter = "2M GPA Pages";
            //HYPERV_HYPERVISIOR_PARTITION_2GP_INFO.CounterLabel = "2M GPA Pages";
            //HYPERV_HYPERVISIOR_PARTITION_2GP_INFO.CounterSummary = "";
            //HYPERV_HYPERVISIOR_PARTITION_2GP_INFO.Instance = "";
            //HYPERV_HYPERVISIOR_PARTITION_2GP_INFO.UOM = "";
            //HYPERV_HYPERVISIOR_PARTITION_2GP_INFO.Singleton = true;
            //HYPERV_HYPERVISIOR_PARTITION_2GP_INFO.WmiClass = "";
            //HYPERV_HYPERVISIOR_PARTITION_2GP_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_HYPERVISIOR_PARTITION_2GP_LIST = new Dictionary<string, List<string>>();
            //HYPERV_HYPERVISIOR_PARTITION_2GP_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_HYPERVISIOR_PARTITION_2GP_INFO.CounterList = HYPERV_HYPERVISIOR_PARTITION_2GP_LIST;

            //CustomCounterBasicInfo HYPERV_HYPERVISIOR_PARTITION_DP_INFO = new CustomCounterBasicInfo();
            //HYPERV_HYPERVISIOR_PARTITION_DP_INFO.Group = "mem-hvisior";
            //HYPERV_HYPERVISIOR_PARTITION_DP_INFO.GroupLabel = "Deposited Pages";
            //HYPERV_HYPERVISIOR_PARTITION_DP_INFO.GroupSummary = "";
            //HYPERV_HYPERVISIOR_PARTITION_DP_INFO.CounterKey = 40;
            //HYPERV_HYPERVISIOR_PARTITION_DP_INFO.Counter = "Deposited Pages";
            //HYPERV_HYPERVISIOR_PARTITION_DP_INFO.CounterLabel = "Deposited Pages";
            //HYPERV_HYPERVISIOR_PARTITION_DP_INFO.CounterSummary = "";
            //HYPERV_HYPERVISIOR_PARTITION_DP_INFO.Instance = "";
            //HYPERV_HYPERVISIOR_PARTITION_DP_INFO.UOM = "";
            //HYPERV_HYPERVISIOR_PARTITION_DP_INFO.Singleton = true;
            //HYPERV_HYPERVISIOR_PARTITION_DP_INFO.WmiClass = "";
            //HYPERV_HYPERVISIOR_PARTITION_DP_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_HYPERVISIOR_PARTITION_DP_LIST = new Dictionary<string, List<string>>();
            //HYPERV_HYPERVISIOR_PARTITION_DP_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_HYPERVISIOR_PARTITION_DP_INFO.CounterList = HYPERV_HYPERVISIOR_PARTITION_DP_LIST;

            //CustomCounterBasicInfo HYPERV_HYPERVISIOR_PARTITION_VP_INFO = new CustomCounterBasicInfo();
            //HYPERV_HYPERVISIOR_PARTITION_VP_INFO.Group = "mem-hvisior";
            //HYPERV_HYPERVISIOR_PARTITION_VP_INFO.GroupLabel = "Virtual Processors";
            //HYPERV_HYPERVISIOR_PARTITION_VP_INFO.GroupSummary = "";
            //HYPERV_HYPERVISIOR_PARTITION_VP_INFO.CounterKey = 41;
            //HYPERV_HYPERVISIOR_PARTITION_VP_INFO.Counter = "Virtual Processors";
            //HYPERV_HYPERVISIOR_PARTITION_VP_INFO.CounterLabel = "Virtual Processors";
            //HYPERV_HYPERVISIOR_PARTITION_VP_INFO.CounterSummary = "";
            //HYPERV_HYPERVISIOR_PARTITION_VP_INFO.Instance = "";
            //HYPERV_HYPERVISIOR_PARTITION_VP_INFO.UOM = "";
            //HYPERV_HYPERVISIOR_PARTITION_VP_INFO.Singleton = true;
            //HYPERV_HYPERVISIOR_PARTITION_VP_INFO.WmiClass = "";
            //HYPERV_HYPERVISIOR_PARTITION_VP_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_HYPERVISIOR_PARTITION_VP_LIST = new Dictionary<string, List<string>>();
            //HYPERV_HYPERVISIOR_PARTITION_VP_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_HYPERVISIOR_PARTITION_VP_INFO.CounterList = HYPERV_HYPERVISIOR_PARTITION_VP_LIST;

            //Memory : HyperV Root Partition
            //HYPERV_ROOT_PARTITION_INFO = new CustomCounterBasicInfo();
            //HYPERV_ROOT_PARTITION_INFO.Group = "mem-root";
            //HYPERV_ROOT_PARTITION_INFO.GroupLabel = "Memory";
            //HYPERV_ROOT_PARTITION_INFO.GroupSummary = "";
            //HYPERV_ROOT_PARTITION_INFO.CounterKey = 42;
            //HYPERV_ROOT_PARTITION_INFO.Counter = "";
            //HYPERV_ROOT_PARTITION_INFO.CounterLabel = "";
            //HYPERV_ROOT_PARTITION_INFO.CounterSummary = "";
            //HYPERV_ROOT_PARTITION_INFO.Instance = "";
            //HYPERV_ROOT_PARTITION_INFO.UOM = "";
            //HYPERV_ROOT_PARTITION_INFO.Singleton = true;
            //Dictionary<string, List<string>> HYPERV_ROOT_PARTITION_LIST = new Dictionary<string, List<string>>();
            //HYPERV_ROOT_PARTITION_LIST.Add("Type", new List<string>() { "Host" });
            //HYPERV_ROOT_PARTITION_LIST.Add("1G GPA Pages", new List<string>());
            //HYPERV_ROOT_PARTITION_LIST.Add("2M GPA Pages", new List<string>());
            //HYPERV_ROOT_PARTITION_LIST.Add("Deposited Pages", new List<string>());
            //HYPERV_ROOT_PARTITION_LIST.Add("Virtual Processors", new List<string>());
            //HYPERV_ROOT_PARTITION_INFO.CounterList = HYPERV_ROOT_PARTITION_LIST;

            //CustomCounterBasicInfo HYPERV_ROOT_PARTITION_1GP_INFO = new CustomCounterBasicInfo();
            //HYPERV_ROOT_PARTITION_1GP_INFO.Group = "mem-root";
            //HYPERV_ROOT_PARTITION_1GP_INFO.GroupLabel = "1G GPA Pages";
            //HYPERV_ROOT_PARTITION_1GP_INFO.GroupSummary = "";
            //HYPERV_ROOT_PARTITION_1GP_INFO.CounterKey = 43;
            //HYPERV_ROOT_PARTITION_1GP_INFO.Counter = "1G GPA Pages";
            //HYPERV_ROOT_PARTITION_1GP_INFO.CounterLabel = "1G GPA Pages";
            //HYPERV_ROOT_PARTITION_1GP_INFO.CounterSummary = "";
            //HYPERV_ROOT_PARTITION_1GP_INFO.Instance = "";
            //HYPERV_ROOT_PARTITION_1GP_INFO.UOM = "";
            //HYPERV_ROOT_PARTITION_1GP_INFO.Singleton = true;
            //HYPERV_ROOT_PARTITION_1GP_INFO.WmiClass = "";
            //HYPERV_ROOT_PARTITION_1GP_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_ROOT_PARTITION_1GP_LIST = new Dictionary<string, List<string>>();
            //HYPERV_ROOT_PARTITION_1GP_LIST.Add("Type", new List<string>() { "Host" });
            //HYPERV_ROOT_PARTITION_1GP_INFO.CounterList = HYPERV_ROOT_PARTITION_1GP_LIST;

            //CustomCounterBasicInfo HYPERV_ROOT_PARTITION_2GP_INFO = new CustomCounterBasicInfo();
            //HYPERV_ROOT_PARTITION_2GP_INFO.Group = "mem-root";
            //HYPERV_ROOT_PARTITION_2GP_INFO.GroupLabel = "2M GPA Pages";
            //HYPERV_ROOT_PARTITION_2GP_INFO.GroupSummary = "";
            //HYPERV_ROOT_PARTITION_2GP_INFO.CounterKey = 44;
            //HYPERV_ROOT_PARTITION_2GP_INFO.Counter = "2M GPA Pages";
            //HYPERV_ROOT_PARTITION_2GP_INFO.CounterLabel = "2M GPA Pages";
            //HYPERV_ROOT_PARTITION_2GP_INFO.CounterSummary = "";
            //HYPERV_ROOT_PARTITION_2GP_INFO.Instance = "";
            //HYPERV_ROOT_PARTITION_2GP_INFO.UOM = "";
            //HYPERV_ROOT_PARTITION_2GP_INFO.Singleton = true;
            //HYPERV_ROOT_PARTITION_2GP_INFO.WmiClass = "";
            //HYPERV_ROOT_PARTITION_2GP_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_ROOT_PARTITION_2GP_LIST = new Dictionary<string, List<string>>();
            //HYPERV_ROOT_PARTITION_2GP_LIST.Add("Type", new List<string>() { "Host" });
            //HYPERV_ROOT_PARTITION_2GP_INFO.CounterList = HYPERV_ROOT_PARTITION_2GP_LIST;

            //CustomCounterBasicInfo HYPERV_ROOT_PARTITION_DP_INFO = new CustomCounterBasicInfo();
            //HYPERV_ROOT_PARTITION_DP_INFO.Group = "mem-root";
            //HYPERV_ROOT_PARTITION_DP_INFO.GroupLabel = "Deposited Pages";
            //HYPERV_ROOT_PARTITION_DP_INFO.GroupSummary = "";
            //HYPERV_ROOT_PARTITION_DP_INFO.CounterKey = 45;
            //HYPERV_ROOT_PARTITION_DP_INFO.Counter = "Deposited Pages";
            //HYPERV_ROOT_PARTITION_DP_INFO.CounterLabel = "Deposited Pages";
            //HYPERV_ROOT_PARTITION_DP_INFO.CounterSummary = "";
            //HYPERV_ROOT_PARTITION_DP_INFO.Instance = "";
            //HYPERV_ROOT_PARTITION_DP_INFO.UOM = "";
            //HYPERV_ROOT_PARTITION_DP_INFO.Singleton = true;
            //HYPERV_ROOT_PARTITION_DP_INFO.WmiClass = "";
            //HYPERV_ROOT_PARTITION_DP_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_ROOT_PARTITION_DP_LIST = new Dictionary<string, List<string>>();
            //HYPERV_ROOT_PARTITION_DP_LIST.Add("Type", new List<string>() { "Host" });
            //HYPERV_ROOT_PARTITION_DP_INFO.CounterList = HYPERV_ROOT_PARTITION_DP_LIST;

            //CustomCounterBasicInfo HYPERV_ROOT_PARTITION_VP_INFO = new CustomCounterBasicInfo();
            //HYPERV_ROOT_PARTITION_VP_INFO.Group = "mem-root";
            //HYPERV_ROOT_PARTITION_VP_INFO.GroupLabel = "Virtual Processors";
            //HYPERV_ROOT_PARTITION_VP_INFO.GroupSummary = "";
            //HYPERV_ROOT_PARTITION_VP_INFO.CounterKey = 46;
            //HYPERV_ROOT_PARTITION_VP_INFO.Counter = "Virtual Processors";
            //HYPERV_ROOT_PARTITION_VP_INFO.CounterLabel = "Virtual Processors";
            //HYPERV_ROOT_PARTITION_VP_INFO.CounterSummary = "";
            //HYPERV_ROOT_PARTITION_VP_INFO.Instance = "";
            //HYPERV_ROOT_PARTITION_VP_INFO.UOM = "";
            //HYPERV_ROOT_PARTITION_VP_INFO.Singleton = true;
            //HYPERV_ROOT_PARTITION_VP_INFO.WmiClass = "";
            //HYPERV_ROOT_PARTITION_VP_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_ROOT_PARTITION_VP_LIST = new Dictionary<string, List<string>>();
            //HYPERV_ROOT_PARTITION_VP_LIST.Add("Type", new List<string>() { "Host" });
            //HYPERV_ROOT_PARTITION_VP_INFO.CounterList = HYPERV_ROOT_PARTITION_VP_LIST;

            //Memory : HyperV VM Vid Partition
            //HYPERV_VM_VID_PARTITION_INFO = new CustomCounterBasicInfo();
            //HYPERV_VM_VID_PARTITION_INFO.Group = "mem-vid";
            //HYPERV_VM_VID_PARTITION_INFO.GroupLabel = "Memory";
            //HYPERV_VM_VID_PARTITION_INFO.GroupSummary = "";
            //HYPERV_VM_VID_PARTITION_INFO.CounterKey = 47;
            //HYPERV_VM_VID_PARTITION_INFO.Counter = "";
            //HYPERV_VM_VID_PARTITION_INFO.CounterLabel = "";
            //HYPERV_VM_VID_PARTITION_INFO.CounterSummary = "";
            //HYPERV_VM_VID_PARTITION_INFO.Instance = "";
            //HYPERV_VM_VID_PARTITION_INFO.UOM = "";
            //HYPERV_VM_VID_PARTITION_INFO.Singleton = true;
            //HYPERV_VM_VID_PARTITION_INFO.WmiClass = "";
            //HYPERV_VM_VID_PARTITION_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VM_VID_PARTITION_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VM_VID_PARTITION_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_VM_VID_PARTITION_LIST.Add("Physical Pages Allocated", new List<string>());
            //HYPERV_VM_VID_PARTITION_LIST.Add("Remote Physical Pages", new List<string>());
            //HYPERV_VM_VID_PARTITION_INFO.CounterList = HYPERV_VM_VID_PARTITION_LIST;

            //CustomCounterBasicInfo HYPERV_VM_VID_PARTITION_PPA_INFO = new CustomCounterBasicInfo();
            //HYPERV_VM_VID_PARTITION_PPA_INFO.Group = "mem-vid";
            //HYPERV_VM_VID_PARTITION_PPA_INFO.GroupLabel = "Physical Pages Allocated";
            //HYPERV_VM_VID_PARTITION_PPA_INFO.GroupSummary = "";
            //HYPERV_VM_VID_PARTITION_PPA_INFO.CounterKey = 48;
            //HYPERV_VM_VID_PARTITION_PPA_INFO.Counter = "Physical Pages Allocated";
            //HYPERV_VM_VID_PARTITION_PPA_INFO.CounterLabel = "Physical Pages Allocated";
            //HYPERV_VM_VID_PARTITION_PPA_INFO.CounterSummary = "";
            //HYPERV_VM_VID_PARTITION_PPA_INFO.Instance = "";
            //HYPERV_VM_VID_PARTITION_PPA_INFO.UOM = "";
            //HYPERV_VM_VID_PARTITION_PPA_INFO.Singleton = true;
            //HYPERV_VM_VID_PARTITION_PPA_INFO.WmiClass = "";
            //HYPERV_VM_VID_PARTITION_PPA_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VM_VID_PARTITION_PPA_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VM_VID_PARTITION_PPA_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_VM_VID_PARTITION_PPA_INFO.CounterList = HYPERV_VM_VID_PARTITION_PPA_LIST;

            //CustomCounterBasicInfo HYPERV_VM_VID_PARTITION_RPP_INFO = new CustomCounterBasicInfo();
            //HYPERV_VM_VID_PARTITION_RPP_INFO.Group = "mem-vid";
            //HYPERV_VM_VID_PARTITION_RPP_INFO.GroupLabel = "Remote Physical Pages";
            //HYPERV_VM_VID_PARTITION_RPP_INFO.GroupSummary = "";
            //HYPERV_VM_VID_PARTITION_RPP_INFO.CounterKey = 49;
            //HYPERV_VM_VID_PARTITION_RPP_INFO.Counter = "Remote Physical Pages";
            //HYPERV_VM_VID_PARTITION_RPP_INFO.CounterLabel = "Remote Physical Pages";
            //HYPERV_VM_VID_PARTITION_RPP_INFO.CounterSummary = "";
            //HYPERV_VM_VID_PARTITION_RPP_INFO.Instance = "";
            //HYPERV_VM_VID_PARTITION_RPP_INFO.UOM = "";
            //HYPERV_VM_VID_PARTITION_RPP_INFO.Singleton = true;
            //HYPERV_VM_VID_PARTITION_RPP_INFO.WmiClass = "";
            //HYPERV_VM_VID_PARTITION_RPP_INFO.SearcherKey = "";
            //Dictionary<string, List<string>> HYPERV_VM_VID_PARTITION_RPP_LIST = new Dictionary<string, List<string>>();
            //HYPERV_VM_VID_PARTITION_RPP_LIST.Add("Type", new List<string>() { "VM" });
            //HYPERV_VM_VID_PARTITION_RPP_INFO.CounterList = HYPERV_VM_VID_PARTITION_RPP_LIST;

            //Processor : Processor / CPU
            PROCESSOR_INFO = new CustomCounterBasicInfo();
            PROCESSOR_INFO.Group = "cpu";
            PROCESSOR_INFO.GroupLabel = "CPU";
            PROCESSOR_INFO.GroupSummary = "";
            PROCESSOR_INFO.CounterKey = 50;
            PROCESSOR_INFO.Counter = "";
            PROCESSOR_INFO.CounterLabel = "";
            PROCESSOR_INFO.CounterSummary = "";
            PROCESSOR_INFO.Instance = "";
            PROCESSOR_INFO.UOM = "";
            PROCESSOR_INFO.Singleton = true;
            Dictionary<string, List<string>> PROCESSOR_LIST = new Dictionary<string, List<string>>();
            PROCESSOR_LIST.Add("Type", new List<string>() { "VM", "Host" });
            PROCESSOR_LIST.Add("DPCs Queued / sec", new List<string>());
            PROCESSOR_LIST.Add("Interrupts / sec", new List<string>());
            PROCESSOR_LIST.Add("% Processor Usage", new List<string>());
            PROCESSOR_INFO.CounterList = PROCESSOR_LIST;

            CustomCounterBasicInfo PROCESSOR_DPCS_QUEUED_PER_SEC_INFO = new CustomCounterBasicInfo();
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.Group = "cpu";
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.GroupLabel = "DPCs Queued / sec";
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.GroupSummary = "";
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.CounterKey = 51;
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.Counter = "DPCs Queued / sec";
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.CounterLabel = "DPCs Queued / sec";
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.CounterSummary = "";
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.Instance = "";
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.UOM = "";
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.Singleton = true;
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.WmiClass = "Win32_PerfFormattedData_Counters_ProcessorInformation";
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.SearcherKey = "DPCsQueuedPersec";
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.WhereClause = "Name = '_Total'";
            Dictionary<string, List<string>> PROCESSOR_DPCS_QUEUED_PER_SEC_LIST = new Dictionary<string, List<string>>();
            PROCESSOR_DPCS_QUEUED_PER_SEC_LIST.Add("Type", new List<string>() { "VM", "Host" });
            PROCESSOR_DPCS_QUEUED_PER_SEC_INFO.CounterList = PROCESSOR_DPCS_QUEUED_PER_SEC_LIST;

            CustomCounterBasicInfo PROCESSOR_INTERRUPTS_PER_SEC_INFO = new CustomCounterBasicInfo();
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.Group = "cpu";
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.GroupLabel = "Interrupts / sec";
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.GroupSummary = "";
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.CounterKey = 52;
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.Counter = "Interrupts / sec";
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.CounterLabel = "Interrupts / sec";
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.CounterSummary = "";
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.Instance = "";
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.UOM = "";
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.Singleton = true;
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.WmiClass = "Win32_PerfFormattedData_Counters_ProcessorInformation";
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.SearcherKey = "InterruptsPersec";
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.WhereClause = "Name = '_Total'";
            Dictionary<string, List<string>> PROCESSOR_INTERRUPTS_PER_SEC_LIST = new Dictionary<string, List<string>>();
            PROCESSOR_INTERRUPTS_PER_SEC_LIST.Add("Type", new List<string>() { "VM", "Host" });
            PROCESSOR_INTERRUPTS_PER_SEC_INFO.CounterList = PROCESSOR_INTERRUPTS_PER_SEC_LIST;

            CustomCounterBasicInfo PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO = new CustomCounterBasicInfo();
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.Group = "cpu";
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.GroupLabel = "% Processor Usage";
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.GroupSummary = "";
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.CounterKey = 53;
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.Counter = "% Processor Usage";
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.CounterLabel = "% Processor Usage";
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.CounterSummary = "";
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.Instance = "";
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.UOM = "";
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.Singleton = true;
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.WmiClass = "Win32_PerfFormattedData_Counters_ProcessorInformation";
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.SearcherKey = "PercentProcessorPerformance";
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.WhereClause = "Name = '_Total'";
            Dictionary<string, List<string>> PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_LIST = new Dictionary<string, List<string>>();
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_LIST.Add("Type", new List<string>() { "VM", "Host" });
            PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO.CounterList = PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_LIST;

            //fill main list
            //hyperVCustomCounterObjects.Add("HyperV Virtual Machine Health Summary", HYPERV_VIRTUAL_MACHINE_HEALTH_SUMMARY_INFO);
            //hyperVCustomCounterObjects.Add("HyperV Hypervisor", HYPERV_HYPERVISOR_INFO);
            hyperVCustomCounterObjects.Add("CPU", PROCESSOR_INFO);
            hyperVCustomCounterObjects.Add("Memory", MEMORY_INFO);
            //hyperVCustomCounterObjects.Add("HyperV Hypervisor Partition",HYPERV_HYPERVISIOR_PARTITION_INFO);
            //hyperVCustomCounterObjects.Add("HyperV Root Partition",HYPERV_ROOT_PARTITION_INFO);
            //hyperVCustomCounterObjects.Add("HyperV VM Vid Partition",HYPERV_VM_VID_PARTITION_INFO);
            hyperVCustomCounterObjects.Add("Network Interface", NETWORK_INTERFACE_INFO);
            //hyperVCustomCounterObjects.Add("HyperV Virtual Switch",HYPERV_VIRTUAL_SWITCH_INFO);
            //hyperVCustomCounterObjects.Add("HyperV Legacy Network Adapter",HYPERV_LEGACY_NETWORK_ADAPTER_INFO);
            //hyperVCustomCounterObjects.Add("HyperV Virtual Network Adapter",HYPERV_VIRTUAL_NETWORK_ADAPTER_INFO);
            hyperVCustomCounterObjects.Add("Physical Disk", PHYSICAL_DISK_INFO);
            hyperVCustomCounterObjects.Add("HyperV Virtual Storage Device", HYPERV_VIRTUAL_STORAGE_DEVICE_INFO);
            hyperVCustomCounterObjects.Add("HyperV Virtual IDE Controller", HYPERV_VIRTUAL_IDE_CONTROLLER_INFO);
            //hyperVCustomCounterObjects.Add("Health Ok", HYPERV_VIRTUAL_MACHINE_HEALTH_OK_SUMMARY_INFO);
            //hyperVCustomCounterObjects.Add("Health Critical", HYPERV_VIRTUAL_MACHINE_HEALTH_CRI_SUMMARY_INFO);
            hyperVCustomCounterObjects.Add("Current Disk Queue Length", PHYSICAL_DISK_Q_LEN_INFO);
            hyperVCustomCounterObjects.Add("Disk Bytes / Sec", PHYSICAL_DISK_BPS_INFO);
            hyperVCustomCounterObjects.Add("Disk Transfers / Sec", PHYSICAL_DISK_TPS_INFO);
            //hyperVCustomCounterObjects.Add("Error Count", HYPERV_VIRTUAL_STORAGE_DEVICE_EC_INFO);
            //hyperVCustomCounterObjects.Add("Flush Count", HYPERV_VIRTUAL_STORAGE_DEVICE_FC_INFO);
            hyperVCustomCounterObjects.Add("Read Bytes / Sec vs", HYPERV_VIRTUAL_STORAGE_DEVICE_RBPS_INFO);
            hyperVCustomCounterObjects.Add("Write Bytes / Sec vs", HYPERV_VIRTUAL_STORAGE_DEVICE_WBPS_INFO);
            //hyperVCustomCounterObjects.Add("Read Count", HYPERV_VIRTUAL_STORAGE_DEVICE_RC_INFO);
            //hyperVCustomCounterObjects.Add("Write Count", HYPERV_VIRTUAL_STORAGE_DEVICE_WC_INFO);
            hyperVCustomCounterObjects.Add("Read Bytes / Sec ide", HYPERV_VIRTUAL_IDE_CONTROLLER_RBPS_INFO);
            hyperVCustomCounterObjects.Add("Write Bytes / Sec ide", HYPERV_VIRTUAL_IDE_CONTROLLER_WBPS_INFO);
            //hyperVCustomCounterObjects.Add("Read Sectors / Sec", HYPERV_VIRTUAL_IDE_CONTROLLER_RSPS_INFO);
            //hyperVCustomCounterObjects.Add("Write Sectors / Sec", HYPERV_VIRTUAL_IDE_CONTROLLER_WSPS_INFO);
            hyperVCustomCounterObjects.Add("Bytes Sent / Sec", NETWORK_INTERFACE_BTSPS_INFO);
            hyperVCustomCounterObjects.Add("Bytes Received / Sec", NETWORK_INTERFACE_BTRPS_INFO);
            hyperVCustomCounterObjects.Add("Packets Sent / Sec", NETWORK_INTERFACE_PSPS_INFO);
            hyperVCustomCounterObjects.Add("Packets Received / Sec", NETWORK_INTERFACE_PRPS_INFO);
            hyperVCustomCounterObjects.Add("Packets Outbound Errors", NETWORK_INTERFACE_POE_INFO);
            hyperVCustomCounterObjects.Add("Packets Receive Errors", NETWORK_INTERFACE_PRE_INFO);
            //hyperVCustomCounterObjects.Add("Bytes/Sec vs", HYPERV_VIRTUAL_SWITCH_BPS_INFO);
            //hyperVCustomCounterObjects.Add("Packets/Sec vs", HYPERV_VIRTUAL_SWITCH_PPS_INFO);
            //hyperVCustomCounterObjects.Add("Bytes Sent / Sec", HYPERV_LEGACY_NETWORK_ADAPTER_BSPS_INFO);
            //hyperVCustomCounterObjects.Add("Bytes Received / Sec", HYPERV_LEGACY_NETWORK_ADAPTER_BRPS_INFO);
            //hyperVCustomCounterObjects.Add("Bytes/Sec vna", HYPERV_VIRTUAL_NETWORK_ADAPTER_BPS_INFO);
            //hyperVCustomCounterObjects.Add("Packets/Sec vna", HYPERV_VIRTUAL_NETWORK_ADAPTER_PPS_INFO);
            hyperVCustomCounterObjects.Add("Available Bytes", MEMORY_AVAILABLE_BYTE_INFO);
            hyperVCustomCounterObjects.Add("Pages / Sec", MEMORY_PAGE_PER_SEC_INFO);
            //hyperVCustomCounterObjects.Add("1G GPA Pages VM", HYPERV_HYPERVISIOR_PARTITION_1GP_INFO);
            //hyperVCustomCounterObjects.Add("2M GPA Pages VM ", HYPERV_HYPERVISIOR_PARTITION_2GP_INFO);
            //hyperVCustomCounterObjects.Add("Deposited Pages VM", HYPERV_HYPERVISIOR_PARTITION_DP_INFO);
            //hyperVCustomCounterObjects.Add("Virtual Processors VM", HYPERV_HYPERVISIOR_PARTITION_VP_INFO);
            //hyperVCustomCounterObjects.Add("1G GPA Pages host", HYPERV_ROOT_PARTITION_1GP_INFO);
            //hyperVCustomCounterObjects.Add("2M GPA Pages host", HYPERV_ROOT_PARTITION_2GP_INFO);
            //hyperVCustomCounterObjects.Add("Deposited Pages host", HYPERV_ROOT_PARTITION_DP_INFO);
            //hyperVCustomCounterObjects.Add("Virtual Processors host", HYPERV_ROOT_PARTITION_VP_INFO);
            //hyperVCustomCounterObjects.Add("Physical Pages Allocated", HYPERV_VM_VID_PARTITION_PPA_INFO);
            //hyperVCustomCounterObjects.Add("Remote Physical Pages", HYPERV_VM_VID_PARTITION_RPP_INFO);
            hyperVCustomCounterObjects.Add("DPCs Queued / sec", PROCESSOR_DPCS_QUEUED_PER_SEC_INFO);
            hyperVCustomCounterObjects.Add("Interrupts / sec", PROCESSOR_INTERRUPTS_PER_SEC_INFO);
            hyperVCustomCounterObjects.Add("% Processor Usage", PROCESSOR_PERCENTAGE_PROCESSOR_USAGE_INFO);


        }

    }

    public class CustomCounterBasicInfo
    {
        private string group;
        private string groupLabel;
        private string groupSummary;
        private int counterKey;
        private string counter;
        private string counterLabel;
        private string counterSummary;
        private string instance;
        private string uom;
        private bool singleton;
        private string wmiclass;
        private string searcherKey;
        private string whereClause = "";
        private Dictionary<string, List<string>> counterList;

        public Dictionary<string, List<string>> CounterList
        {
            get { return counterList; }
            set { counterList = value; }
        }

        public string Group
        {
            get { return group; }
            set { group = value; }
        }

        public string WhereClause
        {
            get { return whereClause; }
            set { whereClause = value; }
        }

        public string GroupLabel
        {
            get { return groupLabel; }
            set { groupLabel = value; }
        }

        public string GroupSummary
        {
            get { return groupSummary; }
            set { groupSummary = value; }
        }

        public int CounterKey
        {
            get { return counterKey; }
            set { counterKey = value; }
        }

        public string Counter
        {
            get { return counter; }
            set { counter = value; }
        }

        public string CounterLabel
        {
            get { return counterLabel; }
            set { counterLabel = value; }
        }

        public string CounterSummary
        {
            get { return counterSummary; }
            set { counterSummary = value; }
        }


        public string Instance
        {
            get { return instance; }
            set { instance = value; }
        }

        public string UOM
        {
            get { return uom; }
            set { uom = value; }
        }

        public string WmiClass
        {
            get { return wmiclass; }
            set { wmiclass = value; }
        }

        public string SearcherKey
        {
            get { return searcherKey; }
            set { searcherKey = value; }
        }

        public bool Singleton
        {
            get { return singleton; }
            set { singleton = value; }
        }


    }


}
