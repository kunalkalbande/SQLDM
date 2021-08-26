using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.WMI;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimePerfProcProcess : RealTimeGridData
    {
        private WmiPerf100NSecTimer _percentProcessorTime = new WmiPerf100NSecTimer();
        private WmiPerf100NSecTimer _percentPrivilegedTime = new WmiPerf100NSecTimer();
        private WmiPerf100NSecTimer _percentUserTime = new WmiPerf100NSecTimer();

        public UInt32 ProcessId { get; private set; }
        public string Name { get; private set; }
        public UInt64 CPU { get { return (ProcessorCount > 1 ? _percentProcessorTime.GetValue(this) / ProcessorCount : _percentProcessorTime.GetValue(this)); } }
        public UInt64 PrivilegedTime { get { return (ProcessorCount > 1 ? _percentPrivilegedTime.GetValue(this) / ProcessorCount : _percentPrivilegedTime.GetValue(this)); } }
        public UInt64 UserTime { get { return (ProcessorCount > 1 ? _percentUserTime.GetValue(this) / ProcessorCount : _percentUserTime.GetValue(this)); } }
        public FormattedBytes WorkingSet { get; private set; }
        public FormattedBytes WorkingSetPeak { get; private set; }
        public FormattedBytes VirtualBytes { get; private set; }
        public FormattedBytes VirtualBytesPeak { get; private set; }
        public FormattedBytes PrivateBytes { get; private set; }
        public UInt32 Threads { get; private set; }
        public UInt32 Handles { get; private set; }

        public UInt32 ProcessorCount { private get; set; }

        public RealTimePerfProcProcess() { }
        internal override bool MultipleSamplesNeeded() { return (true); }

        internal override string GetWmiClassName() { return "Win32_PerfRawData_PerfProc_Process"; }
        internal override string[] GetPropNames()
        {
            return (new string[] { 
                                "Name",
                                "IDProcess",
                                "PercentProcessorTime",
                                "PercentPrivilegedTime",
                                "PercentUserTime",
                                "ThreadCount",
                                "HandleCount",
                                "WorkingSet",
                                "WorkingSetPeak",
                                "VirtualBytes",
                                "VirtualBytesPeak",
                                "PrivateBytes",
                                "Timestamp_Sys100NS"
                                });
        }

        internal override void SetProps(IProvideGridData props)
        {
            base.SetProps(props);
            Name = props.GetString("Name");
            ProcessId = props.GetUInt32("IDProcess");
            _percentProcessorTime.UpdateValue(props.GetString("PercentProcessorTime"));
            _percentPrivilegedTime.UpdateValue(props.GetString("PercentPrivilegedTime"));
            _percentUserTime.UpdateValue(props.GetString("PercentUserTime"));
            WorkingSet = new FormattedBytes(props.GetUInt64("WorkingSet"));
            WorkingSetPeak = new FormattedBytes(props.GetUInt64("WorkingSetPeak"));
            VirtualBytes = new FormattedBytes(props.GetUInt64("VirtualBytes"));
            VirtualBytesPeak = new FormattedBytes(props.GetUInt64("VirtualBytesPeak"));
            PrivateBytes = new FormattedBytes(props.GetUInt64("PrivateBytes"));
            Threads = props.GetUInt32("ThreadCount");
            Handles = props.GetUInt32("HandleCount");
        }
        public override int GetHashCode(){return this.ProcessId.GetHashCode();}
        public override bool Equals(object obj)
        {
            RealTimePerfProcProcess rtp = obj as RealTimePerfProcProcess;
            if (null == rtp) return (false);
            if ((rtp.ProcessId == this.ProcessId) && (rtp.Name == this.Name)) return (true);
            return (false);
        }

        internal override void Merge(RealTimeGridData data)
        {
            RealTimePerfProcProcess rtp = data as RealTimePerfProcProcess;
            if (null == rtp) return;
            if (!this.Equals(rtp)) return;
            base.Merge(data);
            _percentProcessorTime.UpdateValue(rtp._percentProcessorTime.GetCurrentValue());
            _percentPrivilegedTime.UpdateValue(rtp._percentPrivilegedTime.GetCurrentValue());
            _percentUserTime.UpdateValue(rtp._percentUserTime.GetCurrentValue());
            WorkingSet = rtp.WorkingSet;
            WorkingSetPeak = rtp.WorkingSetPeak;
            VirtualBytes = rtp.VirtualBytes;
            VirtualBytesPeak = rtp.VirtualBytesPeak;
            PrivateBytes = rtp.PrivateBytes;
            Threads = rtp.Threads;
            Handles = rtp.Handles;
        }
    }
}
