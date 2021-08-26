using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimeProcess : RealTimeGridData
    {
        public UInt32 ProcessId { get; private set; }
        public string Name { get; private set; }
        public UInt32 Priority { get; private set; }
        public UInt32 ThreadCount { get; private set; }
        public UInt64 WorkingSetSize { get; private set; }
        public string CommandLine { get; private set; }
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
        public RealTimeProcess() { }

        internal override bool MultipleSamplesNeeded() { return (false); }
        internal override string GetWmiClassName() { return "Win32_Process"; }
        internal override string[] GetPropNames()
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

        internal override void SetProps(IProvideGridData props) 
        {
            base.SetProps(props);
            Name = props.GetString("Name");
            CommandLine = props.GetString("CommandLine");
            Priority = props.GetUInt32("Priority");
            ThreadCount = props.GetUInt32("ThreadCount");
            ProcessId = props.GetUInt32("ProcessId");
            WorkingSetSize = props.GetUInt64("WorkingSetSize");
        }

        public override string ToString()
        {
            return (string.Format("{0}  WorkingSetSize:{1}", Name, WorkingSetSize));
        }
        public override int GetHashCode() { return this.ProcessId.GetHashCode(); }
        public override bool Equals(object obj)
        {
            RealTimeProcess rtp = obj as RealTimeProcess;
            if (null == rtp) return (false);
            if ((rtp.ProcessId == this.ProcessId) && (rtp.Name == this.Name)) return (true);
            return (false);
        }
        internal override void Merge(RealTimeGridData data)
        {
            RealTimeProcess rtp = data as RealTimeProcess;
            if (null == rtp) return;
            if (!this.Equals(rtp)) return;
            base.Merge(data);
            Name = rtp.Name;
            CommandLine = rtp.CommandLine;
            Priority = rtp.Priority;
            ThreadCount = rtp.ThreadCount;
            ProcessId = rtp.ProcessId;
            WorkingSetSize = rtp.WorkingSetSize;
        }
    }
}
