IF (object_id('p_GetVirtualizationStatistics') is not null)
BEGIN
drop procedure p_GetVirtualizationStatistics
END
GO

CREATE procedure [dbo].[p_GetVirtualizationStatistics]
	@SQLServerID int,
	@UTCStart Datetime,
	@UTCEnd Datetime,
	@UTCOffset int = 0,
	@Interval int
AS
BEGIN

SELECT 
      [UTCCollectionDateTime] = dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime]))), 
      [VmName] = max(vmConfig.VMName),
      [EsxHostName] = max(esxConfig.HostName),
      [VmCPUUsage] = ROUND(AVG(vm.CPUUsage), 2), 
      [EsxCPUUsage] = ROUND(AVG(esx.CPUUsage), 2), 
      [VmCPUUsageMHz] = ROUND(AVG(vm.CPUUsageMHz), 2), 
      [EsxCPUUsageMHz] = ROUND(AVG(esx.CPUUsageMHz), 2), 
      [VmCPUReady] = ROUND(AVG(vm.CPUReady), 2),
      [VmCPUSwapWait] = ROUND(AVG(CPUSwapWait), 2), 
      [VmMemUsage] = ROUND(AVG(vm.MemUsage), 2), 
      [EsxMemUsage] = ROUND(AVG(esx.MemUsage), 2), 
      [VmMemSwapInRate] = ROUND(AVG(vm.MemSwapInRate), 2), 
      [EsxMemSwapInRate] = ROUND(AVG(esx.MemSwapInRate), 2), 
      [VmMemSwapOutRate] = ROUND(AVG(vm.MemSwapOutRate), 2), 
      [EsxMemSwapOutRate] = ROUND(AVG(esx.MemSwapOutRate), 2), 
      [VmMemGrantedMB] = ROUND(AVG(vm.MemGranted / 1024), 2), 
      [EsxMemGrantedMB] = ROUND(AVG(esx.MemGranted / 1024), 2), 
      [VmMemBaloonedMB] = ROUND(AVG(vm.MemBalooned / 1024), 2), 
      [EsxMemBaloonedMB] = ROUND(AVG(esx.MemBalooned / 1024), 2), 
      [VmMemActiveMB] = ROUND(AVG(vm.MemActive / 1024), 2), 
      [EsxMemActiveMB] = ROUND(AVG(esx.MemActive / 1024), 2), 
      [VmMemConsumedMB] = ROUND(AVG(vm.MemConsumed / 1024), 2), 
      [EsxMemConsumedMB] = ROUND(AVG(esx.MemConsumed / 1024), 2), 
      [VmDiskRead] = ROUND(AVG(vm.DiskRead), 2), 
      [EsxDiskRead] = ROUND(AVG(esx.DiskRead), 2), 
      [VmDiskWrite] = ROUND(AVG(vm.DiskWrite), 2), 
      [EsxDiskWrite] = ROUND(AVG(esx.DiskWrite), 2), 
      [VmNetReceived] = ROUND(AVG(vm.NetReceived), 2), 
      [EsxNetReceived] = ROUND(AVG(esx.NetReceived), 2), 
      [VmNetTransmitted] = ROUND(AVG(vm.NetTransmitted), 2),
      [EsxNetTransmitted] = ROUND(AVG(esx.NetTransmitted), 2),
      [VmPagePerSec] = ROUND(AVG(vm.PagePerSecVM), 2), 
      [EsxMemPagePerSec] = ROUND(AVG(esx.MemPagePerSec), 2), 
      [VmAvailableByte] = ROUND(AVG(vm.AvailableByteVm), 2), 
      [EsxAvailableMemBytes] = ROUND(AVG(esx.AvailableMemBytes), 2),
	  [ServerType] = max(vhs.ServerType)
FROM VMStatistics vm 
left join MonitoredSQLServers servers on servers.SQLServerID = vm.SQLServerID
left join VirtualHostServers vhs on servers.VHostID = vhs.VHostID
left join VMConfigData vmConfig on (vm.SQLServerID = vmConfig.SQLServerID and vm.UTCCollectionDateTime = vmConfig.UTCCollectionDateTime)
left join ESXStatistics esx on servers.SQLServerID = esx.SQLServerID and vm.UTCCollectionDateTime = esx.UTCCollectionDateTime
left join ESXConfigData esxConfig on (esx.SQLServerID = esxConfig.SQLServerID and esx.UTCCollectionDateTime = esxConfig.UTCCollectionDateTime)
WHERE servers.SQLServerID = @SQLServerID and dbo.fn_RoundDateTime(@Interval, esx.UTCCollectionDateTime) between @UTCStart and @UTCEnd
group by
      vmConfig.VMName,
      case when isnull(@Interval,5) <= 4 then datepart(yy, dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) else 1 end
      ,case when isnull(@Interval,5) <= 3 then datepart(mm,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) end
      ,case when isnull(@Interval,5) <= 2 then datepart(dd,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) end
      ,case when isnull(@Interval,5) <= 1 then datepart(hh,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) end
      ,case when isnull(@Interval,5) =  0 then datepart(mi,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) end

end