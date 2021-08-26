using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
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
}
