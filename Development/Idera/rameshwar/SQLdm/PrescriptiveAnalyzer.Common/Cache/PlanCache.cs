using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Cache
{
    public class PlanCache : IDisposable
    {
        private static BBS.TracerX.Logger _logX = BBS.TracerX.Logger.GetLogger("PlanCache");

        private CacheFile _file = null;
        private PlanCache() { }
        public PlanCache(string fileName, int minSizeMB, int maxSizeMB, int growSizeMB) { _file = new CacheFile(fileName, minSizeMB, maxSizeMB, growSizeMB); }

        public long AddPlan(string s)
        {
            return (_file.Append(s));
        }

        public string GetXmlPlan(long key)
        {
            if (0 > key) return (string.Empty);
            long len = _file.ReadInt64(key);
            return (_file.ReadString(key + sizeof(Int64), len));
        }

        public ShowPlanXML GetPlan(long key)
        {
            if (0 > key) return (null);
            long len = _file.ReadInt64(key);
            string plan = _file.ReadString(key + sizeof(Int64), len);
            if (string.IsNullOrEmpty(plan)) return (null);

            try
            {
                return ((string.IsNullOrEmpty(plan)) ? null : ShowPlanUtils.GetPlan(plan));
            }
            catch (Exception ex)
            {
                using (_logX.DebugCall("GetPlan(long key) Error:" + ex.Message))
                {
                    _logX.Info(plan);
                    return (null);
                }
            }

        }

        public void Dispose()
        {
            if (null != _file) { using (_file) { } }
            _file = null;
        }
    }
}
