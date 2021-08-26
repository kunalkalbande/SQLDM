using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimeCacheMemory : RealTimeGridData
    {
        public string DeviceID { get; private set; }
        public string Purpose { get; private set; }
        public string Status { get; private set; }
        public UInt64 StatusInfo { get; private set; }
        public UInt64 Associativity { get; private set; }
        public UInt64 Availability { get; private set; }
        public UInt64 BlockSize { get; private set; }
        public UInt64 CacheType { get; private set; }
        public UInt64 ErrorCorrectType { get; private set; }
        public UInt64 InstalledSize { get; private set; }
        public UInt64 LastErrorCode { get; private set; }
        public UInt64 Level { get; private set; }
        public UInt64 Location { get; private set; }
        public UInt64 MaxCacheSize { get; private set; }
        public UInt64 NumberOfBlocks { get; private set; }
        public UInt64 WritePolicy { get; private set; }
        public string SystemName { get; private set; }

        public RealTimeCacheMemory() { }

        internal override bool MultipleSamplesNeeded() { return (false); }
        internal override string GetWmiClassName() { return "Win32_CacheMemory"; }
        internal override string[] GetPropNames()
        {
            return (new string[] { 
                                "DeviceID",
                                "Purpose",
                                "Status",
                                "StatusInfo",
                                "Associativity",
                                "Availability",
                                "BlockSize",
                                "CacheType",
                                "ErrorCorrectType",
                                "InstalledSize",
                                "LastErrorCode",
                                "Level",
                                "Location",
                                "MaxCacheSize",
                                "NumberOfBlocks",
                                "WritePolicy",
                                "SystemName"
                                });
        }

        internal override void SetProps(IProvideGridData props) 
        {
            base.SetProps(props);
            DeviceID = props.GetString("DeviceID");
            Purpose = props.GetString("Purpose");
            Status = props.GetString("Status");
            SystemName = props.GetString("SystemName");
            StatusInfo = props.GetUInt64("StatusInfo");
            Associativity = props.GetUInt64("Associativity");
            Availability = props.GetUInt64("Availability");
            BlockSize = props.GetUInt64("BlockSize");
            CacheType = props.GetUInt64("CacheType");
            ErrorCorrectType = props.GetUInt64("ErrorCorrectType");
            InstalledSize = props.GetUInt64("InstalledSize");
            LastErrorCode = props.GetUInt64("LastErrorCode");
            Level = props.GetUInt64("Level");
            Location = props.GetUInt64("Location");
            MaxCacheSize = props.GetUInt64("MaxCacheSize");
            NumberOfBlocks = props.GetUInt64("NumberOfBlocks");
            WritePolicy = props.GetUInt64("WritePolicy");
        }

        public override string ToString(){return (DeviceID);}
        public override int GetHashCode() { return this.DeviceID.GetHashCode(); }
        public override bool Equals(object obj)
        {
            RealTimeCacheMemory rtp = obj as RealTimeCacheMemory;
            if (null == rtp) return (false);
            return (rtp.DeviceID == this.DeviceID);
        }
        internal override void Merge(RealTimeGridData data)
        {
            RealTimeCacheMemory rtp = data as RealTimeCacheMemory;
            if (null == rtp) return;
            if (!this.Equals(rtp)) return;
            base.Merge(data);
            Purpose = rtp.Purpose;
            Status = rtp.Status;
            SystemName = rtp.SystemName;
            StatusInfo = rtp.StatusInfo;
            Associativity = rtp.Associativity;
            Availability = rtp.Availability;
            BlockSize = rtp.BlockSize;
            CacheType = rtp.CacheType;
            ErrorCorrectType = rtp.ErrorCorrectType;
            InstalledSize = rtp.InstalledSize;
            LastErrorCode = rtp.LastErrorCode;
            Level = rtp.Level;
            Location = rtp.Location;
            MaxCacheSize = rtp.MaxCacheSize;
            NumberOfBlocks = rtp.NumberOfBlocks;
            WritePolicy = rtp.WritePolicy;
        }
    }
}
