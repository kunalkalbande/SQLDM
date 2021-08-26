using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimeProcessor : RealTimeGridData
    {
        public string Name { get; private set; }
        public string DeviceID { get; private set; }
        public string Description { get; private set; }
        public UInt32 NumberOfCores { get; private set; }
        public UInt32 NumberOfLogicalProcessors { get; private set; }
        public UInt32 LoadPercentage { get; private set; }
        public UInt32 CurrentClockSpeed { get; private set; }
        public UInt32 MaxClockSpeed { get; private set; }
        public UInt32 L2CacheSize { get; private set; }
        public UInt32 L3CacheSize { get; private set; }
        public string Manufacturer { get; private set; }
        public string Status { get; private set; }

        public RealTimeProcessor() { }

        internal override bool MultipleSamplesNeeded() { return (false); }
        internal override string GetWmiClassName() { return "Win32_Processor"; }
        internal override string[] GetPropNames()
        {
            return (new string[] { 
                                "Name",
                                "DeviceID",
                                "Description",
                                "Status",
                                "LoadPercentage",
                                "CurrentClockSpeed",
                                "MaxClockSpeed",
                                "L2CacheSize",
                                "L3CacheSize",
                                "Manufacturer",
                                "NumberOfCores",
                                "NumberOfLogicalProcessors"
                                });
        }

        internal override void SetProps(IProvideGridData props) 
        {
            base.SetProps(props);
            Name = props.GetString("Name");
            DeviceID = props.GetString("DeviceID");
            Description = props.GetString("Description");
            Manufacturer = props.GetString("Manufacturer");
            LoadPercentage = props.GetUInt32("LoadPercentage");
            MaxClockSpeed = props.GetUInt32("MaxClockSpeed");
            CurrentClockSpeed = props.GetUInt32("CurrentClockSpeed");
            L2CacheSize = props.GetUInt32("L2CacheSize");
            L3CacheSize = props.GetUInt32("L3CacheSize");
            NumberOfCores = props.GetUInt32("NumberOfCores");
            NumberOfLogicalProcessors = props.GetUInt32("NumberOfLogicalProcessors");
            Status = props.GetString("Status");
        }

        public override string ToString()
        {
            return (Name);
        }
        public override int GetHashCode() { return this.DeviceID.GetHashCode(); }
        public override bool Equals(object obj)
        {
            RealTimeProcessor rtp = obj as RealTimeProcessor;
            if (null == rtp) return (false);
            if ((rtp.DeviceID == this.DeviceID) && (rtp.Name == this.Name)) return (true);
            return (false);
        }
        internal override void Merge(RealTimeGridData data)
        {
            RealTimeProcessor rtp = data as RealTimeProcessor;
            if (null == rtp) return;
            if (!this.Equals(rtp)) return;
            base.Merge(data);
            Name = rtp.Name;
            DeviceID = rtp.DeviceID;
            Description = rtp.Description;
            Manufacturer = rtp.Manufacturer;
            LoadPercentage = rtp.LoadPercentage;
            MaxClockSpeed = rtp.MaxClockSpeed;
            CurrentClockSpeed = rtp.CurrentClockSpeed;
            L2CacheSize = rtp.L2CacheSize;
            L3CacheSize = rtp.L3CacheSize;
            NumberOfCores = rtp.NumberOfCores;
            NumberOfLogicalProcessors = rtp.NumberOfLogicalProcessors;
            Status = rtp.Status;
        }
    }
}
