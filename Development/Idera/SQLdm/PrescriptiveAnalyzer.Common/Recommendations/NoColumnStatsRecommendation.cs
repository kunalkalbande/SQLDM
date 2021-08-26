using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class NoColumnStatsRecommendation : Recommendation, IScriptGeneratorProvider, IProvideTableName 
    {
        private readonly List<BatchStatements> _batches = new List<BatchStatements>();

        public string Database { get; private set; }
        public string Schema { get; private set; }
        public string Table { get; private set; }
        public string Column { get; private set; }
        public IEnumerable<BatchStatements> Statements { get { return (_batches); } }
        public UInt64 AvgCountPerMinute { get; private set; }

        public NoColumnStatsRecommendation() : base(RecommendationType.NoColumnStats) {}

        public NoColumnStatsRecommendation(RecommendationProperties recProp) : base(RecommendationType.NoColumnStats,recProp) 
        {
            Database = recProp.GetString("Database");
            Schema = recProp.GetString("Schema");
            Table = recProp.GetString("Table");
            Column = recProp.GetString("Column");
            _batches = recProp.GetBatchStatementsList("_batches");
            AvgCountPerMinute = recProp.GetUInt64("AvgCountPerMinute");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Database", Database.ToString());
            prop.Add("Schema", Schema.ToString());
            prop.Add("Table", Table.ToString());
            prop.Add("Column", Column.ToString());
            prop.Add("_batches", RecommendationProperties.GetXml<List<BatchStatements>>(_batches));
            //prop.Add("_batches", Column.ToString());
            return prop;
        }

        public NoColumnStatsRecommendation(string database, string schema, string table, string column, Dictionary<string, List<string>> batches, UInt64 count, int traceDurationMinutes)
            : this()
        {
            try
            {
                Database = database;
                Schema = schema;
                Table = table;
                Column = column;
                AvgCountPerMinute = (traceDurationMinutes > 0) ? (count / Convert.ToUInt64(traceDurationMinutes)) : count;
                if (null == batches) return;
                foreach (KeyValuePair<string, List<string>> batch in batches)
                {
                    _batches.Add(new BatchStatements(batch.Key, batch.Value));
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("NoColumnStatsRecommendation() Exception:", ex);
            }
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new ColumnStatisticsScriptGenerator(Database, Schema, Table, Column);
        }

        public bool IsScriptRunnable { get { return (true); } }

        public override int AdjustImpactFactor(int i)
        {
            //---------------------------------------------------------------------------
            // Per Brett:
            //   Low - occurred less than 5 times per minute
            //   Med - occurred between 5 and 50 times per minute
            //   Hi  - occurred over 50 times per minute
            if (AvgCountPerMinute < 5) return (LOW_IMPACT);
            else if (AvgCountPerMinute <= 50) return (LOW_IMPACT + 1);
            else return (HIGH_IMPACT);
        }
    }
}
