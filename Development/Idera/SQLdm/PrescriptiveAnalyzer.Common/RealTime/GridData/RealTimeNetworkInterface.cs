using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Idera.SQLdoctor.Common.WMI;

namespace Idera.SQLdoctor.Common.RealTime.GridData
{
    [Serializable]
    internal class RealTimeNetworkInterface : RealTimeGridData
    {
        private WmiPerfCounterBulkCount _bytesReceivedPersec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _bytesSentPersec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _packetsSentPersec = new WmiPerfCounterBulkCount();
        private WmiPerfCounterBulkCount _packetsReceivedPersec = new WmiPerfCounterBulkCount();

        public string Name { get; private set; }
        public UInt64 CurrentBandwidth { get; private set; }
        public UInt32 OutputQueueLength { get; private set; }
        public UInt64 BytesReceivedPersec { get { return (_bytesReceivedPersec.GetValue(this)); } }
        public UInt64 BytesSentPersec { get { return (_bytesSentPersec.GetValue(this)); } }
        public UInt64 PacketsReceivedPersec { get { return (_packetsReceivedPersec.GetValue(this)); } }
        public UInt64 PacketsSentPersec { get { return (_packetsSentPersec.GetValue(this)); } }
        public UInt64 PacketsReceivedErrors { get; private set; }
        public UInt64 PacketsReceivedDiscarded { get; private set; }
        public UInt64 PacketsOutboundErrors { get; private set; }
        public UInt64 PacketsOutboundDiscarded { get; private set; }

        public RealTimeNetworkInterface() { }

        internal override bool MultipleSamplesNeeded() { return (true); }
        internal override string GetWmiClassName() { return "Win32_PerfRawData_Tcpip_NetworkInterface"; }
        internal override string[] GetPropNames()
        {
            return (new string[] { 
                                    "Name", 
                                    "BytesReceivedPersec",
                                    "BytesSentPersec",
                                    "CurrentBandwidth", 
                                    "Frequency_Sys100NS",
                                    "Timestamp_Sys100NS",
                                    "OutputQueueLength", 
                                    "PacketsReceivedPersec",
                                    "PacketsSentPersec",
                                    "PacketsReceivedErrors", 
                                    "PacketsReceivedDiscarded", 
                                    "PacketsOutboundErrors", 
                                    "PacketsOutboundDiscarded", 
                                    });
        }

        internal override void SetProps(IProvideGridData props) 
        {
            base.SetProps(props);
            Name = props.GetString("Name");
            CurrentBandwidth = props.GetUInt64("CurrentBandwidth");
            _bytesReceivedPersec.UpdateValue(props.GetString("BytesReceivedPersec"));
            _bytesSentPersec.UpdateValue(props.GetString("BytesSentPersec"));
            _packetsSentPersec.UpdateValue(props.GetString("PacketsSentPersec"));
            _packetsReceivedPersec.UpdateValue(props.GetString("PacketsReceivedPersec"));
            OutputQueueLength = props.GetUInt32("OutputQueueLength");
            PacketsReceivedErrors = props.GetUInt64("PacketsReceivedErrors");
            PacketsReceivedDiscarded = props.GetUInt64("PacketsReceivedDiscarded");
            PacketsOutboundErrors = props.GetUInt64("PacketsOutboundErrors");
            PacketsOutboundDiscarded = props.GetUInt64("PacketsOutboundDiscarded");
        }

        public override int GetHashCode() { return this.Name.GetHashCode(); }
        public override bool Equals(object obj)
        {
            RealTimeNetworkInterface rtn = obj as RealTimeNetworkInterface;
            if (null == rtn) return (false);
            if (rtn.Name == this.Name) return (true);
            return (false);
        }
        internal override void Merge(RealTimeGridData data)
        {
            RealTimeNetworkInterface rtn = data as RealTimeNetworkInterface;
            if (null == rtn) return;
            if (!this.Equals(rtn)) return;
            base.Merge(data);
            _bytesReceivedPersec.UpdateValue(rtn._bytesReceivedPersec.GetCurrentValue());
            _bytesSentPersec.UpdateValue(rtn._bytesSentPersec.GetCurrentValue());
            _packetsReceivedPersec.UpdateValue(rtn._packetsReceivedPersec.GetCurrentValue());
            _packetsSentPersec.UpdateValue(rtn._packetsSentPersec.GetCurrentValue());

            CurrentBandwidth = rtn.CurrentBandwidth;
            OutputQueueLength = rtn.OutputQueueLength;
            PacketsReceivedErrors = rtn.PacketsReceivedErrors;
            PacketsReceivedDiscarded = rtn.PacketsReceivedDiscarded;
            PacketsOutboundErrors = rtn.PacketsOutboundErrors;
            PacketsOutboundDiscarded = rtn.PacketsOutboundDiscarded;
        }

    }
}
