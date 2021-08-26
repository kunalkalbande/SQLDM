using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Microsoft.Data.Schema.ScriptDom.Sql;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using System.IO;
using Microsoft.Data.Schema.ScriptDom;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using System.Data;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class EstRowsMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("EstRowsMetrics");
        private UInt32 _totalStmtsWithRows = 0;
        private UInt32 _totalStmtsWithNoCountOn = 0;
        private ServerVersion _serverVersion;
        private TSqlParser _parser = null;

        public UInt32 TotalStmtsWithRows { get { return (_totalStmtsWithRows); } }
        public UInt32 TotalStmtsWithNoCountOn { get { return (_totalStmtsWithNoCountOn); } }

        public EstRowsMetrics(ServerVersion ver)
        {
            _serverVersion = ver;
        }

        public EstRowsMetrics()
        {
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("EstRowsMetrics not added : " + snapshot.Error); return; }
            if (snapshot.QueryPlanEstRowsSnapshotValue == null) { return; }
            if (snapshot.QueryPlanEstRowsSnapshotValue.QueryPlanEstRows != null && snapshot.QueryPlanEstRowsSnapshotValue.QueryPlanEstRows.Rows.Count > 0)
            {
                string sqlText;
                _totalStmtsWithRows = 0;
                _totalStmtsWithNoCountOn = 0;
                foreach (DataRow dr in snapshot.QueryPlanEstRowsSnapshotValue.QueryPlanEstRows.Rows)
                {
                    if (null == dr["Text"]) continue;
                    sqlText = dr["Text"].ToString();
                    if (0 != string.Compare(sqlText, "null", true))
                    {
                        if (DataHelper.ToDouble(dr, "EstRows") != 0.0)
                        {
                            ++_totalStmtsWithRows;
                            if (IsNoCountOn(sqlText)) ++_totalStmtsWithNoCountOn;
                        }
                    }
                }
            }
        }

        private bool IsNoCountOn(string sqlText)
        {
            if (null == _serverVersion) return (false);
            if (string.IsNullOrEmpty(sqlText)) return (false);
            if (null == _parser) _parser = TSqlParsingHelpers.GetParser(_serverVersion, false);
            StringBuilder sb = new StringBuilder(sqlText.Length);
            using (TextReader reader = new StringReader(sqlText))
            {
                IList<ParseError> errors = new List<ParseError>();
                IList<TSqlParserToken> tokens = _parser.GetTokenStream(reader, errors);
                foreach (TSqlParserToken token in tokens)
                {
                    if (TSqlTokenType.EndOfFile != token.TokenType)
                    {
                        if (KeepToken(token)) sb.Append(" " + token.Text.ToUpper());
                    }
                }
            }
            return (sb.ToString().Contains(" SET NOCOUNT ON"));
        }

        private bool KeepToken(TSqlParserToken token)
        {
            switch (token.TokenType)
            {
                case (TSqlTokenType.MultilineComment):
                case (TSqlTokenType.SingleLineComment):
                case (TSqlTokenType.AsciiStringLiteral):
                case (TSqlTokenType.UnicodeStringLiteral):
                case (TSqlTokenType.WhiteSpace):
                case (TSqlTokenType.EndOfFile):
                case (TSqlTokenType.Integer):
                case (TSqlTokenType.Real):
                case (TSqlTokenType.Double):
                case (TSqlTokenType.Money):
                    {
                        return (false);
                    }
            }
            return (true);
        }
    }
}
