using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
//using Idera.SQLdoctor.Common.Ranking;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Objects;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Ranking;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class IndexRecommendation : Recommendation, IProvideTableName
    {
        public string Database { get; private set; }
        public readonly string _schema;
        public readonly string _table;
        public readonly string Name;
        public Index RecommendationIndex;

        public string Schema { get { return _schema; } }
        public string Table  { get { return _table; } }

        protected IndexRecommendation(RecommendationType rt, RecommendationProperties recProp)
            : base(rt, recProp)
        {
            Database = recProp.GetString("Database");
            _schema = recProp.GetString("_schema");
            _table = recProp.GetString("_table");
            Name = recProp.GetString("Name");
            RecommendationIndex = recProp.GetIndex("RecommendationIndex");
            //RecommendationIndex = new Index(Database, _schema, _table, Name);
        }
        protected IndexRecommendation(RecommendationType rt, string db, string schema, string table, string name) : base(rt)
        {
            Database = db;
            _schema = schema;
            _table = table;
            Name = name;
            RecommendationIndex = new Index(db, schema, table, name);
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Database", Database.ToString());
            prop.Add("_schema", _schema.ToString());
            prop.Add("_table", _table.ToString());
            prop.Add("Name", Name.ToString());
            try
            {
                prop.Add("RecommendationIndex", RecommendationProperties.GetXml<Idera.SQLdm.PrescriptiveAnalyzer.Common.Objects.Index>(RecommendationIndex));
            }
            catch { }
            return prop;
        }

        protected override double GetRelevance(double baseRelevance, RankingStats rankingStats)
        {
            if (this.RecommendationType != RecommendationType.HypotheticalIndex)
            {
                double p = rankingStats.GetPercentile(this);
                if (p > 0.0)
                {
                    if (p > 1.0) LOG.DebugFormat("RankStats Percentile: {0} for {1}", p, this.ToString());
                    return (p + baseRelevance);
                }
            }
            return (baseRelevance);
        }

        public override string ToString()
        {
            return (string.Format("[{0}].[{1}].[{2}].[{3}]", Database, _schema, _table, Name));
        }

        public string TableName
        {
            get { return String.IsNullOrEmpty(_schema) ? _table : _schema + "." + _table ; }
        }

        public string SafeSchemaTable
        {
            get { return SQLHelper.CreateSafeString(BracketedTableName); }
        }

        public string SafeName
        {
            get { return SQLHelper.CreateSafeString(Name); }
        }

        public string IndexName
        {
            get { return Name; }
        }

        public string BracketedDatabase
        {
            get { return String.IsNullOrEmpty(Database) ? string.Empty : SQLHelper.Bracket(Database); }
        }

        public string BracketedTableName
        {
            get { return String.IsNullOrEmpty(_schema) ? SQLHelper.Bracket(_table) : SQLHelper.Bracket(_schema) + "." + SQLHelper.Bracket(_table); }
        }

        public string BracketedName
        {
            get { return SQLHelper.Bracket(Name); }
        }
    }
}
