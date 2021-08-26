//SQLdm 10.0 (Tarun Sapra)- Enum for baseline name and metric id, this helps in getting the baseline name as per the metric id

namespace Idera.SQLdm.Common.Baseline
{
    public enum BaselineNameAsPerMetricID
    {
        ProcedureCacheSizeBaseline = -1003,//in kb -> convert to mb
        HostDiskUsageBaseline = -127,
        HostDiskWriteBaseline = -126,
        HostDiskReadBaseline = -125,
        VMDiskUsageBaseline = -112,
        VMDiskWriteBaseline = -111,
        VMDiskReadBaseline = -110,
        SQLServerMemoryUsedBaseline = -71,//in kb -> convert to mb
        SQLMemoryAllocatedBaseline = -70,//in kb -> convert to mb
        SQLServerMemoryUsageBaseline = 13,
        OSPagingBaseline = 25,
        OSProcessorTimeBaseline = 26,
        OSPrivilegeTimeBaseline = 27,
        OSUserTimeBaseline = 28,
        OSProcessorQueueLengthBaseline = 29,
        OSDiskTimeBaseline = 30,
        PageLifeExpectancyBaseline = 76,
        ProcedureCacheHitRatioBaseline = 81,
        //remaining metrics
        VmMemoryUsageBaseline = -109,
        HostMemoryUsageBaseline = -124,
        SqlServerCPUUsageBaseline = 0,
        VmCPUUsageBaseline = 98,
        HostCPUUsageBaseline = -116,
        OSAverageDiskQueueLengthBaseline = 31//SQLdm 10.0 (Tarun Sapra)- DE45623: 'OS Average Disk Queue Length' baseline graph value is not matching
    }
    /// <summary>
    /// enum tells about shift caused to date due to UTC conversion
    /// </summary>
    public enum BaselineDayShift
    {
        Preshift = -1,
        NoShift = 0,
        PostShift = 1,
    }
}
