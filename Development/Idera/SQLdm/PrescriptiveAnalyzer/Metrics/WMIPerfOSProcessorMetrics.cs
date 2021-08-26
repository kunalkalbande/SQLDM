using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIPerfOSProcessorMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMIPerfOSProcessorMetrics");
        private Dictionary<string, WMIPerfOSProcessorSnapshots> _procs = new Dictionary<string, WMIPerfOSProcessorSnapshots>();

        public int GetAvgPrivilegedTime()
        {
            WMIPerfOSProcessorSnapshots p = GetSnapshot("_Total");
            if (null != p) return (p.AvgPrivilegedTime);
            return (0);
        }
        public double GetAvgPercentInterruptTime()
        {
            WMIPerfOSProcessorSnapshots p = GetSnapshot("_Total");
            if (null != p) return (p.AvgPercentInterruptTime);
            return (0);
        }
        public UInt32 GetAvgInterruptsPerSec()
        {
            WMIPerfOSProcessorSnapshots p = GetSnapshot("_Total");
            if (null != p) return (p.AvgInterruptsPerSec);
            return (0);
        }
        public int GetAvgCpuHigh()
        {
            int high = 0;
            foreach (WMIPerfOSProcessorSnapshots s in _procs.Values)
            {
                if (null != s) high = Math.Max(s.AvgCpu, high);
            }
            return (high);
        }
        public int GetAvgCpu(UInt64 mask)
        {
            if (0 == mask) return (GetAvgCpu("_Total"));
            int totalCpu = 0;
            int cpu = 0;
            int count = 0;
            foreach (KeyValuePair<string, WMIPerfOSProcessorSnapshots> p in _procs)
            {
                if (0 == string.Compare(p.Key, "_Total", true)) continue;
                try
                {
                    cpu = Convert.ToInt32(p.Value.Name);
                    if (0 != (mask & ((UInt64)1 << cpu)))
                    {
                        totalCpu += p.Value.AvgCpu;
                        ++count;
                    }
                }
                catch { }
            }
            if (count <= 0) return (0);
            return (totalCpu / count);
        }
        public int GetAvgCpu(string cpu)
        {
            WMIPerfOSProcessorSnapshots p = GetSnapshot(cpu);
            if (null != p) return (p.AvgCpu);
            return (0);
        }
        internal WMIPerfOSProcessorSnapshots GetSnapshot(string name)
        {
            WMIPerfOSProcessorSnapshots p;
            if (_procs.TryGetValue(name, out p))
            {
                return (p);
            }
            return (null);
        }
        internal int GetProcessorCount(UInt64 mask)
        {
            int count = 0;
            int cpu = 0;
            foreach (KeyValuePair<string, WMIPerfOSProcessorSnapshots> p in _procs)
            {
                if (0 == string.Compare(p.Key, "_Total", true)) continue;
                if (0 == mask)
                {
                    ++count;
                }
                else
                {
                    try
                    {
                        cpu = Convert.ToInt32(p.Value.Name);
                        if (0 != (mask & ((UInt64)1 << cpu)))
                        {
                            ++count;
                        }
                    }
                    catch { }
                }
            }
            return (count > 0 ? count : 1);
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            AddSnapshot(snapshot.WmiPerfOSProcessorSnapshotValueStartup);
            AddSnapshot(snapshot.WmiPerfOSProcessorSnapshotValueShutdown);
        }

        private void AddSnapshot(Idera.SQLdm.Common.Snapshots.WmiPerfOSProcessorSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMIPerfOSProcessorMetrics not added : " + snapshot.Error); return; }
            
            if (snapshot.WmiPerfOSProcessor != null && snapshot.WmiPerfOSProcessor.Rows.Count > 0)
            {
                WMIPerfOSProcessorSnapshots pcs = null;
                for (int index = 0; index < snapshot.WmiPerfOSProcessor.Rows.Count; index++)
                {
                    WMIPerfOSProcessor obj = new WMIPerfOSProcessor();

                    try
                    {
                        obj.Name = (string)snapshot.WmiPerfOSProcessor.Rows[index]["Name"];
                        obj.InterruptsPerSec = (uint)snapshot.WmiPerfOSProcessor.Rows[index]["InterruptsPerSec"];
                        obj.PercentProcessorTime = (ulong)snapshot.WmiPerfOSProcessor.Rows[index]["PercentProcessorTime"];
                        obj.PercentPrivilegedTime = (ulong)snapshot.WmiPerfOSProcessor.Rows[index]["PercentPrivilegedTime"];
                        obj.PercentInterruptTime = (ulong)snapshot.WmiPerfOSProcessor.Rows[index]["PercentInterruptTime"];
                        obj.Frequency_Sys100NS = (ulong)snapshot.WmiPerfOSProcessor.Rows[index]["Frequency_Sys100NS"];
                        obj.Timestamp_Sys100NS = (ulong)snapshot.WmiPerfOSProcessor.Rows[index]["Timestamp_Sys100NS"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    if (!_procs.TryGetValue(obj.Name, out pcs)) _procs.Add(obj.Name, pcs = new WMIPerfOSProcessorSnapshots());
                    pcs.Add(obj);
                }
            }
        }
    }

    internal class WMIPerfOSProcessorSnapshots : List<WMIPerfOSProcessor>
    {
        internal string Name
        {
            get
            {
                if (this.Count <= 0) return (string.Empty);
                return (this[0].Name);
            }
        }
        internal UInt32 AvgInterruptsPerSec
        {
            get
            {
                if (this.Count <= 1) return (0);
                //var last = this.Last();
                var last = this[this.Count - 1];
                if (null == last) return (0);
                var Nx = last.InterruptsPerSec;
                var N0 = this[0].InterruptsPerSec;
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
        }

        internal int AvgPrivilegedTime
        {
            get
            {
                if (this.Count <= 1) return (0);
                try
                {
                    //WMIPerfOSProcessor last = this.Last();
                    WMIPerfOSProcessor last = this[this.Count - 1];
                    UInt64 x0 = this[0].PercentPrivilegedTime;
                    UInt64 y0 = this[0].Timestamp_Sys100NS;
                    UInt64 x1 = last.PercentPrivilegedTime;
                    UInt64 y1 = last.Timestamp_Sys100NS;
                    //Average: (Nx - N0) / (Dx - D0) x 100
                    return (int)Math.Round((((double)(x1 - x0) / (double)(y1 - y0)) * 100));
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log("WMIPerfOSProcessorSnapshots.AvgPrivilegedTime Exception:", ex);
                    return (0);
                }
            }
        }

        internal double AvgPercentInterruptTime
        {
            get
            {
                if (this.Count <= 1) return (0);
                try
                {
                    //WMIPerfOSProcessor last = this.Last();
                    WMIPerfOSProcessor last = this[this.Count - 1];
                    UInt64 x0 = this[0].PercentInterruptTime;
                    UInt64 y0 = this[0].Timestamp_Sys100NS;
                    UInt64 x1 = last.PercentInterruptTime;
                    UInt64 y1 = last.Timestamp_Sys100NS;
                    //Average: (Nx - N0) / (Dx - D0) x 100
                    return (((double)(x1 - x0) / (double)(y1 - y0)) * 100);
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log("WMIPerfOSProcessorSnapshots.AvgPercentInterruptTime Exception:", ex);
                    return (0);
                }
            }
        }

        internal int AvgCpu
        {
            get
            {
                if (this.Count <= 1) return (0);
                try
                {
                    //WMIPerfOSProcessor last = this.Last();
                    WMIPerfOSProcessor last = this[this.Count - 1];
                    UInt64 x0 = this[0].PercentProcessorTime;
                    UInt64 y0 = this[0].Timestamp_Sys100NS;
                    UInt64 x1 = last.PercentProcessorTime;
                    UInt64 y1 = last.Timestamp_Sys100NS;
                    return (int)Math.Round((100 * (1 - (double)(x1 - x0) / (double)(y1 - y0))));
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log("WMIPerfOSProcessorSnapshots.AvgCpu Exception:", ex);
                    return (0);
                }
            }
        }
    }

    internal class WMIPerfOSProcessor
    {
        public string Name { get;  set; }
        public UInt32 InterruptsPerSec { get;  set; }
        public UInt64 PercentProcessorTime { get;  set; }
        public UInt64 PercentPrivilegedTime { get;  set; }
        public UInt64 PercentInterruptTime { get;  set; }
        public UInt64 Frequency_Sys100NS { get;  set; }
        public UInt64 Timestamp_Sys100NS { get;  set; }
        public WMIPerfOSProcessor() { }
    }
}
