using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WMI
{
    public class WMIProcess
    {
        public string Name { get; private set; }
        public string CommandLine { get; private set; }
        public UInt32 Priority { get; private set; }
        public UInt32 ThreadCount { get; private set; }
        public UInt32 ProcessId { get; private set; }
        public UInt64 WorkingSetSize { get; private set; }
        public string SvcHostArgs
        {
            get
            {
                if (0 == string.Compare(Name, "svchost.exe", true))
                {
                    int offset = CommandLine.IndexOf(" -k ");
                    if (offset >= 0)
                    {
                        return (CommandLine.Substring(offset + 4));
                    }
                }
                return (string.Empty);
            }
        }
        private WMIProcess() { }
        public WMIProcess(WMIObjectProperties props)
        {
            Name = props.GetString("Name");
            CommandLine = props.GetString("CommandLine");
            Priority = props.GetUInt32("Priority");
            ThreadCount = props.GetUInt32("ThreadCount");
            ProcessId = props.GetUInt32("ProcessId");
            WorkingSetSize = props.GetUInt64("WorkingSetSize");
        }

        internal static string[] GetPropNames()
        {
            return (new string[] { 
                                    "Name",
                                    "CommandLine",
                                    "ProcessId",
                                    "ThreadCount",
                                    "Priority",
                                    "WorkingSetSize"
                                    });
        }
        public override string ToString()
        {
            return (string.Format("{0}  WorkingSetSize:{1}", Name, WorkingSetSize));
        }
    }
}
