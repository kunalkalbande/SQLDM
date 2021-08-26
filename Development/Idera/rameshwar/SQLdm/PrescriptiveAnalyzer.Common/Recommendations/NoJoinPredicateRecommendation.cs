using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class NoJoinPredicateStatement
    {
        public string statement;
        public double EstimatedTotalSubTreeCost;
        public double StatementSubTreeCost;
    }

    [Serializable]
    public class NoJoinPredicateRecommendation : TSqlRecommendation, IProvideXmlPlan
    {
        private readonly BatchStatements _batchStatements;
        [OptionalField]
        private double _estimatedTotalSubTreeCost;
        [OptionalField]
        private double _totalExecutionPlanCost;

        public BatchStatements Batch { get { return (_batchStatements); } }
        public string XmlPlan { get; private set; }

        public NoJoinPredicateRecommendation()
        {
        }

        public NoJoinPredicateRecommendation(RecommendationProperties recProp):
            base(RecommendationType.NoJoinPredicate, recProp)
        {
            _batchStatements = recProp.GetBatchStatements("_batchStatements");
            _estimatedTotalSubTreeCost = recProp.GetDouble("_estimatedTotalSubTreeCost");
            _totalExecutionPlanCost = recProp.GetDouble("_totalExecutionPlanCost");
            XmlPlan = recProp.GetString("XmlPlan");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("_batchStatements", RecommendationProperties.GetXml<BatchStatements>(_batchStatements));
            prop.Add("_estimatedTotalSubTreeCost", _estimatedTotalSubTreeCost.ToString());
            prop.Add("_totalExecutionPlanCost", _totalExecutionPlanCost.ToString());
            prop.Add("XmlPlan", XmlPlan.ToString());
            return prop;
        }

        public NoJoinPredicateRecommendation(string db, string application, String user, String host, string batch, IEnumerable<NoJoinPredicateStatement> noJoinStatements, string plan) :
            base(RecommendationType.NoJoinPredicate, db, application, user, host) 
        {
            
            //IEnumerable<string> strStatements = from statement in noJoinStatements select statement.statement;
            List<string> strStatements = new List<string>();
            foreach (NoJoinPredicateStatement stat in noJoinStatements)
            {
                strStatements.Add(stat.statement);
            }

            XmlPlan = plan;
            Sql = new OffendingSql(batch, strStatements);
            _batchStatements = new BatchStatements(batch, strStatements);

            //_estimatedTotalSubTreeCost = noJoinStatements.Max<NoJoinPredicateStatement>(new Func<NoJoinPredicateStatement, double>(x => x.EstimatedTotalSubTreeCost));
            foreach (NoJoinPredicateStatement stat in noJoinStatements)
            {
                if (stat.EstimatedTotalSubTreeCost > _estimatedTotalSubTreeCost)
                {
                    _estimatedTotalSubTreeCost = stat.EstimatedTotalSubTreeCost;
                }
            }
            //_totalExecutionPlanCost = noJoinStatements.Max<NoJoinPredicateStatement>(new Func<NoJoinPredicateStatement, double>(x => x.StatementSubTreeCost));
            foreach (NoJoinPredicateStatement stat in noJoinStatements)
            {
                if (stat.StatementSubTreeCost > _totalExecutionPlanCost)
                {
                    _totalExecutionPlanCost = stat.StatementSubTreeCost;
                }
            }
        }


        public override int AdjustConfidenceFactor(int i)
        {
            if (_estimatedTotalSubTreeCost < 0.05 * _totalExecutionPlanCost)
                return LOW_CONFIDENCE;
            else
                return base.AdjustConfidenceFactor(i);
        }

        public override int AdjustImpactFactor(int i)
        {
            if (_estimatedTotalSubTreeCost < 0.05 * _totalExecutionPlanCost)
                return LOW_IMPACT;
            else if (_estimatedTotalSubTreeCost > 0.75 * _totalExecutionPlanCost)
                return HIGH_IMPACT;
            else 
                return base.AdjustImpactFactor(i);
        }
    }
}
