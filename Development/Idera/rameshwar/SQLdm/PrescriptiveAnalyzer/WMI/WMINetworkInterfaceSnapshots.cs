using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WMI
{
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
}
