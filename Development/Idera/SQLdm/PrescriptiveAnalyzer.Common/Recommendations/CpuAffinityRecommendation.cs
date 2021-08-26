using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class CpuAffinityRecommendation : CpuRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, IUndoMessageGenerator
    {
        public int TotalCpuCount { get; private set; }
        public int AllowedToUseCpuCount { get; private set; }
        public int CpuUsagePercent { get; private set; }
        public ulong? ObservedAffinityMask { get; private set; }

        public CpuAffinityRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.CpuAffinity, recProp)
        {
            TotalCpuCount = recProp.GetInt("TotalCpuCount");
            AllowedToUseCpuCount = recProp.GetInt("AllowedToUseCpuCount");
            CpuUsagePercent = recProp.GetInt("CpuUsagePercent");
            ObservedAffinityMask = recProp.GetNullableULong("ObservedAffinityMask");
        }
        
        public CpuAffinityRecommendation(int cpuCount, int allowedCount, int cpuUsagePercent, ulong observedAffinityMask)
            : base(RecommendationType.CpuAffinity)
        {
            TotalCpuCount = cpuCount;
            AllowedToUseCpuCount = allowedCount;
            CpuUsagePercent = cpuUsagePercent;
            ObservedAffinityMask = (ulong?)observedAffinityMask;
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("TotalCpuCount", TotalCpuCount.ToString());
            prop.Add("AllowedToUseCpuCount", AllowedToUseCpuCount.ToString());
            prop.Add("CpuUsagePercent", CpuUsagePercent.ToString());
            prop.Add("ObservedAffinityMask", ObservedAffinityMask == null ? "" : ObservedAffinityMask.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            //----------------------------------------------------------------------------
            // If sql server is not allowed to use more than 40% of processors and the 
            // processors that are being used has a usage percentage greater than 60%
            // mark this as a high impact recommendation.
            // 
            if (AllowedToUseCpuCount < (TotalCpuCount * .4))
            {
                if (CpuUsagePercent > 60) return (HIGH_IMPACT);
            }
            return (base.AdjustImpactFactor(i));
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new ConfigureAffinityMaskScriptGenerator(ObservedAffinityMask);
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new ConfigureAffinityMaskScriptGenerator(ObservedAffinityMask);
        }

        public bool IsUndoScriptRunnable { get { return (null != ObservedAffinityMask); } }

        new public List<string> GetUndoMessages(RecommendationOptimizationStatus res, SqlConnection connection)
        {
            List<string> messages = new List<string>();
            messages.AddRange(base.GetUndoMessages(res, connection));

            try
            {
                //SqlConnection conn = connection.GetConnection();
                SQLHelper.CheckConnection(connection);
                string queryString = "select cast(value_in_use as varchar) from sys.configurations where configuration_id = 1550; -- I/O affinity mask";
                SqlCommand query = new SqlCommand(queryString, connection);
                string currentIOAffinityMask = query.ExecuteScalar().ToString();
                Int32 tempint = Int32.Parse(currentIOAffinityMask);
                ulong templong = (ulong)tempint;
                if (0 != (templong & ObservedAffinityMask))
                    messages.Add("This Undo Optimization script will revert changes made to your CPU Affinity settings in a previous optimization. Verify that additional changes to affinity settings have not occurred that could lead to an incompatible state when executing this Undo Optimization script.");
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(LOG, "CPUAffinityRecommedation GetUndoMessages failed: ", ex);
            }
            return messages;
        }
    }
}
