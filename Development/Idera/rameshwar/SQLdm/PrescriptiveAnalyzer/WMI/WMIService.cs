using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WMI
{
    public class WMIService
    {
        public string Name { get; private set; }
        public UInt32 ProcessId { get; private set; }
        public string State { get; private set; }

        public bool IsRunning { get { return (0 == string.Compare(State, "Running", true)); } }

        private WMIService() { }
        public WMIService(WMIObjectProperties props)
        {
            Name = props.GetString("Name");
            ProcessId = props.GetUInt32("ProcessId");
            State = props.GetString("State");
        }

        internal static string[] GetPropNames()
        {
            return (new string[] { 
                                    "Name", 
                                    "ProcessId",
                                    "State"
                                    });
        }
        public override string ToString()
        {
            return (string.Format("Name:{0}  PID:{1}  State:{2}", Name, ProcessId, State));
        }
    }
}
