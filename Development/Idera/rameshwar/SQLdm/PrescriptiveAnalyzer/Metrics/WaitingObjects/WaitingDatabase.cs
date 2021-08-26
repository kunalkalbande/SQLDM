using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics.WaitingObjects
{
    public class WaitingDatabase
    {
        private class PagesToResource : Dictionary<UInt64, WaitingResource> { }
        private class FilesToPages : Dictionary<UInt32, PagesToResource> { }

        public UInt32 ID { get; private set; }
        public int Count { get; private set; }

        private FilesToPages _resources = null;

        public WaitingDatabase(UInt32 id) { ID = id; }
        public WaitingDatabase(WaitingDatabase wd) 
        { 
            ID = wd.ID;
            Add(wd);
        }
        public void Add(WaitingResource wr)
        {
            ++Count;
            if (null == _resources) _resources = new FilesToPages();
            PagesToResource pr = null;
            if (_resources.Count > 100) { System.Diagnostics.Debug.Assert(false, "Too many unique files!"); return; }
            if (!_resources.TryGetValue(wr.File, out pr)) { _resources.Add(wr.File, pr = new PagesToResource());}
            WaitingResource w = null;
            if (pr.Count > 1000) { System.Diagnostics.Debug.Assert(false, "Too many unique pages!"); return; }
            if (!pr.TryGetValue(wr.Page, out w)) 
            { 
                pr.Add(wr.Page, new WaitingResource(wr));
                return;
            }
            w.Add(wr);
        }
        public void Add(WaitingDatabase wd)
        {
            System.Diagnostics.Debug.Assert(ID == wd.ID);
            Count += wd.Count;
            if (null != wd._resources)
            {
                foreach (PagesToResource pr in wd._resources.Values)
                {
                    foreach (WaitingResource wr in pr.Values) { Add(wr); }
                }
            }
        }
        public List<WaitingResource> GetSortedWaitingResources()
        {
            if (null == _resources) return (null);
            if (_resources.Count <= 0) return (null);
            List<WaitingResource> l = new List<WaitingResource>();
            foreach (PagesToResource pr in _resources.Values)
            {
                foreach (WaitingResource wr in pr.Values) { l.Add(wr); }
            }
            l.Sort(delegate(WaitingResource wr1, WaitingResource wr2) { return(wr1.Count.CompareTo(wr2.Count) * -1); });
            return (l);
        }
    }
}
