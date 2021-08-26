using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Idera.SQLdoctor.Common.Helpers;
using Idera.SQLdoctor.AnalysisEngine.Batches;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIPhysicalMemory
    {
        public string Caption { get; private set; }
        public UInt64 Capacity { get; private set; }

        private WMIPhysicalMemory() { }
        public WMIPhysicalMemory(WMIObjectProperties props)
        {
            Capacity = props.GetUInt64("Capacity");
            Caption = props.GetString("Caption");
        }

        internal static string[] GetPropNames()
        {
            return (new string[] { 
                                    "Capacity", 
                                    "Caption", 
                                    });
        }

        public override string ToString()
        {
            return (string.Format("WMIPhysicalMemory - Caption: {0}  Capacity: {1}", Caption, Capacity));
        }
    }
}
