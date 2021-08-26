using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class RecatalogModuleScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        private readonly SQLModuleOptionRecommendation _recommendation;

        public RecatalogModuleScriptGenerator(SQLModuleOptionRecommendation recommendation)
        {
            _recommendation = recommendation;
        }

        public string GetTSqlFix(Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration.SqlConnectionInfo connectionInfo)
        {
            string[] objectTypeParts = _recommendation.ObjectType.Split('_');
            string objectType = objectTypeParts[objectTypeParts.Length-1];
            
            string schema = _recommendation.SchemaName;
            string obj = _recommendation.ObjectName;
            string qualobject = String.Format("{0}.{1}", schema, obj);
            
            StringBuilder builder = new StringBuilder();

            builder.Append("USE ").AppendLine(SQLHelper.CreateBracketedString(_recommendation.DatabaseName));
            builder.AppendLine("GO");

            builder.AppendLine("SET ANSI_NULLS ON");
            builder.AppendLine("SET QUOTED_IDENTIFIER ON");
            builder.AppendLine("GO");

            string def = _recommendation.ObjectDefinition.Trim();
            int x = def.IndexOf("create ", StringComparison.CurrentCultureIgnoreCase);
            if (x != -1)
            {
                builder.AppendLine();
                builder.Append("ALTER ");
                builder.Append(def.Substring(x + 7));
            }
            else
            {
                builder.AppendLine("-- Recataloging will remove any permissions (grants/revokes) on this SQL Module");
                builder.AppendLine("-- Please ensure you reapply any permissions after executing this script");
                builder.AppendFormat("IF (object_id('{0}') IS NOT NULL)", qualobject).AppendLine("");                                       
                builder.AppendLine("BEGIN");
                builder.AppendFormat("    DROP {0} {1}", objectType, qualobject).AppendLine("");
                builder.AppendLine("END");
                builder.AppendLine("GO").AppendLine();
                builder.Append(_recommendation.ObjectDefinition);
            }

            return builder.ToString();
        }

        public string GetTSqlUndo(Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration.SqlConnectionInfo connectionInfo)
        {
            string[] objectTypeParts = _recommendation.ObjectType.Split('_');
            string objectType = objectTypeParts[objectTypeParts.Length-1];

            string schema = _recommendation.SchemaName;
            string obj = _recommendation.ObjectName;
            string qualobject = String.Format("{0}.{1}", schema, obj);

            StringBuilder builder = new StringBuilder();

            builder.Append("USE ").AppendLine(SQLHelper.CreateBracketedString(_recommendation.DatabaseName));
            builder.AppendLine("GO");

            builder.AppendLine((_recommendation.UsesAnsiNulls) ? "SET ANSI_NULLS ON" : "SET ANSI_NULLS OFF");
            builder.AppendLine((_recommendation.UsesQuotedIdentifier) ? "SET QUOTED_IDENTIFIER ON" : "SET QUOTED_IDENTIFIER OFF");
            builder.AppendLine("GO");
            
            string def = _recommendation.ObjectDefinition.Trim();
            int x = def.IndexOf("create ", StringComparison.CurrentCultureIgnoreCase);
            if (x != -1)
            {
                builder.AppendLine();
                builder.Append("ALTER ");
                builder.Append(def.Substring(x + 7));
            }
            else
            {
                builder.AppendLine("-- Recataloging will remove any permissions (grants/revokes) on this SQL Module");
                builder.AppendLine("-- Please ensure you reapply any permissions after executing this script");
                builder.AppendFormat("IF (object_id('{0}') IS NOT NULL)", qualobject).AppendLine("");
                builder.AppendLine("BEGIN");
                builder.AppendFormat("    DROP {0} {1}", objectType, qualobject).AppendLine("");
                builder.AppendLine("END");
                builder.AppendLine("GO").AppendLine();
                builder.Append(_recommendation.ObjectDefinition);
            }

            return builder.ToString();
        }
    }
}
