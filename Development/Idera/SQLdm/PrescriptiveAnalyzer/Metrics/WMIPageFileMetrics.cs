using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIPageFileMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMIPageFileMetrics");
        private Dictionary<string, WMIPageFileSnapshots> _pageFiles = new Dictionary<string, WMIPageFileSnapshots>();

        public int GetPageFileCount(string predicate)
        {
            int count = 0;
            foreach (string str in _pageFiles.Keys)
            {
                if (str.StartsWith(predicate, StringComparison.InvariantCultureIgnoreCase))
                {
                    count++;
                }
            }
            return count;
        }

        //public IEnumerable<string> FindPageFileNames(string predicate)
        //{
        //    return _pageFiles.Keys.Where(predicate);
        //}

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            AddSnapshot(snapshot.WmiPageFileSnapshotValueStartup);
            AddSnapshot(snapshot.WmiPageFileSnapshotValueShutdown);
        }

        private void AddSnapshot(Idera.SQLdm.Common.Snapshots.WmiPageFileSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMIPageFileMetrics not added : " + snapshot.Error); return; }
            if (snapshot.WmiPageFile != null && snapshot.WmiPageFile.Rows.Count > 0)
            {
                WMIPageFileSnapshots pcs = null;
                for (int index = 0; index < snapshot.WmiPageFile.Rows.Count; index++)
                {
                    WMIPageFile obj = new WMIPageFile();

                    try
                    {
                        obj.Name = (string)snapshot.WmiPageFile.Rows[index]["Name"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    if (!_pageFiles.TryGetValue(obj.Name, out pcs))
                    {
                        _pageFiles.Add(obj.Name, pcs = new WMIPageFileSnapshots());
                    }
                    pcs.Add(obj);
                }
            }
        }

    }

    internal class WMIPageFileSnapshots : List<WMIPageFile>
    {
    }

    internal class WMIPageFile
    {
        public string Name { get;  set; }
        public WMIPageFile() { }
    }
}
