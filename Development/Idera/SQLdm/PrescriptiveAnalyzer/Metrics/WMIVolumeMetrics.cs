using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using System.Data;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIVolumeMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMIVolumeMetrics");
        private Dictionary<string, WMIVolume> _volumes = new Dictionary<string, WMIVolume>();

        public WMIVolume GetVolume(string name)
        {
            WMIVolume result = null;
            string lower = name.ToLower();
            //-----------------------------------------------------------
            // Test to see if we can find the volume based on the name given.
            //
            if (!_volumes.TryGetValue(lower, out result))
            {
                //-----------------------------------------------------------
                // If we cannot find the volume with the matching name, try adding
                // or removing a trailing \ based on if a trailing slash is in
                // the given name.
                //
                if (lower.EndsWith(@"\")) _volumes.TryGetValue(lower.TrimEnd('\\'), out result); else _volumes.TryGetValue(lower + @"\", out result);
            }
            return result;
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMIVolumeMetrics not added : " + snapshot.Error); return; }
            
            if (snapshot.WmiVolumeSnapshotValueStartup == null) { return; }
            if (snapshot.WmiVolumeSnapshotValueStartup.WmiVolume != null && snapshot.WmiVolumeSnapshotValueStartup.WmiVolume.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.WmiVolumeSnapshotValueStartup.WmiVolume.Rows.Count; index++)
                {
                    WMIVolume obj = new WMIVolume();
                    try
                    {
                        DataRow r = snapshot.WmiVolumeSnapshotValueStartup.WmiVolume.Rows[index];
                        obj.Name = DataHelper.ToString(r, "Name");
                        obj.BlockSize = DataHelper.ToUInt64(r, "BlockSize");
                        obj.FileSystem = DataHelper.ToString(r, "FileSystem");
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }

                    if (!String.IsNullOrEmpty(obj.Name))
                    {
                        string volumeName = obj.Name.ToLower();
                        if (_volumes.ContainsKey(volumeName))
                            _volumes.Remove(volumeName);

                        _volumes.Add(volumeName, obj);
                    }
                }
            }
        }

    }

    public class WMIVolume
    {
        public string Name { get;  set; }
        public ulong BlockSize { get;  set; }
        public string FileSystem { get;  set; }

        public WMIVolume() { }
    }

}
