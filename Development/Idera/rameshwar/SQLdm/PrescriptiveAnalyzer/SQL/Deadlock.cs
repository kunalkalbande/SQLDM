using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.SQL
{
    internal class Deadlock
    {
        internal long EventSequence { get; set; }
        internal DateTime StartTime { get; set; }
        internal String TextData    { get; set; }

        internal Deadlock(DataRow row)
        {
            EventSequence = DataHelper.ToLong(row, "EventSequence");
            StartTime = DataHelper.ToDateTime(row, "StartTime");
            TextData = DataHelper.ToString(row, "TextData");
        }
    }
}
