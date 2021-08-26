using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimeComputerSystem : RealTimeGridData
    {
        public string Name { get; private set; }
        public UInt32 AdminPasswordStatus { get; private set; }
        public string BootupState { get; private set; }
        public UInt32 ChassisBootupState { get; private set; }
        public int CurrentTimeZone { get; private set; }
        public string Description { get; private set; }
        public string DNSHostName { get; private set; }
        public string Domain { get; private set; }
        public UInt32 DomainRole { get; private set; }
        public bool EnableDaylightSavingsTime { get; private set; }
        public UInt32 FrontPanelResetStatus { get; private set; }
        public UInt32 KeyboardPasswordStatus { get; private set; }
        public string Manufacturer { get; private set; }
        public string Model { get; private set; }
        public bool NetworkServerModeEnabled { get; private set; }
        public UInt32 NumberOfLogicalProcessors { get; private set; }
        public UInt32 NumberOfProcessors { get; private set; }
        public bool PartOfDomain { get; private set; }
        public int PauseAfterReset { get; private set; }
        public UInt32 PCSystemType { get; private set; }
        public UInt32 PowerOnPasswordStatus { get; private set; }
        public UInt32 PowerState { get; private set; }
        public UInt32 PowerSupplyState { get; private set; }
        public UInt32 ResetCapability { get; private set; }
        public int ResetCount { get; private set; }
        public int ResetLimit { get; private set; }
        public string Status { get; private set; }
        public string SystemType { get; private set; }
        public UInt32 ThermalState { get; private set; }
        public UInt64 TotalPhysicalMemory { get; private set; }
        public string UserName { get; private set; }
        public UInt32 WakeUpType { get; private set; }

        public RealTimeComputerSystem() { }

        internal override bool MultipleSamplesNeeded() { return (false); }
        internal override string GetWmiClassName() { return "Win32_ComputerSystem"; }
        internal override string[] GetPropNames()
        {
            return (new string[] { 
                                "Name",
                                "AdminPasswordStatus",
                                "BootupState",
                                "ChassisBootupState",
                                "CurrentTimeZone",
                                "Description",
                                "DNSHostName",
                                "Domain",
                                "DomainRole",
                                "EnableDaylightSavingsTime",
                                "FrontPanelResetStatus",
                                "KeyboardPasswordStatus",
                                "Manufacturer",
                                "Model",
                                "NetworkServerModeEnabled",
                                "NumberOfLogicalProcessors",
                                "NumberOfProcessors",
                                "PartOfDomain",
                                "PauseAfterReset",
                                "PCSystemType",
                                "PowerOnPasswordStatus",
                                "PowerState",
                                "PowerSupplyState",
                                "ResetCapability",
                                "ResetCount",
                                "ResetLimit",
                                "Status",
                                "SystemType",
                                "ThermalState",
                                "TotalPhysicalMemory",
                                "UserName",
                                "WakeUpType",
                                });
        }

        internal override void SetProps(IProvideGridData props) 
        {
            base.SetProps(props);
            Name = props.GetString("Name");
            AdminPasswordStatus = props.GetUInt32("AdminPasswordStatus");
            BootupState = props.GetString("BootupState");
            Description = props.GetString("Description");
            DNSHostName = props.GetString("DNSHostName");
            Domain = props.GetString("Domain");
            Manufacturer = props.GetString("Manufacturer");
            Model = props.GetString("Model");
            Status = props.GetString("Status");
            SystemType = props.GetString("SystemType");
            UserName = props.GetString("UserName");
            ChassisBootupState = props.GetUInt32("ChassisBootupState");
            DomainRole = props.GetUInt32("DomainRole");
            FrontPanelResetStatus = props.GetUInt32("FrontPanelResetStatus");
            KeyboardPasswordStatus = props.GetUInt32("KeyboardPasswordStatus");
            NumberOfLogicalProcessors = props.GetUInt32("NumberOfLogicalProcessors");
            NumberOfProcessors = props.GetUInt32("NumberOfProcessors");
            PCSystemType = props.GetUInt32("PCSystemType");
            PowerOnPasswordStatus = props.GetUInt32("PowerOnPasswordStatus");
            PowerState = props.GetUInt32("PowerState");
            PowerSupplyState = props.GetUInt32("PowerSupplyState");
            ResetCapability = props.GetUInt32("ResetCapability");
            ThermalState = props.GetUInt32("ThermalState");
            WakeUpType = props.GetUInt32("WakeUpType");
            TotalPhysicalMemory = props.GetUInt64("TotalPhysicalMemory");
            EnableDaylightSavingsTime = props.GetBool("EnableDaylightSavingsTime");
            NetworkServerModeEnabled = props.GetBool("NetworkServerModeEnabled");
            PartOfDomain = props.GetBool("PartOfDomain");
            CurrentTimeZone = props.GetInt32("CurrentTimeZone");
            PauseAfterReset = props.GetInt32("PauseAfterReset");
            ResetCount = props.GetInt32("ResetCount");
            ResetLimit = props.GetInt32("ResetLimit");
        }

        public override string ToString()
        {
            return (Name);
        }
        public override int GetHashCode() { return this.Name.GetHashCode(); }
        public override bool Equals(object obj)
        {
            RealTimeComputerSystem rtp = obj as RealTimeComputerSystem;
            if (null == rtp) return (false);
            return (rtp.Name == this.Name);
        }
        internal override void Merge(RealTimeGridData data)
        {
            RealTimeComputerSystem rtp = data as RealTimeComputerSystem;
            if (null == rtp) return;
            if (!this.Equals(rtp)) return;
            base.Merge(data);
            AdminPasswordStatus = rtp.AdminPasswordStatus;
            BootupState = rtp.BootupState;
            Description = rtp.Description;
            DNSHostName = rtp.DNSHostName;
            Domain = rtp.Domain;
            Manufacturer = rtp.Manufacturer;
            Model = rtp.Model;
            Status = rtp.Status;
            SystemType = rtp.SystemType;
            UserName = rtp.UserName;
            ChassisBootupState = rtp.ChassisBootupState;
            DomainRole = rtp.DomainRole;
            FrontPanelResetStatus = rtp.FrontPanelResetStatus;
            KeyboardPasswordStatus = rtp.KeyboardPasswordStatus;
            NumberOfLogicalProcessors = rtp.NumberOfLogicalProcessors;
            NumberOfProcessors = rtp.NumberOfProcessors;
            PCSystemType = rtp.PCSystemType;
            PowerOnPasswordStatus = rtp.PowerOnPasswordStatus;
            PowerState = rtp.PowerState;
            PowerSupplyState = rtp.PowerSupplyState;
            ResetCapability = rtp.ResetCapability;
            ThermalState = rtp.ThermalState;
            WakeUpType = rtp.WakeUpType;
            TotalPhysicalMemory = rtp.TotalPhysicalMemory;
            EnableDaylightSavingsTime = rtp.EnableDaylightSavingsTime;
            NetworkServerModeEnabled = rtp.NetworkServerModeEnabled;
            PartOfDomain = rtp.PartOfDomain;
            CurrentTimeZone = rtp.CurrentTimeZone;
            PauseAfterReset = rtp.PauseAfterReset;
            ResetCount = rtp.ResetCount;
            ResetLimit = rtp.ResetLimit;
        }
    }
}
