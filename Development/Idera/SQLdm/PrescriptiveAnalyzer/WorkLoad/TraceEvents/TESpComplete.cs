using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.Schema.ScriptDom.Sql;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents
{
    public class TESpComplete : TEBase
    {
        public TESpComplete(DataRow dr)
            : base(dr) 
        {
        }
        public TESpComplete(SqlDataReader r)
            : base(r)
        {
        }
        internal override bool KeepToken(TSqlParserToken token)
        {
            return (TSqlTokenType.Identifier == token.TokenType);
        }
    }
}
