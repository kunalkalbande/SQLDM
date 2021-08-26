using System.Data;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.SQL
{
    public class BlockingProcess
    {
        internal int SPID { get; private set; }
        internal string ApplicationName { get; private set; }
        internal string HostName { get; private set; }
        internal string UserName { get; private set; }
        internal string Database { get; private set; }
        internal long BlockedWait { get; private set; }
        internal int BlockedNumberOfProcesses { get; private set; }
        internal string BlockedResource { get; private set; }
        internal string BlockedTSQL { get; private set; }

        internal BlockingProcess(DataRow row)
        {
            BlockedNumberOfProcesses = DataHelper.ToInt32(row, "blocking");
            SPID = DataHelper.ToInt32(row, "spid");
            BlockedWait = DataHelper.ToLong(row, "waittime");
            ApplicationName = DataHelper.ToString(row, "program_name").TrimEnd();
            Database = DataHelper.ToString(row, "database").TrimEnd();
            HostName = DataHelper.ToString(row, "hostname").TrimEnd();
            UserName = DataHelper.ToString(row, "loginame").TrimEnd();
            BlockedTSQL = DataHelper.ToString(row, "script").TrimEnd();
            BlockedResource = DataHelper.ToString(row, "waitresource").TrimEnd();
        }
       
        internal static int GetSPID(DataRow row)
        {
            return DataHelper.ToInt32(row, "spid");
        }
        internal static long GetBlockedWait(DataRow row)
        {
            return DataHelper.ToLong(row, "waittime");
        }
    }
}
