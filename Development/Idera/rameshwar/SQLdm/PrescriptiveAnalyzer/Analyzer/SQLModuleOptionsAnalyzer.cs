using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BBS.TracerX;
//using Idera.SQLdoctor.AnalysisEngine.Batches;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    internal class SQLModuleOptionsAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 16;
        private static Logger _logX = Logger.GetLogger("SQLModuleOptionsAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public SQLModuleOptionsAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("SQLModuleOptions analysis"); }


        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.SQLModuleOptionsMetrics == null) return;
            foreach (SQLModuleOptionsForDB metrics in sm.SQLModuleOptionsMetrics.SQLModuleOptionsForDBs)
            {
                foreach (SQLModuleOptions snap in metrics.SQLModuleOptionsList)
                {
                    SQLModuleOptionRecommendation recommendation = new SQLModuleOptionRecommendation();
                    recommendation.DatabaseName = snap.DatabaseName;
                    recommendation.SchemaName = snap.SchemaName;
                    recommendation.ObjectName = snap.ObjectName;
                    recommendation.ObjectType = snap.ObjectType;
                    if (snap.ObjectDefinition == null)
                    {
                        recommendation.ObjectDefinition = String.Empty;
                    }
                    else
                    {
                        recommendation.ObjectDefinition = snap.ObjectDefinition;
                    }
                    recommendation.UsesAnsiNulls = snap.UsesAnsiNulls;
                    recommendation.UsesQuotedIdentifier = snap.UsesQuotedIdentifier;
                    AddRecommendation(recommendation);
                }
            }
        }
    }
}