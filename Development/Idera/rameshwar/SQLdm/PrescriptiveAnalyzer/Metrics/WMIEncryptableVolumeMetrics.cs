using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIEncryptableVolumeMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMIEncryptableVolumeMetrics");
        private Dictionary<string, WMIEncryptableVolumeSnapshots> _volumes = new Dictionary<string, WMIEncryptableVolumeSnapshots>();

        public string[] DriveLetters 
        { 
            get 
            {
                string[] array = new string[_volumes.Count];
                int index = 0;
                foreach (string str in _volumes.Keys)
                {
                    array[index] = str;
                    index++;
                }
                //return _volumes.Keys.ToArray();
                return array;
            }
        }

        internal WMIEncryptableVolume GetVolume(string driveLetter)
        {
            WMIEncryptableVolumeSnapshots s = null;
            if (!_volumes.TryGetValue(driveLetter, out s)) return (null);
            if (null == s) return (null);
            if (s.Count <= 0) return (null);
            return (s[0]);
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMIEncryptableVolumeMetrics not added : " + snapshot.Error); return; }
            
            if (snapshot.WmiEncryptableVolumeSnapshotValueShutdown == null) { return; }
            if (snapshot.WmiEncryptableVolumeSnapshotValueShutdown.WmiEncryptableVolume != null && snapshot.WmiEncryptableVolumeSnapshotValueShutdown.WmiEncryptableVolume.Rows.Count > 0)
            {
                WMIEncryptableVolumeSnapshots s = null;
                for (int index = 0; index < snapshot.WmiEncryptableVolumeSnapshotValueShutdown.WmiEncryptableVolume.Rows.Count; index++)
                {
                    WMIEncryptableVolume obj = new WMIEncryptableVolume();

                    try
                    {
                        obj.DriveLetter = (string)snapshot.WmiEncryptableVolumeSnapshotValueShutdown.WmiEncryptableVolume.Rows[index]["DriveLetter"];
                        obj.ProtectionStatus = (uint)snapshot.WmiEncryptableVolumeSnapshotValueShutdown.WmiEncryptableVolume.Rows[index]["ProtectionStatus"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    if (!_volumes.TryGetValue(obj.DriveLetter, out s)) _volumes.Add(obj.DriveLetter, s = new WMIEncryptableVolumeSnapshots());
                    s.Add(obj);
                }
            }
        }

    }

    internal class WMIEncryptableVolumeSnapshots : List<WMIEncryptableVolume>
    {
    }

    internal class WMIEncryptableVolume
    {
        public string DriveLetter { get;  set; }
        public UInt32 ProtectionStatus { get;  set; }

        public WMIEncryptableVolume() { }
    }
}
