using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using System.Data.SqlClient;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class FragmentedIndexRecommendation : IndexRecommendation, IScriptGeneratorProvider, IMessageGenerator
    {
        public readonly int Partition;
        public readonly float AvgFragmentation;
        public readonly UInt64 PartitionPages;
        public readonly UInt64 TablePages;
        public readonly UInt64 TotalServerBufferPages;
        public string FragmentationPercentage { get { return (string.Format("{0:0.#}", AvgFragmentation)); } }
        public string TableSize { get { return (FormatHelper.FormatBytes(TablePages * 8192)); } }
        public string PartitionSize { get { return (FormatHelper.FormatBytes(PartitionPages * 8192)); } }

        public FragmentedIndexRecommendation(SqlConnection conn, string db, string schema, string table, string name, int partition, float frag, UInt64 partitionPages, UInt64 tablePages, UInt64 totalServerBufferPages)
            : base(RecommendationType.FragmentedIndex, db, schema, table, name)
        {
            Partition = partition;
            AvgFragmentation = frag;
            PartitionPages = partitionPages;
            TablePages = tablePages;
            TotalServerBufferPages = totalServerBufferPages;
            //RecommendationIndex.GetIndexProperties(conn);
        }

        public FragmentedIndexRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.FragmentedIndex, recProp)
        {
            Partition = recProp.GetInt("Partition");
            AvgFragmentation = recProp.GetFloat("AvgFragmentation");
            PartitionPages = recProp.GetUInt64("PartitionPages");
            TablePages = recProp.GetUInt64("TablePages");
            TotalServerBufferPages = recProp.GetUInt64("TotalServerBufferPages");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Partition", Partition.ToString());
            prop.Add("AvgFragmentation", AvgFragmentation.ToString());
            prop.Add("PartitionPages", PartitionPages.ToString());
            prop.Add("TablePages", TablePages.ToString());
            prop.Add("TotalServerBufferPages", TotalServerBufferPages.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            //----------------------------------------------------------------------------
	        // Per Brett (PR DR372):
	        //   Lastly something I didnt show is the impact (as that would be typically calculated in the program and not TSQL. I would use the following:
	        //     a. If the index size is > 15% of total allocated SQL Server buffer size and > 75% fragmented then its high.
	        //     b. If the index size is between 10% and 15% of total allocated SQL Server buffer size and > 60% fragmented then its medium.
            //     c. All other combinations are low impact.
            // 
            if (TotalServerBufferPages > 0)
            {
                if (PartitionPages <= (TotalServerBufferPages * .15))
                {
                    if (PartitionPages >= (TotalServerBufferPages * .10)) { if (AvgFragmentation > 60) return (LOW_IMPACT + 1); }
                }
                else
                {
                    if (AvgFragmentation > 75) return (HIGH_IMPACT);
                }
            }
            return (LOW_IMPACT);
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new RebuildIndexScriptGenerator(this);
        }

        public bool IsScriptRunnable { get { return true; } }

      new public List<string> GetMessages(RecommendationOptimizationStatus res, SqlConnection connection)
        {
            List<string> messages = new List<string>();
            messages.AddRange(base.GetMessages(res, connection));
            messages.Add(Properties.Resources.RecommendationScriptRunDuration);
            RebuildIndexScriptGenerator ScriptGen = (RebuildIndexScriptGenerator)GetScriptGenerator();
            ScriptGen.CheckIndexForOnlineRebuild(connection);
            if ("OFF" == ScriptGen.Online.ToUpper())
            {
                messages.Add(string.Format(Properties.Resources.RebuildIndexPerformedOffline, RecommendationIndex.IndexName));
            }
            return messages;
        }
    }
}
