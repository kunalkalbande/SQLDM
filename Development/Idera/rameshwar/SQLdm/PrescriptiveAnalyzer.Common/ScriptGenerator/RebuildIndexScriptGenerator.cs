using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class RebuildIndexScriptGenerator : IScriptGenerator
    {
        private readonly FragmentedIndexRecommendation _recommendation;
        private bool online = false;

        public RebuildIndexScriptGenerator(FragmentedIndexRecommendation recommendation)
        {
            _recommendation = recommendation;
        }

        public string BracketDatabase
        {
            get
            {
                if (null == _recommendation) return null;
                return _recommendation.BracketedDatabase;
            }
        }
        public string BracketSchemaTable { get { return SQLHelper.Bracket(_recommendation.Schema, _recommendation.Table); } }
        public string BracketIndexName { get { return SQLHelper.Bracket(_recommendation.Name); } }
        public int Partition { get { return _recommendation.Partition; } }
        public string Online { get { return online ? "ON" : "OFF"; } }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            try
            {
                CheckIndexForOnlineRebuild(connectionInfo);
                string template = ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.RebuildIndex.sql");
                return FormatHelper.Format(template, this);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Unable to generate rebuild index script.", e);
            }
        }

        public void CheckIndexForOnlineRebuild(SqlConnectionInfo connectionInfo)
        {
            //SQLDM-27129 - condition to check if _recommendation.RecommendationIndex is null or not
            if ((null == _recommendation.RecommendationIndex) || (null == _recommendation.RecommendationIndex.OnlineRebuild))
            {
                if (connectionInfo != null)
                {
                    string script = Idera.SQLdm.PrescriptiveAnalyzer.Common.Properties.Resources.IsIndexRebuildableOnline;

                    string database = _recommendation.Database;
                    //using (SqlConnection cnn = connectionInfo.GetConnection())
                    //{
                    //    cnn.Open();
                    //    if (!String.IsNullOrEmpty(database))
                    //        cnn.ChangeDatabase(database);

                    //    using (SqlCommand command = new SqlCommand(script, cnn))
                    //    {
                    //        command.Parameters.AddWithValue("@ObjectName", BracketSchemaTable);
                    //        command.Parameters.AddWithValue("@IndexName", _recommendation.Name);
                    //        online = Convert.ToBoolean(command.ExecuteScalar());
                    //    }
                    //}
                    online = false;
                }
            }
            else
            {
                online = (bool)_recommendation.RecommendationIndex.OnlineRebuild;
            }
        }
        public void CheckIndexForOnlineRebuild(SqlConnection connection)
        {
            if (null == _recommendation.RecommendationIndex.OnlineRebuild)
            {
                if (connection != null)
                {
                    string script = Idera.SQLdm.PrescriptiveAnalyzer.Common.Properties.Resources.IsIndexRebuildableOnline;

                    string database = _recommendation.Database;
                    //using (SqlConnection cnn = connectionInfo.GetConnection())
                    //{
                    //    cnn.Open();
                    //    if (!String.IsNullOrEmpty(database))
                    //        cnn.ChangeDatabase(database);

                    //    using (SqlCommand command = new SqlCommand(script, cnn))
                    //    {
                    //        command.Parameters.AddWithValue("@ObjectName", BracketSchemaTable);
                    //        command.Parameters.AddWithValue("@IndexName", _recommendation.Name);
                    //        online = Convert.ToBoolean(command.ExecuteScalar());
                    //    }
                    //}
                    online = false;
                }
            }
            else
            {
                online = (bool)_recommendation.RecommendationIndex.OnlineRebuild;
            }
        }
    }
}
