using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.WMI;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimePerfOSProcessor : RealTimeGridData
    {
        private WmiPerfCounterBulkCount _c1TransitionsPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _c2TransitionsPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _c3TransitionsPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _dPCsQueuedPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _interruptsPerSec = new WmiPerfCounterBulkCount();
        private WmiPerf100NSecTimerInv _percentProcessorTime = new WmiPerf100NSecTimerInv();
        private WmiPerf100NSecTimer _percentC1Time = new WmiPerf100NSecTimer();
        private WmiPerf100NSecTimer _percentC2Time = new WmiPerf100NSecTimer();
        private WmiPerf100NSecTimer _percentC3Time = new WmiPerf100NSecTimer();
        private WmiPerf100NSecTimer _percentDPCTime = new WmiPerf100NSecTimer();
        private WmiPerf100NSecTimer _percentIdleTime = new WmiPerf100NSecTimer();
        private WmiPerf100NSecTimer _percentInterruptTime = new WmiPerf100NSecTimer();
        private WmiPerf100NSecTimer _percentPrivilegedTime = new WmiPerf100NSecTimer();
        private WmiPerf100NSecTimer _percentUserTime = new WmiPerf100NSecTimer();

        public string Name { get; private set; }

        public UInt64 C1TransitionsPerSec { get { return (_c1TransitionsPerSec.GetValue(this)); } }
        public UInt64 C2TransitionsPerSec { get { return (_c2TransitionsPerSec.GetValue(this)); } }
        public UInt64 C3TransitionsPerSec { get { return (_c3TransitionsPerSec.GetValue(this)); } }
        public UInt32 DPCRate { get; private set; }
        public UInt64 DPCsQueuedPerSec { get { return (_dPCsQueuedPerSec.GetValue(this)); } }
        public UInt64 InterruptsPerSec { get { return (_interruptsPerSec.GetValue(this)); } }
        public UInt64 PercentC1Time { get { return (_percentC1Time.GetValue(this)); } }
        public UInt64 PercentC2Time { get { return (_percentC2Time.GetValue(this)); } }
        public UInt64 PercentC3Time { get { return (_percentC3Time.GetValue(this)); } }
        public UInt64 PercentDPCTime { get { return (_percentDPCTime.GetValue(this)); } }
        public UInt64 PercentIdleTime { get { return (_percentIdleTime.GetValue(this)); } }
        public UInt64 PercentInterruptTime { get { return (_percentInterruptTime.GetValue(this)); } }
        public UInt64 PercentPrivilegedTime { get { return (_percentPrivilegedTime.GetValue(this)); } }
        public UInt64 PercentProcessorTime { get { return (_percentProcessorTime.GetValue(this)); } }
        public UInt64 PercentUserTime { get { return (_percentUserTime.GetValue(this)); } }

        public RealTimePerfOSProcessor() { }
        internal override bool MultipleSamplesNeeded() { return (true); }

        internal override string GetWmiClassName() { return "Win32_PerfRawData_PerfOS_Processor"; }
        internal override string[] GetPropNames()
        {
            return (new string[] { 
                                "Name",
                                "C1TransitionsPerSec",
                                "C2TransitionsPerSec",
                                "C3TransitionsPerSec",
                                "DPCRate",
                                "DPCsQueuedPerSec",
                                "InterruptsPerSec",
                                "PercentC1Time",
                                "PercentC2Time",
                                "PercentC3Time",
                                "PercentDPCTime",
                                "PercentIdleTime",
                                "PercentInterruptTime",
                                "PercentPrivilegedTime",
                                "PercentProcessorTime",
                                "PercentUserTime",
                                "Frequency_Sys100NS",
                                "Timestamp_Sys100NS"
                                });
        }

        internal override void SetProps(IProvideGridData props)
        {
            base.SetProps(props);
            Name = props.GetString("Name");
            _c1TransitionsPerSec.UpdateValue(props.GetString("C1TransitionsPerSec"));
            _c2TransitionsPerSec.UpdateValue(props.GetString("C2TransitionsPerSec"));
            _c3TransitionsPerSec.UpdateValue(props.GetString("C3TransitionsPerSec"));
            DPCRate = props.GetUInt32("DPCRate");
            _dPCsQueuedPerSec.UpdateValue(props.GetString("DPCsQueuedPerSec"));
            _interruptsPerSec.UpdateValue(props.GetString("InterruptsPerSec"));
            _percentC1Time.UpdateValue(props.GetString("PercentC1Time"));
            _percentC2Time.UpdateValue(props.GetString("PercentC2Time"));
            _percentC3Time.UpdateValue(props.GetString("PercentC3Time"));
            _percentDPCTime.UpdateValue(props.GetString("PercentDPCTime"));
            _percentIdleTime.UpdateValue(props.GetString("PercentIdleTime"));
            _percentInterruptTime.UpdateValue(props.GetString("PercentInterruptTime"));
            _percentPrivilegedTime.UpdateValue(props.GetString("PercentPrivilegedTime"));
            _percentProcessorTime.UpdateValue(props.GetString("PercentProcessorTime"));
            _percentUserTime.UpdateValue(props.GetString("PercentUserTime"));
        }
        public override int GetHashCode() { return this.Name.GetHashCode(); }
        public override bool Equals(object obj)
        {
            RealTimePerfOSProcessor rtp = obj as RealTimePerfOSProcessor;
            if (null == rtp) return (false);
            return (rtp.Name == this.Name);
        }

        internal override void Merge(RealTimeGridData data)
        {
            RealTimePerfOSProcessor rtp = data as RealTimePerfOSProcessor;
            if (null == rtp) return;
            if (!this.Equals(rtp)) return;
            base.Merge(data);
            _c1TransitionsPerSec.UpdateValue(rtp._c1TransitionsPerSec.GetCurrentValue());
            _c2TransitionsPerSec.UpdateValue(rtp._c2TransitionsPerSec.GetCurrentValue());
            _c3TransitionsPerSec.UpdateValue(rtp._c3TransitionsPerSec.GetCurrentValue());
            _dPCsQueuedPerSec.UpdateValue(rtp._dPCsQueuedPerSec.GetCurrentValue());
            _interruptsPerSec.UpdateValue(rtp._interruptsPerSec.GetCurrentValue());
            _percentProcessorTime.UpdateValue(rtp._percentProcessorTime.GetCurrentValue());
            _percentC1Time.UpdateValue(rtp._percentC1Time.GetCurrentValue());
            _percentC2Time.UpdateValue(rtp._percentC2Time.GetCurrentValue());
            _percentC3Time.UpdateValue(rtp._percentC3Time.GetCurrentValue());
            _percentDPCTime.UpdateValue(rtp._percentDPCTime.GetCurrentValue());
            _percentIdleTime.UpdateValue(rtp._percentIdleTime.GetCurrentValue());
            _percentInterruptTime.UpdateValue(rtp._percentInterruptTime.GetCurrentValue());
            _percentPrivilegedTime.UpdateValue(rtp._percentPrivilegedTime.GetCurrentValue());
            _percentUserTime.UpdateValue(rtp._percentUserTime.GetCurrentValue());
            DPCRate = rtp.DPCRate;
        }
    }
}
