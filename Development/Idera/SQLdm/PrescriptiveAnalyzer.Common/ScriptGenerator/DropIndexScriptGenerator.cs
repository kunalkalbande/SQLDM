using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using System.Data;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class DropIndexScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        private readonly IndexRecommendation _recommendation;
        private string _indexName;
        private bool _returnDatabase;

        public DropIndexScriptGenerator(IndexRecommendation recommendation) : this(recommendation, recommendation.Name)
        {
        }

        public DropIndexScriptGenerator(IndexRecommendation recommendation, string indexName)
        {
            _indexName = indexName;
            _recommendation = recommendation;
            _returnDatabase = true;
        }

        internal bool ReturnDatabase
        {
            get { return _returnDatabase; }
            set { _returnDatabase = value; }
        }

        public string BracketDatabase
        {
            get { return _returnDatabase ? SQLHelper.CreateBracketedString(_recommendation.Database) : null; }
        }

        public string BracketSchemaTable
        {
            get { return SQLHelper.Bracket(_recommendation.Schema, _recommendation.Table); }
        }

        public string SafeSchemaTable
        {
            get { return SQLHelper.CreateSafeString(BracketSchemaTable); }
        }

        public string SafeIndexName
        {
            get { return SQLHelper.CreateSafeString(IndexName); }
        }

        public string BracketIndexName
        {
            get { return SQLHelper.Bracket(IndexName); }
        }
        public string IndexName
        {
            get { return _indexName; }
            internal set { _indexName = value; }
        }


        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            byte indexType;
            bool isPrimaryKey;
            bool isUniqueIndex;
            if (GetIndexMetaData(connectionInfo, out indexType, out isPrimaryKey, out isUniqueIndex))
            {
                string resource = (isPrimaryKey || isUniqueIndex)
                    ? "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.DropUniqueConstraint.sql"
                    : "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.DropIndex.sql";
                string template = ApplicationHelper.GetEmbededResource(GetType().Assembly, resource);
                return FormatHelper.Format(template, this);
            }

            return String.Empty;
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            StringBuilder script = new StringBuilder();
            script.Append(_recommendation.RecommendationIndex.GetIndexCreateScript());

            if (RecommendationType.DisabledIndex == _recommendation.RecommendationType)
            {
                script.AppendLine();
                script.AppendLine(_recommendation.RecommendationIndex.GetIndexDisableScript());
            }

            return script.ToString();
        }

        public bool GetIndexMetaData(SqlConnectionInfo connectionInfo, out byte index_type, out bool is_primary_key, out bool is_unique_constraint)
        {
            bool result = false;
            string database = _recommendation.Database;
            try
            {
                using (SqlConnection cnn = connectionInfo.GetConnection())
                {
                    cnn.Open();
                    if (!String.IsNullOrEmpty(database))
                        cnn.ChangeDatabase(database);

                    //using (SqlCommand command = new SqlCommand("select [type],is_primary_key,is_unique_constraint from sys.indexes where name = @Index", cnn))
                    //{
                    //    command.Parameters.AddWithValue("@Index", _indexName);

                    //    using (SqlDataReader reader = command.ExecuteReader())
                    //    {
                    //        if (reader.Read())
                    //        {
                    //            index_type = SQLHelper.GetByte(reader, 0);
                    //            is_primary_key = SQLHelper.GetBoolean(reader, 1);
                    //            is_unique_constraint = SQLHelper.GetBoolean(reader, 2);
                    //            result = true;
                    //        }
                    //        else
                    //            throw new RowNotInTableException(String.Format("Index '{0}' not found.  This recommendation may no longer be an issue.", _indexName));
                    //    }
                    //}
                    index_type = 0;
                    is_primary_key = false;
                    is_unique_constraint = false;
                    result = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log("DropIndexScriptGenerator.GetIndexMetaData() Exception:", ex);
                index_type = 0;
                is_primary_key = false;
                is_unique_constraint = false;
                throw new ApplicationException("Unable to generate drop index script.", ex);
            }
            return result;
        }
    }
}
