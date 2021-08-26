using System;
using System.Data;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.SQL
{
    public class OpenTransaction
    {
        internal long TID { get; private set; }
        internal int SPID { get; private set; }
        internal DateTime Start { get; private set; }
        internal TimeSpan Duration { get; private set; }
        internal string Database { get; private set; }
        internal string   Login    { get; private set; }
        internal string Host { get; private set; }
        internal string Program { get; private set; }
        internal int Stmt_Start { get; private set; }
        internal int Stmt_End { get; private set; }
        internal string Sql { get; private set; }

        internal double DurationMinutes { get { return Math.Round(Duration.TotalMinutes, 1); } }

        internal OpenTransaction(DataRow row)
        {
            SPID = DataHelper.ToInt32(row, "session_id");
            TID = DataHelper.ToLong(row, "transaction_id");
            Start = DataHelper.ToDateTime(row, "transaction_begin_time");
            Duration = DataHelper.ToDateTime(row, "transaction_duration").TimeOfDay;
            Database = DataHelper.ToString(row, "name").TrimEnd();
            Login = DataHelper.ToString(row, "loginame").TrimEnd();
            Host = DataHelper.ToString(row, "hostname").TrimEnd();
            Program = DataHelper.ToString(row, "program_name").TrimEnd();
            Stmt_Start = DataHelper.ToInt32(row, "stmt_start");
            Stmt_End = DataHelper.ToInt32(row, "stmt_end");
            Sql = DataHelper.ToString(row, "text");
        }
    }
}
