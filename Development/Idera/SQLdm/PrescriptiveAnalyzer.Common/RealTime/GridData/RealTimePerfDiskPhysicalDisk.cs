using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.WMI;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimePerfDiskPhysicalDisk : RealTimeGridData
    {
        private WmiPerfCounterBulkCount _diskBytesPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _diskReadBytesPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _diskReadsPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _diskTransfersPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _diskWriteBytesPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _diskWritesPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _splitIOPerSec = new WmiPerfCounterBulkCount();

        public string Name { get; private set; }
        public UInt32 CurrentDiskQueueLength { get; private set; }

        public UInt64 DiskBytesPerSec { get { return (_diskBytesPerSec.GetValue(this)); } }
        public UInt64 DiskReadBytesPerSec { get { return (_diskReadBytesPerSec.GetValue(this)); } }
        public UInt64 DiskReadsPerSec { get { return (_diskReadsPerSec.GetValue(this)); } }
        public UInt64 DiskTransfersPerSec { get { return (_diskTransfersPerSec.GetValue(this)); } }
        public UInt64 DiskWriteBytesPerSec { get { return (_diskWriteBytesPerSec.GetValue(this)); } }
        public UInt64 DiskWritesPerSec { get { return (_diskWritesPerSec.GetValue(this)); } }
        public UInt64 SplitIOPerSec { get { return (_splitIOPerSec.GetValue(this)); } }

        public RealTimePerfDiskPhysicalDisk() { }
        internal override bool MultipleSamplesNeeded() { return (true); }

        internal override string GetWmiClassName() { return "Win32_PerfRawData_PerfDisk_PhysicalDisk"; }
        internal override string[] GetPropNames()
        {
            return (new string[] { 
                                "Name",
                                "CurrentDiskQueueLength",
                                "DiskBytesPerSec",
                                "DiskReadBytesPerSec",
                                "DiskReadsPerSec",
                                "DiskTransfersPerSec",
                                "DiskWriteBytesPerSec",
                                "DiskWritesPerSec",
                                "SplitIOPerSec",
                                "Frequency_Sys100NS",
                                "Timestamp_Sys100NS"
                                });
        }

        internal override void SetProps(IProvideGridData props)
        {
            base.SetProps(props);
            Name = props.GetString("Name");
            CurrentDiskQueueLength = props.GetUInt32("CurrentDiskQueueLength");
            _diskBytesPerSec.UpdateValue(props.GetString("DiskBytesPerSec"));
            _diskReadBytesPerSec.UpdateValue(props.GetString("DiskReadBytesPerSec"));
            _diskReadsPerSec.UpdateValue(props.GetString("DiskReadsPerSec"));
            _diskTransfersPerSec.UpdateValue(props.GetString("DiskTransfersPerSec"));
            _diskWriteBytesPerSec.UpdateValue(props.GetString("DiskWriteBytesPerSec"));
            _diskWritesPerSec.UpdateValue(props.GetString("DiskWritesPerSec"));
            _splitIOPerSec.UpdateValue(props.GetString("SplitIOPerSec"));
        }
        public override int GetHashCode() { return this.Name.GetHashCode(); }
        public override bool Equals(object obj)
        {
            RealTimePerfDiskPhysicalDisk rtp = obj as RealTimePerfDiskPhysicalDisk;
            if (null == rtp) return (false);
            return (rtp.Name == this.Name);
        }

        internal override void Merge(RealTimeGridData data)
        {
            RealTimePerfDiskPhysicalDisk rtp = data as RealTimePerfDiskPhysicalDisk;
            if (null == rtp) return;
            if (!this.Equals(rtp)) return;
            base.Merge(data);
            CurrentDiskQueueLength = rtp.CurrentDiskQueueLength;
            _diskBytesPerSec.UpdateValue(rtp._diskBytesPerSec.GetCurrentValue());
            _diskReadBytesPerSec.UpdateValue(rtp._diskReadBytesPerSec.GetCurrentValue());
            _diskReadsPerSec.UpdateValue(rtp._diskReadsPerSec.GetCurrentValue());
            _diskTransfersPerSec.UpdateValue(rtp._diskTransfersPerSec.GetCurrentValue());
            _diskWriteBytesPerSec.UpdateValue(rtp._diskWriteBytesPerSec.GetCurrentValue());
            _diskWritesPerSec.UpdateValue(rtp._diskWritesPerSec.GetCurrentValue());
            _splitIOPerSec.UpdateValue(rtp._splitIOPerSec.GetCurrentValue());
        }
    }
}
