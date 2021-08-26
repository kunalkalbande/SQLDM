using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WMI
{
    public class WMINetworkInterface
    {
        public string Name { get; private set; }
        public UInt64 CurrentBandwidth { get; private set; }
        public UInt32 OutputQueueLength { get; private set; }
        public UInt32 PacketsPerSec { get; private set; }
        public UInt32 PacketsReceivedErrors { get; private set; }
        public UInt32 PacketsOutboundErrors { get; private set; }
        public UInt64 BytesTotalPerSec { get; private set; }
        public UInt64 Frequency_Sys100NS { get; private set; }
        public UInt64 Timestamp_Sys100NS { get; private set; }

        private WMINetworkInterface() { }
        public WMINetworkInterface(WMIObjectProperties props)
        {
            Name = props.GetString("Name");
            CurrentBandwidth = props.GetUInt64("CurrentBandwidth");
            PacketsPerSec = props.GetUInt32("PacketsPerSec");
            OutputQueueLength = props.GetUInt32("OutputQueueLength");
            BytesTotalPerSec = props.GetUInt64("BytesTotalPerSec");
            PacketsReceivedErrors = props.GetUInt32("PacketsReceivedErrors");
            PacketsOutboundErrors = props.GetUInt32("PacketsOutboundErrors");
            Frequency_Sys100NS = props.GetUInt64("Frequency_Sys100NS");
            Timestamp_Sys100NS = props.GetUInt64("Timestamp_Sys100NS");
        }

        internal static string[] GetPropNames()
        {
            return (new string[] { 
                                    "Name", 
                                    "CurrentBandwidth", 
                                    "Frequency_Sys100NS",
                                    "Timestamp_Sys100NS",
                                    "OutputQueueLength", 
                                    "PacketsPerSec",
                                    "PacketsReceivedErrors", 
                                    "PacketsOutboundErrors", 
                                    "BytesTotalPerSec" 
                                    });
        }
    }
}
