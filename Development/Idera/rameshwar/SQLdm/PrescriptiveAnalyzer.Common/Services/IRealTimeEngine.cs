using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Values;
//using Idera.SQLdoctor.Common.Configuration;
//using Idera.SQLdoctor.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Values;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Services
{
    public enum RealTimeRequest
    {
        None = 0,
        Server,
        Sessions,
        Waits,
        DMVQueries,
        ServerHealth,
        JobsChecklist
    }
    public enum RealTimeServerData
    {
        None = 0,
        Processes,
        NetworkInterface,
        PerfProcProcess,
        Processors,
        PerfOSProcessor,
        PerfDiskPhysicalDisk,
        PerfDiskLogicalDisk,
        ComputerSystem,
        CacheMemory,
        DiskPartitions,
        PerfOSMemory,
        PhysicalMemory,
        Bios,
    }
    
    public interface IRealTimeEngine
    {
        IList<IRecommendation> GetRecommendations(RealTimeRequest req, object o);
        SnapshotMetrics GetHealthMetrics();
        object GetServerData(RealTimeServerData rtsd);
        Dictionary<string, int> GetPerformanceCheckListData(out Exception ex);
        DisasterRecoveryCheckListData GetDisasterRecoveryCheckListData(out Exception ex);
        List<string> GetJobsChecklist(out Exception ex);
        List<string> GetRecentFailedJobs(int threshold, out Exception ex);
        string GetServerInfoString();
        List<string> GetDiskSpaceChecklist(int threshold, out Exception ex);
    }

    public interface IProvideRealTimeEngine
    {
        IRealTimeEngine GetRealTimeEngine();
    }

    [Serializable]
    public class DisasterRecoveryCheckListData
    {
        public Dictionary<string, DateTime?> FullBackups = new Dictionary<string,DateTime?>();
        public Dictionary<string, DateTime?> LogBackups = new Dictionary<string,DateTime?>();
        public List<string> SuspectPages = new List<string>();
    }
}
