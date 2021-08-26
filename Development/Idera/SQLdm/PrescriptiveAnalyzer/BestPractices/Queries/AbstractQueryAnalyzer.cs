using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.MetaData;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Resources;

namespace Idera.SQLdm.PrescriptiveAnalyzer.BestPractices.Queries
{
    internal class AbstractQueryAnalyzer : IAnalyzeQueries
    {
        private static TracerX.Logger _logX = TracerX.Logger.GetLogger("AbstractQueryAnalyzer");
        private string database;
        private string _className = null;
        private ServerVersion serverVersion;
        private List<IRecommendation> recommendations;
        private RecommendationCountHelper _counter = new RecommendationCountHelper(Properties.Settings.Default.Max_RecommendationsPerType);
        protected ColumnResolver columnResolver;
        protected Int32 _id;

        public AbstractQueryAnalyzer(string database)
        {
            this.database = database;
            recommendations = new List<IRecommendation>();
        }

        public Int32 ID { get { return _id; } }

        public ServerVersion ServerVersion
        {
            get { return serverVersion;  }
            set { serverVersion = value; }
        }

       public string Application { get; set; }
       public string Database
       {
           get { return database; }
           set { database = value; }
       }

       public string Host { get; set; }
       public string User { get; set; }

       public string ClassName
       {
           get
           {
               if (null != _className) return (_className);
               string name = ToString();
               try { name = this.GetType().Name; }
               catch { }
               return (_className = name);
           }
       }

       public void Analyze(string script, TSqlScript scriptParseTree)
        {
            foreach (TSqlBatch batch in scriptParseTree.Batches)
            {
                Analyze(script, batch);
            }
        }

        public virtual void Analyze(string script, TSqlBatch batchParseTree)
        {
            foreach (TSqlStatement stmt in batchParseTree.Statements)
            {
                Analyze(script, stmt);
            }
        }

        public virtual void Analyze(string script, TSqlStatement stmtParseTree)
        {
            if (stmtParseTree is UseStatement)
            {
                UpdateDatabase((UseStatement)stmtParseTree);
            }
        }

        protected void UpdateDatabase(UseStatement use)
        {
            if (use.DatabaseName != null)
            {
                database = use.DatabaseName.Value;
            }
        }

        public virtual void Clear()
        {
            recommendations = new List<IRecommendation>();
        }

        public virtual IRecommendation[] GetRecommendations()
        {
            return recommendations.ToArray();
        }

        protected void AddRecommendation(IRecommendation r)
        {
            if (null == r) return;
            //Check if Recomm exists in master recomm
            if (!MasterRecommendations.ContainsRecommendation(r.ID))
                return;
            if (!_counter.Allow(r.RecommendationType))
            {
                _logX.DebugFormat("Recommendation limit exceeded :{0} - {1}", r.ID, r.FindingText); 
                return;
            }
            //----------------------------------------------------------------------------
            // If we already have too many recommendations, ignore the new recommendations
            // being added.  This is added as a fail safe limiting mechanism and is not intended
            // to select the best recommendations.
            // 
            try
            {
                if (Settings.Default.Max_RecommendationsPerAnalyzer > 0)
                {
                    if (recommendations.Count >= Settings.Default.Max_RecommendationsPerAnalyzer)
                    {
                        using (_logX.InfoCall(string.Format("{0}.AddRecommendation() limit of {1} encountered", ClassName, Settings.Default.Max_RecommendationsPerAnalyzer)))
                        {
                            _logX.InfoFormat("Recommendation being thrown away due to limit of {0} recommendations", Settings.Default.Max_RecommendationsPerAnalyzer);
                            _logX.InfoFormat("Recommendation:{0} - {1}", r.ID, r.FindingText);
                        }
                        return;
                    }
                }
            }
            catch (Exception)
            { /* */ }

            _counter.Add(r);
            recommendations.Add(r);
        }
    }
}
