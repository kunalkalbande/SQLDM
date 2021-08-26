using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMINetworkInterfaceMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMINetworkInterfaceMetrics");
        private Dictionary<string, WMINetworkInterfaceSnapshots> _nics = new Dictionary<string, WMINetworkInterfaceSnapshots>();

        public IEnumerable<WMINetworkInterfaceSnapshots> Cards { get { return (_nics.Values); } }
        public int ActiveNetworkCards
        {
            get
            {
                int count = 0;
                foreach (WMINetworkInterfaceSnapshots nic in _nics.Values)
                {
                    if (nic.IsActive()) ++count;
                }
                return (count);
            }
        }

        public UInt64 TotalNetworkBandwidth
        {
            get
            {
                UInt64 bandwidth = 0;
                foreach (WMINetworkInterfaceSnapshots nic in _nics.Values)
                {
                    if (nic.IsActive()) bandwidth += nic.CurrentBandwidth();
                }
                return (bandwidth);
            }
        }

        public UInt64 TotalOutputQueueLength
        {
            get
            {
                UInt64 len = 0;
                foreach (WMINetworkInterfaceSnapshots nic in _nics.Values)
                {
                    len += nic.TotalOutputQueueLength();
                }
                return (len);
            }
        }

        public UInt64 TotalPacketsPerSec
        {
            get
            {
                UInt64 p = 0;
                foreach (WMINetworkInterfaceSnapshots nic in _nics.Values)
                {
                    p += nic.TotalPacketsPerSec;
                }
                return (p);
            }
        }

        public UInt64 AvgPacketsPerSec_TotalForAllCards
        {
            get
            {
                UInt64 p = 0;
                foreach (WMINetworkInterfaceSnapshots nic in _nics.Values)
                {
                    p += nic.AvgPacketsPerSec();
                }
                return (p);
            }
        }

        public double MaxAvgOutputQueueLength
        {
            get
            {
                double len = 0;
                double maxlen = 0;
                foreach (WMINetworkInterfaceSnapshots nic in _nics.Values)
                {
                    len = nic.AvgOutputQueueLength();
                    if (len > maxlen) maxlen = len;
                }
                return (maxlen);
            }
        }

        public List<string> GetCardsWithHighQueueLength()
        {
            List<string> cards = new List<string>();
            foreach (WMINetworkInterfaceSnapshots nic in _nics.Values)
            {
                if (nic.AvgOutputQueueLength() > Settings.Default.Net_MaxAvgQueueLength)
                {
                    cards.Add(nic.Name);
                }
            }
            return (cards);
        }

        public IEnumerable<WMINetworkInterfaceSnapshots> GetCardsWithPacketErrors()
        {
            foreach (WMINetworkInterfaceSnapshots nic in _nics.Values) if (nic.HasPacketErrors()) yield return (nic);
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            AddSnapshot(snapshot.WmiNetworkInterfaceSnapshotValueStartup);
            AddSnapshot(snapshot.WmiNetworkInterfaceSnapshotValueShutdown);
        }

        private void AddSnapshot(Idera.SQLdm.Common.Snapshots.WmiNetworkInterfaceSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMINetworkInterfaceMetrics not added : " + snapshot.Error); return; }
            
            if (snapshot.WmiNetworkInterface != null && snapshot.WmiNetworkInterface.Rows.Count > 0)
            {
                WMINetworkInterfaceSnapshots nis = null;
                for (int index = 0; index < snapshot.WmiNetworkInterface.Rows.Count; index++)
                {
                    WMINetworkInterface obj = new WMINetworkInterface();
                    try
                    {
                        obj.Name = (string)snapshot.WmiNetworkInterface.Rows[index]["Name"];
                        obj.CurrentBandwidth = (ulong)snapshot.WmiNetworkInterface.Rows[index]["CurrentBandwidth"];
                        obj.OutputQueueLength = (uint)snapshot.WmiNetworkInterface.Rows[index]["OutputQueueLength"];
                        obj.PacketsPerSec = (uint)snapshot.WmiNetworkInterface.Rows[index]["PacketsPerSec"];
                        obj.PacketsReceivedErrors = (uint)snapshot.WmiNetworkInterface.Rows[index]["PacketsReceivedErrors"];
                        obj.PacketsOutboundErrors = (uint)snapshot.WmiNetworkInterface.Rows[index]["PacketsOutboundErrors"];
                        obj.BytesTotalPerSec = (ulong)snapshot.WmiNetworkInterface.Rows[index]["BytesTotalPerSec"];
                        obj.Frequency_Sys100NS = (ulong)snapshot.WmiNetworkInterface.Rows[index]["Frequency_Sys100NS"];
                        obj.Timestamp_Sys100NS = (ulong)snapshot.WmiNetworkInterface.Rows[index]["Timestamp_Sys100NS"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    if (!_nics.TryGetValue(obj.Name, out nis)) _nics.Add(obj.Name, nis = new WMINetworkInterfaceSnapshots());
                    nis.Add(obj);
                }
            }
        }
    }

    public class WMINetworkInterfaceSnapshots : List<WMINetworkInterface>
    {
        internal double AvgOutputQueueLength()
        {
            if (this.Count <= 0) return (0);
            return ((double)TotalOutputQueueLength() / this.Count);
        }
        internal UInt64 TotalOutputQueueLength()
        {
            if (this.Count <= 0) return (0);
            UInt32 len = 0;
            foreach (WMINetworkInterface snapshot in this) len += snapshot.OutputQueueLength;
            return (len);
        }
        internal UInt32 AvgPacketsPerSec()
        {
            if (this.Count <= 1) return (0);
            //var last = this.Last();
            var last = this[this.Count - 1];
            if (null == last) return (0);
            var Nx = last.PacketsPerSec;
            var N0 = this[0].PacketsPerSec;
            var Dx = last.Timestamp_Sys100NS;
            var D0 = this[0].Timestamp_Sys100NS;
            var F = last.Frequency_Sys100NS;
            if (0 == F) F = 100;
            var D = Dx - D0;
            var N = Nx - N0;
            if (0 == D) return (N);
            // Average: (Nx - N0) / ((Dx - D0) / F)
            return (UInt32)Math.Round(((double)N / ((double)D / F)));
        }
        internal UInt64 CurrentBandwidth()
        {
            if (this.Count <= 0) return (0);
            UInt64 bandwidth = 0;
            foreach (WMINetworkInterface snapshot in this) { if (snapshot.CurrentBandwidth > bandwidth) bandwidth = snapshot.CurrentBandwidth; }
            return (bandwidth);
        }
        internal bool IsActive()
        {
            if (this.Count <= 0) return (false);
            UInt64 bytes = this[0].BytesTotalPerSec;
            foreach (WMINetworkInterface snapshot in this) { if (snapshot.BytesTotalPerSec != bytes) return (true); }
            return (false);
        }
        internal bool HasPacketErrors()
        {
            if (this.Count <= 1) return (false);
            UInt32 oe = this[0].PacketsOutboundErrors;
            UInt32 re = this[0].PacketsReceivedErrors;
            //WMINetworkInterface last = this.Last();
            WMINetworkInterface last = this[this.Count - 1];
            if (last.PacketsOutboundErrors > oe) return (true);
            if (last.PacketsReceivedErrors > re) return (true);
            return (false);
        }
        internal string Name
        {
            get
            {
                if (this.Count <= 0) return (string.Empty);
                return (this[0].Name);
            }
        }
        internal UInt32 TotalPacketsWithOutboundErrors
        {
            get
            {
                if (this.Count <= 1) return (0);
                //return (this.Last().PacketsOutboundErrors - this[0].PacketsOutboundErrors);
                return (this[this.Count - 1].PacketsOutboundErrors - this[0].PacketsOutboundErrors);
            }
        }
        internal UInt32 TotalPacketsWithReceivedErrors
        {
            get
            {
                if (this.Count <= 1) return (0);
                //return (this.Last().PacketsReceivedErrors - this[0].PacketsReceivedErrors);
                return (this[this.Count - 1].PacketsReceivedErrors - this[0].PacketsReceivedErrors);
            }
        }
        internal UInt32 TotalPacketsPerSec
        {
            get
            {
                if (this.Count <= 1) return (0);
                //return (this.Last().PacketsPerSec - this[0].PacketsPerSec);
                return (this[this.Count - 1].PacketsPerSec - this[0].PacketsPerSec);
            }
        }
    }

    public class WMINetworkInterface
    {
        public string Name { get;  set; }
        public UInt64 CurrentBandwidth { get;  set; }
        public UInt32 OutputQueueLength { get;  set; }
        public UInt32 PacketsPerSec { get;  set; }
        public UInt32 PacketsReceivedErrors { get;  set; }
        public UInt32 PacketsOutboundErrors { get;  set; }
        public UInt64 BytesTotalPerSec { get;  set; }
        public UInt64 Frequency_Sys100NS { get;  set; }
        public UInt64 Timestamp_Sys100NS { get;  set; }

        public WMINetworkInterface() { }

    }

}
