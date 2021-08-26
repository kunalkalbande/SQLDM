using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.WMI;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimePerfDiskLogicalDisk : RealTimeGridData
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
        //public UInt32 PercentFreeSpace { get; private set; }
        //public UInt32 PercentFreeSpace_Base { get; private set; }
        private UInt32 PercentFreeSpace { get; set; }
        private UInt32 PercentFreeSpace_Base { get; set; }

        public UInt64 DiskBytesPerSec { get { return (_diskBytesPerSec.GetValue(this)); } }
        public UInt64 DiskReadBytesPerSec { get { return (_diskReadBytesPerSec.GetValue(this)); } }
        public UInt64 DiskReadsPerSec { get { return (_diskReadsPerSec.GetValue(this)); } }
        public UInt64 DiskTransfersPerSec { get { return (_diskTransfersPerSec.GetValue(this)); } }
        public UInt64 DiskWriteBytesPerSec { get { return (_diskWriteBytesPerSec.GetValue(this)); } }
        public UInt64 DiskWritesPerSec { get { return (_diskWritesPerSec.GetValue(this)); } }
        public UInt64 SplitIOPerSec { get { return (_splitIOPerSec.GetValue(this)); } }

        public UInt64 FreeMegabytes { get; private set; }

        public RealTimePerfDiskLogicalDisk() { }
        internal override bool MultipleSamplesNeeded() { return (true); }

        internal override string GetWmiClassName() { return "Win32_PerfRawData_PerfDisk_LogicalDisk"; }
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
                                "FreeMegabytes",
                                "Frequency_Sys100NS",
                                "Timestamp_Sys100NS",
                                "PercentFreeSpace",
                                "PercentFreeSpace_Base"
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
            FreeMegabytes = props.GetUInt32("FreeMegabytes");
            PercentFreeSpace = props.GetUInt32("PercentFreeSpace");
            PercentFreeSpace_Base = props.GetUInt32("PercentFreeSpace_Base");
        }
        public override int GetHashCode() { return this.Name.GetHashCode(); }
        public override bool Equals(object obj)
        {
            RealTimePerfDiskLogicalDisk rtp = obj as RealTimePerfDiskLogicalDisk;
            if (null == rtp) return (false);
            return (rtp.Name == this.Name);
        }

        internal override void Merge(RealTimeGridData data)
        {
            RealTimePerfDiskLogicalDisk rtp = data as RealTimePerfDiskLogicalDisk;
            if (null == rtp) return;
            if (!this.Equals(rtp)) return;
            base.Merge(data);
            _diskBytesPerSec.UpdateValue(rtp._diskBytesPerSec.GetCurrentValue());
            _diskReadBytesPerSec.UpdateValue(rtp._diskReadBytesPerSec.GetCurrentValue());
            _diskReadsPerSec.UpdateValue(rtp._diskReadsPerSec.GetCurrentValue());
            _diskTransfersPerSec.UpdateValue(rtp._diskTransfersPerSec.GetCurrentValue());
            _diskWriteBytesPerSec.UpdateValue(rtp._diskWriteBytesPerSec.GetCurrentValue());
            _diskWritesPerSec.UpdateValue(rtp._diskWritesPerSec.GetCurrentValue());
            _splitIOPerSec.UpdateValue(rtp._splitIOPerSec.GetCurrentValue());

            CurrentDiskQueueLength = rtp.CurrentDiskQueueLength;
            FreeMegabytes = rtp.FreeMegabytes;
            PercentFreeSpace = rtp.PercentFreeSpace;
            PercentFreeSpace_Base = rtp.PercentFreeSpace_Base;
        }

        public int GetPercentFreeSpace()
        {
            return (int)((Convert.ToDouble(PercentFreeSpace) / Convert.ToDouble(PercentFreeSpace_Base)) * 100);
        }
    }
}
