using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimePhysicalMemory : RealTimeGridData
    {
        public string BankLabel { get; private set; }
        public UInt64 Capacity { get; private set; }
        public UInt64 DataWidth { get; private set; }
        public string DeviceLocator { get; private set; }
        public UInt64 FormFactor { get; private set; }
        public UInt64 InterleaveDataDepth { get; private set; }
        public UInt64 InterleavePosition { get; private set; }
        public string Manufacturer { get; private set; }
        public UInt64 MemoryType { get; private set; }
        public string Model { get; private set; }
        public string Name { get; private set; }
        public string OtherIdentifyingInfo { get; private set; }
        public string PartNumber { get; private set; }
        public UInt64 PositionInRow { get; private set; }
        public string SerialNumber { get; private set; }
        public string SKU { get; private set; }
        public UInt64 Speed { get; private set; }
        public string Status { get; private set; }
        public string Tag { get; private set; }
        public UInt64 TotalWidth { get; private set; }
        public UInt64 TypeDetail { get; private set; }
        public string Version { get; private set; }

        private string LabelLocation { get { return (string.Format("{0}:{1}", BankLabel, DeviceLocator)); } }

        public RealTimePhysicalMemory() { }

        internal override bool MultipleSamplesNeeded() { return (false); }
        internal override string GetWmiClassName() { return "Win32_PhysicalMemory"; }
        internal override string[] GetPropNames()
        {
            return (new string[] { 
                                "BankLabel",
                                "Capacity",
                                "DataWidth",
                                "DeviceLocator",
                                "FormFactor",
                                "InterleaveDataDepth",
                                "InterleavePosition",
                                "Manufacturer",
                                "MemoryType",
                                "Model",
                                "Name",
                                "OtherIdentifyingInfo",
                                "PartNumber",
                                "PositionInRow",
                                "SerialNumber",
                                "SKU",
                                "Speed",
                                "Status",
                                "Tag",
                                "TotalWidth",
                                "TypeDetail",
                                "Version"
                                });
        }

        internal override void SetProps(IProvideGridData props) 
        {
            base.SetProps(props);
            BankLabel = props.GetString("BankLabel");
            DeviceLocator = props.GetString("DeviceLocator");
            Manufacturer = props.GetString("Manufacturer");
            Model = props.GetString("Model");
            Name = props.GetString("Name");
            OtherIdentifyingInfo = props.GetString("OtherIdentifyingInfo");
            PartNumber = props.GetString("PartNumber");
            SerialNumber = props.GetString("SerialNumber");
            SKU = props.GetString("SKU");
            Status = props.GetString("Status");
            Tag = props.GetString("Tag");
            Version = props.GetString("Version");
            Capacity = props.GetUInt64("Capacity");
            DataWidth = props.GetUInt64("DataWidth");
            FormFactor = props.GetUInt64("FormFactor");
            InterleaveDataDepth = props.GetUInt64("InterleaveDataDepth");
            InterleavePosition = props.GetUInt64("InterleavePosition");
            MemoryType = props.GetUInt64("MemoryType");
            PositionInRow = props.GetUInt64("PositionInRow");
            Speed = props.GetUInt64("Speed");
            TotalWidth = props.GetUInt64("TotalWidth");
            TypeDetail = props.GetUInt64("TypeDetail");
        }
        public override string ToString() { return (LabelLocation); }
        public override int GetHashCode() { return this.LabelLocation.GetHashCode(); }
        public override bool Equals(object obj)
        {
            RealTimePhysicalMemory rtp = obj as RealTimePhysicalMemory;
            if (null == rtp) return (false);
            return (rtp.LabelLocation == this.LabelLocation);
        }
        internal override void Merge(RealTimeGridData data)
        {
            RealTimePhysicalMemory rtp = data as RealTimePhysicalMemory;
            if (null == rtp) return;
            if (!this.Equals(rtp)) return;
            base.Merge(data);
            BankLabel = rtp.BankLabel;
            DeviceLocator = rtp.DeviceLocator;
            Manufacturer = rtp.Manufacturer;
            Model = rtp.Model;
            Name = rtp.Name;
            OtherIdentifyingInfo = rtp.OtherIdentifyingInfo;
            PartNumber = rtp.PartNumber;
            SerialNumber = rtp.SerialNumber;
            SKU = rtp.SKU;
            Status = rtp.Status;
            Tag = rtp.Tag;
            Version = rtp.Version;
            Capacity = rtp.Capacity;
            DataWidth = rtp.DataWidth;
            FormFactor = rtp.FormFactor;
            InterleaveDataDepth = rtp.InterleaveDataDepth;
            InterleavePosition = rtp.InterleavePosition;
            MemoryType = rtp.MemoryType;
            PositionInRow = rtp.PositionInRow;
            Speed = rtp.Speed;
            TotalWidth = rtp.TotalWidth;
            TypeDetail = rtp.TypeDetail;
        }
    }
}
