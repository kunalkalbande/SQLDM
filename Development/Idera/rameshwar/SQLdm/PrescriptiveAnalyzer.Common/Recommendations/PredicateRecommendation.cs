
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class PredicateRecommendation : TSqlRecommendation, IProvideTableName
    {
        public string Schema { get; private set; }
        public string Table  { get; private set; }
        public readonly string Column;
        public readonly string StatementText;
        public readonly string Batch;
        public readonly string PhysicalOp;
        public readonly string ObjectName;
        public readonly double PercentOfTraceDuration;

        protected PredicateRecommendation(RecommendationType rt, RecommendationProperties recProp)
            : base(rt, recProp)
        {
            Schema = recProp.GetString("Schema");
            Table = recProp.GetString("Table");
            Column = recProp.GetString("Column");
            StatementText = recProp.GetString("StatementText");
            Batch = recProp.GetString("Batch");
            PhysicalOp = recProp.GetString("PhysicalOp");
            ObjectName = recProp.GetString("ObjectName");
            PercentOfTraceDuration = recProp.GetDouble("PercentOfTraceDuration");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Schema", Schema.ToString());
            prop.Add("Table", Table.ToString());
            prop.Add("Column", Column.ToString());
            prop.Add("StatementText", StatementText.ToString());
            prop.Add("Batch", Batch.ToString());
            prop.Add("PhysicalOp", PhysicalOp.ToString());
            prop.Add("ObjectName", ObjectName.ToString());
            prop.Add("PercentOfTraceDuration", PercentOfTraceDuration.ToString());
            return prop;
        }

        protected PredicateRecommendation(RecommendationType rt, string db, string schema, string table, string column, string stmt, string batch, string op, string objectName, string applicationName, string hostName, string userName, UInt64 totalDuration, int traceDurationMinutes)
            : base(rt, db, applicationName, userName, hostName)
        {
            try
            {
                Schema = schema;
                Table = table;
                Column = column;
                StatementText = stmt;
                Batch = batch;
                PhysicalOp = op;
                ObjectName = objectName;
                double seconds = totalDuration / 1000000.0;
                PercentOfTraceDuration = (traceDurationMinutes > 0) ? ((seconds / 60) * 100.0) / traceDurationMinutes : ((seconds / 60) * 100.0);
                Sql = new OffendingSql(batch, stmt);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("PredicateRecommendation() Exception:", ex);
            }
        }
        protected void GenerateFocusSelectionsWithinPredicate(string text)
        {
            Sql.GenerateFocusSelectionsWithinPredicate(text);
        }
        public override int AdjustImpactFactor(int i)
        {
            if (PercentOfTraceDuration < 5.0) return (LOW_IMPACT);
            else if (PercentOfTraceDuration > 20) return (HIGH_IMPACT);
            return (LOW_IMPACT + 1);
        }
    }
}

