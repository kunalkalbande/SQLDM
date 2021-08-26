using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Data.SqlClient;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
//using Idera.SQLdoctor.AnalysisEngine.Batches;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    class DBSecurityAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 3;
        private static Logger _logX = Logger.GetLogger("DBSecurityAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public DBSecurityAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("DBSecurity analysis"); }
        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.DBSecurityMetrics == null) return;
            foreach (DBSecurityForDB metrics in sm.DBSecurityMetrics.DBSecurityForDBs)
            {

                if (metrics.GuestHasDatabaseAccess)
                    AddRecommendation(
                        new DatabaseWithFixRecommendation(RecommendationType.GuestHasDatabaseAccess,
                                                            metrics.dbname));

                if (metrics.IsTrustworthyVulnerable)
                    AddRecommendation(
                        new DatabaseWithFixRecommendation(RecommendationType.IsTrustworthyVulnerable,
                                                            metrics.dbname));

                if (metrics.SystemDatabaseSymmetricKey)
                    AddRecommendation(
                        new DatabaseNoFixRecommendation(RecommendationType.SystemDatabaseSymmetricKey,
                                                        metrics.dbname));

                if (metrics.NonSystemDatabaseWeakKey)
                    AddRecommendation(
                        new DatabaseNoFixRecommendation(RecommendationType.NonSystemDatabaseWeakKey,
                                                        metrics.dbname));
            }
        }
                       
    }
}