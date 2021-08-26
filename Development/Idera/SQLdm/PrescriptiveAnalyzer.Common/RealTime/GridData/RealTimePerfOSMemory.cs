using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.WMI;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimePerfOSMemory : RealTimeGridData
    {
        private WmiPerfCounterBulkCount _CacheFaultsPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _DemandZeroFaultsPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _FreeSystemPageTableEntries = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _PageFaultsPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _PageReadsPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _PagesInputPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _PagesOutputPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _PagesPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _PageWritesPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _TransitionFaultsPerSec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _WriteCopiesPerSec = new WmiPerfCounterBulkCount();

        private string Name { get; set; }
        private string Description { get; set; }
        private string Caption { get; set; }

        public UInt64 AvailableBytes { get; private set; }
        public UInt64 AvailableKBytes { get; private set; }
        public UInt64 AvailableMBytes { get; private set; }
        public UInt64 CacheBytes { get; private set; }
        public UInt64 CacheBytesPeak { get; private set; }
        public UInt64 CommitLimit { get; private set; }
        public UInt64 CommittedBytes { get; private set; }
        public UInt64 PoolNonpagedAllocs { get; private set; }
        public UInt64 PoolNonpagedBytes { get; private set; }
        public UInt64 PoolPagedAllocs { get; private set; }
        public UInt64 PoolPagedBytes { get; private set; }
        public UInt64 PoolPagedResidentBytes { get; private set; }
        public UInt64 SystemCacheResidentBytes { get; private set; }
        public UInt64 SystemCodeResidentBytes { get; private set; }
        public UInt64 SystemCodeTotalBytes { get; private set; }
        public UInt64 SystemDriverResidentBytes { get; private set; }
        public UInt64 SystemDriverTotalBytes { get; private set; }

        public UInt64 CacheFaultsPerSec { get { return (_CacheFaultsPerSec.GetValue(this)); } }
        public UInt64 DemandZeroFaultsPerSec { get { return (_DemandZeroFaultsPerSec.GetValue(this)); } }
        public UInt64 FreeSystemPageTableEntries { get { return (_FreeSystemPageTableEntries.GetValue(this)); } }
        public UInt64 PageFaultsPerSec { get { return (_PageFaultsPerSec.GetValue(this)); } }
        public UInt64 PageReadsPerSec { get { return (_PageReadsPerSec.GetValue(this)); } }
        public UInt64 PagesInputPerSec { get { return (_PagesInputPerSec.GetValue(this)); } }
        public UInt64 PagesOutputPerSec { get { return (_PagesOutputPerSec.GetValue(this)); } }
        public UInt64 PagesPerSec { get { return (_PagesPerSec.GetValue(this)); } }
        public UInt64 PageWritesPerSec { get { return (_PageWritesPerSec.GetValue(this)); } }
        public UInt64 TransitionFaultsPerSec { get { return (_TransitionFaultsPerSec.GetValue(this)); } }
        public UInt64 WriteCopiesPerSec { get { return (_WriteCopiesPerSec.GetValue(this)); } }

        public RealTimePerfOSMemory() { }
        internal override bool MultipleSamplesNeeded() { return (true); }

        internal override string GetWmiClassName() { return "Win32_PerfRawData_PerfOS_Memory"; }
        internal override string[] GetPropNames()
        {
            return (new string[] { 
                                "Name",
                                "Description",
                                "Caption",
                                "AvailableBytes",
                                "AvailableKBytes",
                                "AvailableMBytes",
                                "CacheBytes",
                                "CacheBytesPeak",
                                "CommitLimit",
                                "CommittedBytes",
                                "PoolNonpagedAllocs",
                                "PoolNonpagedBytes",
                                "PoolPagedAllocs",
                                "PoolPagedBytes",
                                "PoolPagedResidentBytes",
                                "SystemCacheResidentBytes",
                                "SystemCodeResidentBytes",
                                "SystemCodeTotalBytes",
                                "SystemDriverResidentBytes",
                                "SystemDriverTotalBytes",
                                "CacheFaultsPerSec",
                                "DemandZeroFaultsPerSec",
                                "FreeSystemPageTableEntries",
                                "PageFaultsPerSec",
                                "PageReadsPerSec",
                                "PagesInputPerSec",
                                "PagesOutputPerSec",
                                "PagesPerSec",
                                "PageWritesPerSec",
                                "TransitionFaultsPerSec",
                                "WriteCopiesPerSec",
                                "Frequency_Sys100NS",
                                "Timestamp_Sys100NS"
                                });
        }

        internal override void SetProps(IProvideGridData props)
        {
            base.SetProps(props);
            Name = props.GetString("Name");
            Description = props.GetString("Description");
            Caption = props.GetString("Caption");
            AvailableBytes = props.GetUInt64("AvailableBytes");
            AvailableKBytes = props.GetUInt64("AvailableKBytes");
            AvailableMBytes = props.GetUInt64("AvailableMBytes");
            CacheBytes = props.GetUInt64("CacheBytes");
            CacheBytesPeak = props.GetUInt64("CacheBytesPeak");
            CommitLimit = props.GetUInt64("CommitLimit");
            CommittedBytes = props.GetUInt64("CommittedBytes");
            PoolNonpagedAllocs = props.GetUInt64("PoolNonpagedAllocs");
            PoolNonpagedBytes = props.GetUInt64("PoolNonpagedBytes");
            PoolPagedAllocs = props.GetUInt64("PoolPagedAllocs");
            PoolPagedBytes = props.GetUInt64("PoolPagedBytes");
            PoolPagedResidentBytes = props.GetUInt64("PoolPagedResidentBytes");
            SystemCacheResidentBytes = props.GetUInt64("SystemCacheResidentBytes");
            SystemCodeResidentBytes = props.GetUInt64("SystemCodeResidentBytes");
            SystemCodeTotalBytes = props.GetUInt64("SystemCodeTotalBytes");
            SystemDriverResidentBytes = props.GetUInt64("SystemDriverResidentBytes");
            SystemDriverTotalBytes = props.GetUInt64("SystemDriverTotalBytes");
        
            _CacheFaultsPerSec.UpdateValue(props.GetString("CacheFaultsPerSec"));
            _DemandZeroFaultsPerSec.UpdateValue(props.GetString("DemandZeroFaultsPerSec"));
            _FreeSystemPageTableEntries.UpdateValue(props.GetString("FreeSystemPageTableEntries"));
            _PageFaultsPerSec.UpdateValue(props.GetString("PageFaultsPerSec"));
            _PageReadsPerSec.UpdateValue(props.GetString("PageReadsPerSec"));
            _PagesInputPerSec.UpdateValue(props.GetString("PagesInputPerSec"));
            _PagesOutputPerSec.UpdateValue(props.GetString("PagesOutputPerSec"));
            _PagesPerSec.UpdateValue(props.GetString("PagesPerSec"));
            _PageWritesPerSec.UpdateValue(props.GetString("PageWritesPerSec"));
            _TransitionFaultsPerSec.UpdateValue(props.GetString("TransitionFaultsPerSec"));
            _WriteCopiesPerSec.UpdateValue(props.GetString("WriteCopiesPerSec"));
        }
        public override int GetHashCode() { return string.Format("{0}:{1}:{2}", Name, Description, Caption).GetHashCode(); }
        public override bool Equals(object obj)
        {
            RealTimePerfOSMemory rtp = obj as RealTimePerfOSMemory;
            if (null == rtp) return (false);
            return ((rtp.Name == this.Name) && (rtp.Description == this.Description) && (rtp.Caption == this.Caption));
        }

        internal override void Merge(RealTimeGridData data)
        {
            RealTimePerfOSMemory rtp = data as RealTimePerfOSMemory;
            if (null == rtp) return;
            if (!this.Equals(rtp)) return;
            base.Merge(data);

            AvailableBytes = rtp.AvailableBytes;
            AvailableKBytes = rtp.AvailableKBytes;
            AvailableMBytes = rtp.AvailableMBytes;
            CacheBytes = rtp.CacheBytes;
            CacheBytesPeak = rtp.CacheBytesPeak;
            CommitLimit = rtp.CommitLimit;
            CommittedBytes = rtp.CommittedBytes;
            PoolNonpagedAllocs = rtp.PoolNonpagedAllocs;
            PoolNonpagedBytes = rtp.PoolNonpagedBytes;
            PoolPagedAllocs = rtp.PoolPagedAllocs;
            PoolPagedBytes = rtp.PoolPagedBytes;
            PoolPagedResidentBytes = rtp.PoolPagedResidentBytes;
            SystemCacheResidentBytes = rtp.SystemCacheResidentBytes;
            SystemCodeResidentBytes = rtp.SystemCodeResidentBytes;
            SystemCodeTotalBytes = rtp.SystemCodeTotalBytes;
            SystemDriverResidentBytes = rtp.SystemDriverResidentBytes;
            SystemDriverTotalBytes = rtp.SystemDriverTotalBytes;

            _CacheFaultsPerSec.UpdateValue(rtp._CacheFaultsPerSec.GetCurrentValue());
            _DemandZeroFaultsPerSec.UpdateValue(rtp._DemandZeroFaultsPerSec.GetCurrentValue());
            _FreeSystemPageTableEntries.UpdateValue(rtp._FreeSystemPageTableEntries.GetCurrentValue());
            _PageFaultsPerSec.UpdateValue(rtp._PageFaultsPerSec.GetCurrentValue());
            _PageReadsPerSec.UpdateValue(rtp._PageReadsPerSec.GetCurrentValue());
            _PagesInputPerSec.UpdateValue(rtp._PagesInputPerSec.GetCurrentValue());
            _PagesOutputPerSec.UpdateValue(rtp._PagesOutputPerSec.GetCurrentValue());
            _PagesPerSec.UpdateValue(rtp._PagesPerSec.GetCurrentValue());
            _PageWritesPerSec.UpdateValue(rtp._PageWritesPerSec.GetCurrentValue());
            _TransitionFaultsPerSec.UpdateValue(rtp._TransitionFaultsPerSec.GetCurrentValue());
            _WriteCopiesPerSec.UpdateValue(rtp._WriteCopiesPerSec.GetCurrentValue());

        }
    }
}
