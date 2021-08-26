using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TracerX;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIPerfDiskLogicalDisk
    {
        private static Logger _logX = Logger.GetLogger("WMIPerfDiskLogicalDisk");
        public string Name { get; private set; }
        public UInt64 AvgDiskQueueLength { get; private set; }
        public UInt32 CurrentDiskQueueLength { get; private set; }
        public UInt32 DiskTransfersPerSecond { get; private set; }
        public UInt32 SplitIOPerSec { get; private set; }
        public UInt64 PercentDiskTime { get; private set; }
        public UInt64 PercentDiskTimeBase { get; private set; }
        public UInt32 AvgDisksecPerTransfer { get; private set; }
        public UInt32 AvgDisksecPerTransfer_Base { get; private set; }
        public UInt64 Timestamp_Sys100NS { get; private set; }
        
        private WMIPerfDiskLogicalDisk() { }
        public WMIPerfDiskLogicalDisk(WMIObjectProperties props)
        {
            Name = props.GetString("Name");
            AvgDiskQueueLength = props.GetUInt64("AvgDiskQueueLength");
            CurrentDiskQueueLength = props.GetUInt32("CurrentDiskQueueLength");
            SplitIOPerSec = props.GetUInt32("SplitIOPerSec");
            PercentDiskTime = props.GetUInt64("PercentDiskTime");
            PercentDiskTimeBase = props.GetUInt64("PercentDiskTime_Base");
            AvgDisksecPerTransfer = props.GetUInt32("AvgDisksecPerTransfer");
            AvgDisksecPerTransfer_Base = props.GetUInt32("AvgDisksecPerTransfer_Base");
            DiskTransfersPerSecond = props.GetUInt32("DiskTransfersPerSec");
            Timestamp_Sys100NS = props.GetUInt64("Timestamp_Sys100NS");

            _logX.DebugFormat("Disk {0} disk queue length sample is {1}", Name, CurrentDiskQueueLength);
        }

        internal static string[] GetPropNames()
        {
            return (new string[] {  "Name", 
                                    "Timestamp_Sys100NS", 
                                    "SplitIOPerSec", 
                                    "AvgDiskQueueLength",
                                    "CurrentDiskQueueLength",
                                    "DiskTransfersPerSec",
                                    "PercentDiskTime", 
                                    "PercentDiskTime_Base", 
                                    "AvgDisksecPerTransfer", 
                                    "AvgDisksecPerTransfer_Base" 
                                 });
        }
    }
}
