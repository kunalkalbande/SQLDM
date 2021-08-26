using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Cache;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents
{
    public class TraceEventPlan
    {
        private static Logger _logX = Logger.GetLogger("TraceEventPlan");
        private PlanCache _planCache = null;
        private long _planCacheKey = -1;
        public string XmlPlan
        {
            get
            {
                return (_planCache.GetXmlPlan(_planCacheKey));
            }
        }
        public ShowPlanXML Plan 
        { 
            get 
            {
                return (_planCache.GetPlan(_planCacheKey));
            } 
        }
        public readonly TEBase Event;
        public Exception Error { get; private set; }
        private TraceEventPlan() { }
        public TraceEventPlan(SqlConnection conn, TEBase e, PlanCache planCache) 
        {
            _planCache = planCache;
            Event = e;
            try
            {
                TEWorstTSQL w = e as TEWorstTSQL;
                string xml = string.Empty;
                if (null != w)
                {
                    xml = w.XMLPlan;
                }
                else
                {
                    xml = AutoPilotHelpers.RunWithShowPlan(conn, e.TextData, true);
                }
                if (!string.IsNullOrEmpty(xml)) _planCacheKey = _planCache.AddPlan(xml);
            }
            catch (Exception ex)
            {
                Error = ex;
                using (_logX.InfoCall("TraceEventPlan(SqlConnection conn, TEBase e) Error:" + ex.Message))
                {
                    _logX.Info(e.TextData);
                }
            }
        }

        public TraceEventPlan(string queryPlan, TEBase e, PlanCache planCache)
        {
            Event = e;
            _planCache = planCache;
            string xml = queryPlan;
            if (!string.IsNullOrEmpty(xml)) _planCacheKey = _planCache.AddPlan(xml);
        }
    }
}
