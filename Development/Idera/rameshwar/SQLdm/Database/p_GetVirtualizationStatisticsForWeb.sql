/* Sample execution script
EXEC p_GetVirtualizationStatisticsForWeb @SQLServerID=3,@UTCStart='2017-01-15 6:00:00.000',@UTCEnd='2017-01-17 6:00:00.000'
Procedure to get virtualization graph details for web UI overview graph
*/
IF (object_id('p_GetVirtualizationStatisticsForWeb') is not null)
BEGIN
drop procedure p_GetVirtualizationStatisticsForWeb
END
GO

CREATE procedure [dbo].[p_GetVirtualizationStatisticsForWeb]
	@SQLServerID int,
	@UTCStart Datetime,
	@UTCEnd Datetime
AS
BEGIN

SELECT 
	 vms.UTCCollectionDateTime as UTCCollectionDateTime
	,vms.DiskRead as [vmDiskRead]
	,vms.DiskWrite as [vmDiskWrite]
	,esxStat.DiskRead as ESXDiskRead
	,esxStat.DiskWrite as ESXDiskWrite
	,vms.AvailableByteVm as VmAvailableByte
	,esxStat.AvailableMemBytes as [EsxAvailableMemBytes]
	,vms.MemGranted / 1024 as VmMemGrantedMB
	,vms.MemBalooned / 1024 as VmMemBaloonedMB
	,vms.MemActive / 1024 as VmMemActiveMB
	,vms.MemConsumed / 1024 as VmMemConsumedMB
	,esxStat.MemGranted / 1024 as ESXMemGrantedMB
	,esxStat.MemBalooned / 1024 as ESXMemBaloonedMB
	,esxStat.MemActive / 1024 as ESXMemActiveMB
	,esxStat.MemConsumed / 1024 as ESXMemConsumedMB
	,[ServerType]
	FROM [VMStatistics] vms
	left join MonitoredSQLServers servers on servers.SQLServerID = vms.SQLServerID
	left join VirtualHostServers vhs on servers.VHostID = vhs.VHostID
	left join [ESXStatistics] esxStat on servers.SQLServerID = esxStat.SQLServerID and vms.UTCCollectionDateTime = esxStat.UTCCollectionDateTime
	WHERE vms.SQLServerID = @SQLServerID AND vms.UTCCollectionDateTime between @UTCStart and @UTCEnd

end
