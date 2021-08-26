using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIProcessor
    {
        public UInt32 AddressWidth { get; private set; }
        public UInt32 NumberOfLogicalProcessors { get; private set; }
        public UInt32 NumberOfCores { get; private set; }
        public UInt32 CurrentClockSpeed { get; private set; }
        public UInt32 MaxClockSpeed { get; private set; }
        private WMIProcessor() { }
        public WMIProcessor(WMIObjectProperties props)
        {
            NumberOfLogicalProcessors = props.GetUInt32("NumberOfLogicalProcessors");
            NumberOfCores = props.GetUInt32("NumberOfCores");
            CurrentClockSpeed = props.GetUInt32("CurrentClockSpeed");
            MaxClockSpeed = props.GetUInt32("MaxClockSpeed");
            AddressWidth = props.GetUInt32("AddressWidth");
        }

        internal static string[] GetPropNames()
        {
            return (new string[] { 
                                    "AddressWidth",
                                    "NumberOfLogicalProcessors",
                                    "NumberOfCores",
                                    "CurrentClockSpeed", 
                                    "MaxClockSpeed" 
                                    });
        }
    }
}
