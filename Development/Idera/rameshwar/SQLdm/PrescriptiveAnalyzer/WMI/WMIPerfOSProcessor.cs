using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WMI
{
    internal class WMIPerfOSProcessor
    {
        public string Name { get; private set; }
        public UInt32 InterruptsPerSec { get; private set; }
        public UInt64 PercentProcessorTime { get; private set; }
        public UInt64 PercentPrivilegedTime { get; private set; }
        public UInt64 PercentInterruptTime { get; private set; }
        public UInt64 Frequency_Sys100NS { get; private set; }
        public UInt64 Timestamp_Sys100NS { get; private set; }
        private WMIPerfOSProcessor() { }
        public WMIPerfOSProcessor(WMIObjectProperties props)
        {
            Name = props.GetString("Name");
            InterruptsPerSec = props.GetUInt32("InterruptsPerSec");
            PercentProcessorTime = props.GetUInt64("PercentProcessorTime");
            PercentPrivilegedTime = props.GetUInt64("PercentPrivilegedTime");
            PercentInterruptTime = props.GetUInt64("PercentInterruptTime");
            Frequency_Sys100NS = props.GetUInt64("Frequency_Sys100NS");
            Timestamp_Sys100NS = props.GetUInt64("Timestamp_Sys100NS");
        }

        internal static string[] GetPropNames()
        {
            return (new string[] { 
                                    "Name", 
                                    "InterruptsPerSec",
                                    "PercentProcessorTime", 
                                    "PercentPrivilegedTime",
                                    "PercentInterruptTime",
                                    "Frequency_Sys100NS",
                                    "Timestamp_Sys100NS" 
                                    });
        }
    }
}
