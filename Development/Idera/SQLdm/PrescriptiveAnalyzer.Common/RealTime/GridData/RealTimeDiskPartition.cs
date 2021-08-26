using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimeDiskPartition : RealTimeGridData
    {
        public string DeviceID { get; private set; }
        public string Status { get; private set; }
        public UInt64 StatusInfo { get; private set; }
        public UInt64 Size { get; private set; }
        public UInt64 StartingOffset { get; private set; }
        public UInt64 BlockSize { get; private set; }
        public UInt64 NumberOfBlocks { get; private set; }
        public bool Bootable { get; private set; }
        public bool BootPartition { get; private set; }
        public string Description { get; private set; }
        public UInt64 DiskIndex { get; private set; }
        public UInt64 HiddenSectors { get; private set; }
        public UInt64 Index { get; private set; }
        public string PNPDeviceID { get; private set; }
        public bool PrimaryPartition { get; private set; }
        public string Type { get; private set; }
        public string SystemName { get; private set; }

        public RealTimeDiskPartition() { }

        internal override bool MultipleSamplesNeeded() { return (false); }
        internal override string GetWmiClassName() { return "Win32_DiskPartition"; }
        internal override string[] GetPropNames()
        {
            return (new string[] { 
                                "DeviceID",
                                "Status",
                                "StatusInfo",
                                "BlockSize",
                                "NumberOfBlocks",
                                "Bootable",
                                "BootPartition",
                                "Description",
                                "DiskIndex",
                                "HiddenSectors",
                                "Index",
                                "PNPDeviceID",
                                "PrimaryPartition",
                                "Size",
                                "StartingOffset",
                                "Type",
                                "SystemName"
                                });
        }

        internal override void SetProps(IProvideGridData props) 
        {
            base.SetProps(props);
            DeviceID = props.GetString("DeviceID");
            Status = props.GetString("Status");
            SystemName = props.GetString("SystemName");
            StatusInfo = props.GetUInt64("StatusInfo");
            BlockSize = props.GetUInt64("BlockSize");
            NumberOfBlocks = props.GetUInt64("NumberOfBlocks");
            Bootable = props.GetBool("Bootable");
            BootPartition = props.GetBool("BootPartition");
            Description = props.GetString("Description");
            DiskIndex = props.GetUInt64("DiskIndex");
            HiddenSectors = props.GetUInt64("HiddenSectors");
            Index = props.GetUInt64("Index");
            PNPDeviceID = props.GetString("PNPDeviceID");
            PrimaryPartition = props.GetBool("PrimaryPartition");
            Size = props.GetUInt64("Size");
            StartingOffset = props.GetUInt64("StartingOffset");
            Type = props.GetString("Type");
        }

        public override string ToString(){return (DeviceID);}
        public override int GetHashCode() { return this.DeviceID.GetHashCode(); }
        public override bool Equals(object obj)
        {
            RealTimeDiskPartition rtp = obj as RealTimeDiskPartition;
            if (null == rtp) return (false);
            return (rtp.DeviceID == this.DeviceID);
        }
        internal override void Merge(RealTimeGridData data)
        {
            RealTimeDiskPartition rtp = data as RealTimeDiskPartition;
            if (null == rtp) return;
            if (!this.Equals(rtp)) return;
            base.Merge(data);
            Status = rtp.Status;
            SystemName = rtp.SystemName;
            StatusInfo = rtp.StatusInfo;
            BlockSize = rtp.BlockSize;
            NumberOfBlocks = rtp.NumberOfBlocks;
            Bootable = rtp.Bootable;
            BootPartition = rtp.BootPartition;
            Description = rtp.Description;
            DiskIndex = rtp.DiskIndex;
            HiddenSectors = rtp.HiddenSectors;
            Index = rtp.Index;
            PNPDeviceID = rtp.PNPDeviceID;
            PrimaryPartition = rtp.PrimaryPartition;
            Size = rtp.Size;
            StartingOffset = rtp.StartingOffset;
            Type = rtp.Type;
        }
    }
}
