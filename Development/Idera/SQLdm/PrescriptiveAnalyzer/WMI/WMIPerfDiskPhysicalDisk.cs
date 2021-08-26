using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIPerfDiskPhysicalDisk
    {
        public string Name { get; private set; }
        public UInt32 DiskTransfersPerSec { get; private set; }
        public UInt64 Frequency_Sys100NS { get; private set; }
        public UInt64 Timestamp_Sys100NS { get; private set; }
        
        private WMIPerfDiskPhysicalDisk() { }
        public WMIPerfDiskPhysicalDisk(WMIObjectProperties props)
        {
            Name = props.GetString("Name");
            DiskTransfersPerSec = props.GetUInt32("DiskTransfersPerSec");
            Frequency_Sys100NS = props.GetUInt64("Frequency_Sys100NS");
            Timestamp_Sys100NS = props.GetUInt64("Timestamp_Sys100NS");
        }

        internal static string[] GetPropNames()
        {
            return (new string[] {  "Name", 
                                    "Frequency_Sys100NS",
                                    "Timestamp_Sys100NS", 
                                    "DiskTransfersPerSec"
                                 });
        }
    }
}
