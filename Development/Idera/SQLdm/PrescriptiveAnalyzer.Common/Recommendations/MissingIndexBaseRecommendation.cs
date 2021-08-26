using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using System.Data.SqlClient;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class MissingIndexBaseRecommendation : Recommendation, IScriptGeneratorProvider, IProvideTableName, IUndoScriptGeneratorProvider, IMessageGenerator
    {
        private readonly List<RecommendedIndex> _recommendedIndexes = new List<RecommendedIndex>();
        private double _tableUpdatesPerSec = 0;

        public ICollection<RecommendedIndex> RecommendedIndexes { get { return (_recommendedIndexes); } }
        public string Database { get; protected set; }
        public string Schema { get; protected set; }
        public string Table { get; protected set; }
        public double TableUpdatesPerMinute { get; protected set; }

        public ICollection<string> KeyColumns
        {
            get
            {
                List<string> r = new List<string>();
                var ri = GetFirstRecommendedIndex();
                if (null != ri)
                {
                    if (null != ri.EqualColumns)
                        foreach (string i in ri.EqualColumns) r.Add(i);
                    if (null != ri.NotEqualColumns)
                        foreach (string i in ri.NotEqualColumns) r.Add(i);
                }
                return (r);
            }
        }

        public ICollection<string> IncludeColumns
        {
            get
            {
                List<string> r = new List<string>();
                var ri = GetFirstRecommendedIndex();
                if (null != ri)
                    if (null != ri.IncludeColumns)
                        foreach (string i in ri.IncludeColumns) r.Add(i);
                return (r);
            }
        }

        public IEnumerable<string> DropIndexes
        {
            get
            {
                List<string> r = new List<string>();
                var ri = GetFirstRecommendedIndex();
                if (null != ri)
                    if (null != ri.RedundantIndexes)
                        foreach (RedundantIndex i in ri.RedundantIndexes) if (null != i) r.Add(i.Name);
                return (r);
            }
        }

        public string IndexSizeFormatted { get { return (FormatHelper.FormatBytes((UInt64)IndexSize)); } }
        public long IndexSize
        {
            get
            {
                var ri = GetFirstRecommendedIndex();
                if (null != ri) return (ri.EstSize);
                return (0);
            }
        }

        public string DropIndexesSizeFormatted { get { return (FormatHelper.FormatBytes((UInt64)DropIndexesSize)); } }
        public long DropIndexesSize
        {
            get
            {
                long s = 0;
                var ri = GetFirstRecommendedIndex();
                if (null != ri)
                    if (null != ri.RedundantIndexes)
                        foreach (RedundantIndex i in ri.RedundantIndexes) if (null != i) s += i.Size;
                return (s);
            }
        }

        public MissingIndexBaseRecommendation(RecommendationType rt, IEnumerable<RecommendedIndex> recommendedIndexes, double tableUpdatesPerSec, double tableUpdatesPerMinute)
            : base(rt)
        {
            if (null != recommendedIndexes) _recommendedIndexes.AddRange(recommendedIndexes);
            _tableUpdatesPerSec = Math.Round(tableUpdatesPerSec, 2);
            TableUpdatesPerMinute = Math.Round(tableUpdatesPerMinute, 2);
        }

        public MissingIndexBaseRecommendation(RecommendationType rt, string db, string schema, string table, IEnumerable<RecommendedIndex> recommendedIndexes, double tableUpdatesPerSec, double tableUpdatesPerMinute)
            : this(rt, recommendedIndexes, tableUpdatesPerSec, tableUpdatesPerMinute)
        {
            Database = db;
            Schema = schema;
            Table = table;
        }

        public MissingIndexBaseRecommendation(RecommendationType rt, RecommendationProperties recProp)
            :base(rt,recProp)
        {
        }

        public override int AdjustConfidenceFactor(int i)
        {
            //----------------------------------------------------------------------------
            // Per Brett:
            //     Confidence = 100% - 2% for every 0.1 insert/update/delete per sec found 
            //                  on the underlying table. Don't show anything with a confidence < 10% 
            // 
            if (_tableUpdatesPerSec < 0.1) return (HIGH_CONFIDENCE);
            double confidence = ((_tableUpdatesPerSec / 0.1) * 2) / 10;
            if (confidence > 9.0) return (LOW_CONFIDENCE);
            try
            {
                return (Convert.ToInt32(HIGH_CONFIDENCE - confidence));
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(string.Format("MissingIndexBaseRecommendation.AdjustConfidenceFactor({0})", HIGH_CONFIDENCE - confidence), ex);
            }
            return (LOW_CONFIDENCE);
        }

        public IScriptGenerator GetScriptGenerator()
        {
            if (HasRecommendedIndexes)
                return new CreateIndexScriptGenerator(Database, Schema, Table, _recommendedIndexes);

            return null;
        }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            if (HasRecommendedIndexes)
                return new CreateIndexScriptGenerator(Database, Schema, Table, _recommendedIndexes);

            return null;
        }


        public bool HasRecommendedIndexes { get { return ((null != _recommendedIndexes) ? (_recommendedIndexes.Count > 0) : false); } }
        public bool IsScriptRunnable { get { return (HasRecommendedIndexes); } }
        public bool IsUndoScriptRunnable { get { return (HasRecommendedIndexes); } }

        private RecommendedIndex GetFirstRecommendedIndex()
        {
            if (null == _recommendedIndexes) return (null);
            if (_recommendedIndexes.Count <= 0) return (null);
            //return (_recommendedIndexes.First());
            return (_recommendedIndexes[0]);
        }

        new public List<string> GetMessages(RecommendationOptimizationStatus res, SqlConnection connection)
        {
            List<string> messages = new List<string>();
            messages.AddRange(base.GetMessages(res, connection));
            messages.Add(Properties.Resources.RecommendationScriptRunDuration);
            return messages;
        }
    }
}
