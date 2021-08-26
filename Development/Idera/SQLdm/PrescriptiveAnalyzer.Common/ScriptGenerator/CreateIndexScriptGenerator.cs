using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class CreateIndexScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        private ReadOnlyCollection<RecommendedIndex> _recommendedIndexes;
        private string _database;
        public string Database
        {
            get
            {
                if (String.IsNullOrEmpty(_database))
                    return null;
                return SQLHelper.CreateBracketedString(_database);
            }
            private set { _database = value; }
        }
        public string Schema { get; private set; }
        public string Table { get; private set; }
        public CreateIndexScriptGenerator(string database, string schema, string table, List<RecommendedIndex> recommendedIndexes)
        {
            Database = database;
            Schema = schema;
            Table = table;
            _recommendedIndexes = new ReadOnlyCollection<RecommendedIndex>(recommendedIndexes);
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            StringBuilder drop = new StringBuilder();
            string template = ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdoctor.Common.ScriptGenerator.Templates.CreateIndex.sql");
            foreach (var i in _recommendedIndexes)
            {
                create.AppendLine(FormatHelper.Format(template, i));
                AppendDropRedundant(connectionInfo, drop, i);
            }
            if (drop.Length > 0) 
            {
                create.AppendLine();
                create.Append(drop); 
            }
            return (create.ToString());
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            StringBuilder undoCreate = new StringBuilder();
            StringBuilder undoDrop = new StringBuilder();
            string template = ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdoctor.Common.ScriptGenerator.Templates.DropIndex.sql");
            foreach (var i in _recommendedIndexes)
            {
                undoCreate.AppendLine(FormatHelper.Format(template, i));
                AppendUndoDropRedundant(connectionInfo, undoDrop, i);
            }
            if (undoDrop.Length > 0)
            {
                undoCreate.AppendLine();
                undoCreate.Append(undoDrop);
            }
            return (undoCreate.ToString());
        }

        private void AppendDropRedundant(SqlConnectionInfo connectionInfo, StringBuilder drop, RecommendedIndex i)
        {
            if (null == i) return;
            if (null == i.RedundantIndexes) return;
            if (i.RedundantIndexes.Count <= 0) return;            
            var duplicate = new DuplicateIndexRecommendation(connectionInfo.GetConnection(), Database, Schema, Table, 0.0, string.Empty, 0, 0, i.IndexName, 0, 0);
            var dropGen = new DropIndexScriptGenerator(duplicate);
            dropGen.ReturnDatabase = false;
            foreach (var r in i.RedundantIndexes)
            {
                if (null == r) continue;
                if (string.IsNullOrEmpty(r.Name)) continue;
                dropGen.IndexName = r.Name;

                try
                {
                    string dropScript = dropGen.GetTSqlFix(connectionInfo);
                    if (!String.IsNullOrEmpty(dropScript))
                        drop.AppendLine(dropScript);
                }
                catch (Exception)
                {
                    /* */
                }
            }
        }

        private void AppendUndoDropRedundant(SqlConnectionInfo connectionInfo, StringBuilder drop, RecommendedIndex i)
        {
            if (null == i) return;
            if (null == i.RedundantIndexes) return;
            if (i.RedundantIndexes.Count <= 0) return;
            var duplicate = new DuplicateIndexRecommendation(connectionInfo.GetConnection(), Database, Schema, Table, 0.0, string.Empty, 0, 0, i.IndexName, 0, 0);
            var undoDropGen = new DropIndexScriptGenerator(duplicate);
            undoDropGen.ReturnDatabase = false;
            foreach (var r in i.RedundantIndexes)
            {
                if (null == r) continue;
                if (string.IsNullOrEmpty(r.Name)) continue;
                undoDropGen.IndexName = r.Name;

                try
                {
                    string undoDropScript = undoDropGen.GetTSqlUndo(connectionInfo);
                    if (!String.IsNullOrEmpty(undoDropScript))
                        drop.AppendLine(undoDropScript);
                }
                catch (Exception)
                {
                    /* */
                }
            }
        }
    }
}
