if (object_id('p_GetVMSummaryReport') is not null)
begin
drop procedure [p_GetVMSummaryReport]
end
go


create procedure [dbo].[p_GetVMSummaryReport]
				@ServerID int,
				@UTCOffset int = null,
				@StartTime DateTime = null,
				@EndTime DateTime = null
as
begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

set @EndTime = ISNULL(@EndTime, GetutcDate())
set @UTCOffset = ISNULL(@UTCOffset, datediff(mi, getutcdate(), getdate()))
set @StartTime = ISNULL(@StartTime, CONVERT(CHAR(8),getutcdate(),112))


select
	@ServerID as [ServerID]
	,DATEADD(mi, @UTCOffset, vs.UTCCollectionDateTime) as [LastCollectionInterval]
	,vs.[CPUUsage] as [VmCpuUsage]
	,es.[CPUUsage] as [EsxCpuUsage]
	,vs.[MemUsage] as [VmMemUsage]
	,es.[MemUsage] as [EsxMemUsage]
	,(ISNULL(vs.[DiskRead], 0) + ISNULL(vs.[DiskWrite], 0)) as [VmDiskUsage] 
	,(ISNULL(es.[DiskRead], 0) + ISNULL(es.[DiskWrite], 0)) as [EsxDiskUsage]
	,vs.[NetUsage] as [VmNetUsage]
	,es.[NetUsage] as [EsxNetUsage]
from
	[VMStatistics] vs
	left join [MonitoredSQLServers] ms 
		on vs.SQLServerID = ms.SQLServerID 
	left join [ESXStatistics] es
		on vs.[SQLServerID] = es.[SQLServerID] 
		and vs.[UTCCollectionDateTime] = es.[UTCCollectionDateTime] 
where
	ms.[SQLServerID] = @ServerID
	and ms.[Active] = 1
	and vs.[UTCCollectionDateTime] between @StartTime and @EndTime 
order by
	vs.[UTCCollectionDateTime] 

end

GO


