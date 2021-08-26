using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using System.Data;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIProcessorMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMIProcessorMetrics");
        private Dictionary<int, WMIProcessorSnapshots> _ps = new Dictionary<int, WMIProcessorSnapshots>();

        public UInt32 AddressWidth
        {
            get
            {
                foreach (WMIProcessorSnapshots s in _ps.Values)
                {
                    if (null != s) return (s.AddressWidth);
                }
                return (0);
            }
        }

        public bool Is64Bit { get { return (64 == AddressWidth); } }
        public bool Is32Bit { get { return (32 == AddressWidth); } }
        public double AvgFreqMaxPercentage
        {
            get
            {
                double avg = 0;
                int c = 0;
                foreach (WMIProcessorSnapshots s in _ps.Values)
                {
                    if (null != s)
                    {
                        avg += s.AvgClockSpeedMaxPercent;
                        ++c;
                    }
                }
                return ((0 == c) ? 0 : avg / c);
            }
        }

        public bool HitMaxClockSpeedOnAllProcessors
        {
            get
            {
                foreach (WMIProcessorSnapshots s in _ps.Values)
                {
                    if (null != s)
                    {
                        if (!s.HitMaxClockSpeed) return (false);
                    }
                }
                return (true);
            }
        }

        public bool SustainedMaxClockSpeed
        {
            get
            {
                foreach (WMIProcessorSnapshots s in _ps.Values)
                {
                    if (null != s)
                    {
                        if (!s.SustainedMaxClockSpeed) return (false);
                    }
                }
                return (true);
            }
        }

        public UInt64 TotalMaxClockSpeed
        {
            get
            {
                UInt64 t = 0;
                foreach (WMIProcessorSnapshots s in _ps.Values)
                {
                    if (null != s)
                    {
                        t += s.MaxClockSpeedHigh;
                    }
                }
                return (t);
            }
        }

        public UInt32 TotalNumberOfLogicalProcessors
        {
            get
            {
                UInt32 t = 0;
                foreach (WMIProcessorSnapshots s in _ps.Values)
                {
                    if (null != s)
                    {
                        t += s.NumberOfLogicalProcessors;
                    }
                }
                return (t);
            }
        }

        public UInt32 TotalNumberOfCores
        {
            get
            {
                UInt32 t = 0;
                foreach (WMIProcessorSnapshots s in _ps.Values)
                {
                    if (null != s)
                    {
                        t += s.NumberOfCores;
                    }
                }
                return (t);
            }
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMIProcessorMetrics not added : " + snapshot.Error); return; }            
            if (snapshot.WmiProcessorSnapshotValueInterval == null) { return; }
            if (snapshot.WmiProcessorSnapshotValueInterval.ListWmiProcessor != null)
                foreach (var dt in snapshot.WmiProcessorSnapshotValueInterval.ListWmiProcessor)
                    AddSnapshot(dt);
        }

        private void AddSnapshot(DataTable dt)
        {
            if (dt == null) { return; }
            if (dt.Rows.Count > 0)
            {
                int id = 0;
                WMIProcessorSnapshots ps = null;
                for (int index = 0; index < dt.Rows.Count; index++)
                {
                    WMIProcessor obj = new WMIProcessor();
                    try
                    {
                        obj.AddressWidth = (uint)dt.Rows[index]["AddressWidth"];
                        obj.NumberOfCores = (uint)dt.Rows[index]["NumberOfCores"];
                        obj.NumberOfLogicalProcessors = (uint)dt.Rows[index]["NumberOfLogicalProcessors"];
                        obj.CurrentClockSpeed = (uint)dt.Rows[index]["CurrentClockSpeed"];
                        obj.MaxClockSpeed = (uint)dt.Rows[index]["MaxClockSpeed"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    if (!_ps.TryGetValue(id, out ps)) _ps.Add(id, ps = new WMIProcessorSnapshots());
                    ps.Add(obj);
                    ++id;
                }
            }
        }
    }

    internal class WMIProcessorSnapshots : List<WMIProcessor>
    {
        public UInt32 AddressWidth
        {
            get
            {
                if (this.Count <= 0) return (0);
                return (this[0].AddressWidth);
            }
        }
        public bool SustainedMaxClockSpeed
        {
            get
            {
                foreach (WMIProcessor p in this)
                {
                    if (p.CurrentClockSpeed < p.MaxClockSpeed) return (false);
                }
                return (true);
            }
        }
        public bool HitMaxClockSpeed
        {
            get
            {
                foreach (WMIProcessor p in this)
                {
                    if (p.CurrentClockSpeed >= p.MaxClockSpeed) return (true);
                }
                return (false);
            }
        }
        public double AvgClockSpeed
        {
            get
            {
                double speed = 0;
                int c = 0;
                foreach (WMIProcessor p in this)
                {
                    if (null != p)
                    {
                        speed += p.CurrentClockSpeed;
                        ++c;
                    }
                }
                return (speed / c);
            }
        }
        public double AvgClockSpeedMaxPercent
        {
            get
            {
                UInt64 maxSpeed = MaxClockSpeedHigh;
                return ((maxSpeed <= 0) ? 0 : ((AvgClockSpeed * 100.0) / maxSpeed));
            }
        }
        public UInt32 NumberOfLogicalProcessors
        {
            get
            {
                if (this.Count <= 0) return (0);
                return ((this[0].NumberOfLogicalProcessors < this[0].NumberOfCores) ? this[0].NumberOfCores : this[0].NumberOfLogicalProcessors);
            }
        }
        public UInt32 NumberOfCores
        {
            get
            {
                if (this.Count <= 0) return (0);
                return (this[0].NumberOfCores);
            }
        }
        public UInt64 MaxClockSpeedHigh
        {
            get
            {
                UInt64 t = 0;
                foreach (WMIProcessor p in this)
                {
                    if (null != p) t = Math.Max(t, p.MaxClockSpeed);
                }
                return (t);
            }
        }
        public UInt64 CurrentClockSpeedHigh
        {
            get
            {
                UInt64 t = 0;
                foreach (WMIProcessor p in this)
                {
                    if (null != p) t = Math.Max(t, p.CurrentClockSpeed);
                }
                return (t);
            }
        }
        public UInt64 CurrentClockSpeedLow
        {
            get
            {
                UInt64 t = 0;
                foreach (WMIProcessor p in this)
                {
                    if (null != p)
                    {
                        if (0 == t)
                            t = p.CurrentClockSpeed;
                        else
                            t = Math.Min(t, p.CurrentClockSpeed);
                    }
                }
                return (t);
            }
        }
    }

    internal class WMIProcessor
    {
        public UInt32 AddressWidth { get;  set; }
        public UInt32 NumberOfLogicalProcessors { get;  set; }
        public UInt32 NumberOfCores { get;  set; }
        public UInt32 CurrentClockSpeed { get;  set; }
        public UInt32 MaxClockSpeed { get;  set; }

        public WMIProcessor() { }
    }
}
