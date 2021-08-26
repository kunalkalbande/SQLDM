using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using System.Diagnostics;

namespace Idera.SQLdm.PrescriptiveAnalyzer.SQL
{
    public class WaitInfo
    {
        public string Type { get; set; }
        public UInt64 Time { get; set; }
        public UInt64 SignalTime { get; set; }
        public WaitInfo() { }
        public WaitInfo(WaitInfo wi) 
        {
            Type = wi.Type;
            Time = wi.Time;
            SignalTime = wi.SignalTime;
        }
        public static WaitInfo operator -(WaitInfo wi1, WaitInfo wi2)
        {
            return (new WaitInfo() 
                                { 
                                    Type = wi1.Type, 
                                    Time = wi1.Time - wi2.Time, 
                                    SignalTime = wi1.SignalTime - wi2.SignalTime 
                                });
        }
        public override string ToString()
        {
            return (string.Format("{0} - {1}", Type, Time, SignalTime));
        }
    }

    public class WaitStats
    {
        private UInt64? _totalWaitTime = null;
        private Dictionary<string, WaitInfo> _data = new Dictionary<string, WaitInfo>();
        private List<WaitInfo> _sorted = null;
        private object _locksort = new object();

        public DateTime AsOf { get; private set; }
        public UInt64 TotalWaitTime
        {
            get 
            {
                if (null == _totalWaitTime)
                {
                    UInt64 t = 0;
                    foreach (WaitInfo wi in _data.Values) { t += wi.Time; }
                    _totalWaitTime = t;
                }
                return (_totalWaitTime.Value);
            }
        }

        private WaitStats() { }
        public WaitStats(SqlDataReader r) 
        {
            if (null == r) return;
            if (!r.Read()) { Debug.Assert(false, "Failed to read AsOf date"); return; }
            if (0 == string.Compare(r[0].ToString(), "AsOf"))
            {
          //      AsOf = DataHelper.ToDateTime(r, 1, CultureInfo.CreateSpecificCulture("en-US")); 
            }
            if (!r.NextResult()) { Debug.Assert(false, "Failed to read Wait Stats data"); return; }
            string wt;
            while (r.Read())
            {
                wt = DataHelper.ToString(r, "wait_type");
                _data[wt] = new WaitInfo() { 
                                            Type = wt, 
                                            Time = DataHelper.ToUInt64(r, "wait_time_ms"),
                                            SignalTime = DataHelper.ToUInt64(r, "signal_wait_time_ms"),
                                            };
            }
        }
        public static WaitStats operator -(WaitStats w1, WaitStats w2)
        {
            WaitStats ws = new WaitStats();
            ws.AsOf = ws.AsOf;
            WaitInfo wi2; 
            foreach (WaitInfo wi1 in w1._data.Values)
            {
                if (w2._data.TryGetValue(wi1.Type, out wi2))
                {
                    ws._data[wi1.Type] = wi1 - wi2;
                }
                else
                {
                    ws._data[wi1.Type] = new WaitInfo(wi1);
                }
            }
            return (ws);
        }

        public List<WaitInfo> Top(int n)
        {
            lock (_locksort)
            {
                if (null == _sorted)
                {
                    _sorted = new List<WaitInfo>(_data.Values);
                    _sorted.Sort(new WaitInfoComparer(false));
                }
                List<WaitInfo> l = new List<WaitInfo>(_sorted);
                if (_sorted.Count > n)
                {
                    l.RemoveRange(n, l.Count - n);
                }
                return (l);
            }
        }
        public WaitInfo GetWaitInfo(string[] wt, bool startsWith)
        {
            WaitInfo wi = null;
            System.Collections.IEnumerator e = wt.GetEnumerator();
            int count = 0;
            while (e.MoveNext())
            {
                count++;
            }

            if (!startsWith && (1 == count))
            //if (!startsWith && (1 == wt.Count()))
            {
                _data.TryGetValue(wt[0], out wi);
                return (wi);
            }
            wi = new WaitInfo();
            wi.Type = wt[0];
            foreach (WaitInfo w in _data.Values)
            {
                if (!startsWith)
                {
                    if (!Compare(w.Type, wt)) { continue; }
                }
                else if (!StartsWith(w.Type, wt)){continue;}
                wi.Time += w.Time;
                wi.SignalTime += w.SignalTime;
            }
            return (wi);
        }
        public static bool StartsWith(string p, string[] prefixes)
        {
            foreach (string prefix in prefixes) { if (p.StartsWith(prefix)) return (true); }
            return (false);
        }
        public static bool Compare(string p, string[] strings)
        {
            foreach (string s in strings) { if (0 == string.Compare(p, s, true)) return (true); }
            return (false);
        }
    }
    internal class WaitInfoComparer : IComparer<WaitInfo>
    {
        private bool _ascending = true;
        public WaitInfoComparer(bool ascending)
        {
            _ascending = ascending;
        }
        public int Compare(WaitInfo x, WaitInfo y)
        {
            int r = x.Time.CompareTo(y.Time);
            if (!_ascending) r *= -1;
            return (r);
        }
    }
}
