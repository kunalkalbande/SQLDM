if (object_id('p_InsertESXStatistics') is not null)
begin
drop procedure p_InsertESXStatistics
end
go
create procedure p_InsertESXStatistics 
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@ESXCPUUsage float,
	@ESXCPUUsageMHz int,
	@ESXMemSwapInRate bigint,
	@ESXMemSwapOutRate bigint,
	@ESXMemActive bigint,
	@ESXMemConsumed bigint,
	@ESXMemGranted bigint,
	@ESXMemBallooned bigint,
	@ESXMemUsage float,
	@ESXDiskRead bigint,
	@ESXDiskWrite bigint,
	@ESXDiskDeviceLatency bigint,
	@ESXKernelLatency bigint,
	@ESXQueueLatency bigint,
	@ESXTotalLatency bigint,
	@ESXDiskUsage bigint,
	@ESXNetUsage bigint,
	@ESXNetReceived bigint,
	@ESXNetTransmitted bigint,
	@ESXMemPagePerSec bigint,
	@ESXAvailableMemBytes bigint,
	@ReturnMessage nvarchar(128) output
as
begin

	insert into [ESXStatistics]  (
		[SQLServerID],
		[UTCCollectionDateTime],
		[CPUUsage] ,
		[CPUUsageMHz] ,
		[MemSwapInRate] ,
		[MemSwapOutRate] ,
		[MemActive] ,
		[MemConsumed] ,
		[MemGranted] ,
		[MemBalooned] ,
		[MemUsage] ,
		[DiskRead] ,
		[DiskWrite] ,
		[DiskDeviceLatency] ,
		[DiskKernelLatency] ,
		[DiskQueueLatency] ,
		[DiskTotalLatency] ,
		[DiskUsage] ,
		[NetUsage] ,
		[NetReceived] ,
		[NetTransmitted],
		[MemPagePerSec],
		[AvailableMemBytes]
		)
	values (
		@SQLServerID,
		@UTCCollectionDateTime ,
		@ESXCPUUsage ,
		@ESXCPUUsageMHz ,
		@ESXMemSwapInRate ,
		@ESXMemSwapOutRate ,
		@ESXMemActive ,
		@ESXMemConsumed , 
		@ESXMemGranted ,
		@ESXMemBallooned ,
		@ESXMemUsage ,
		@ESXDiskRead ,
		@ESXDiskWrite ,
		@ESXDiskDeviceLatency ,
		@ESXKernelLatency ,
		@ESXQueueLatency ,
		@ESXTotalLatency ,
		@ESXDiskUsage ,
		@ESXNetUsage ,
		@ESXNetReceived ,
		@ESXNetTransmitted,
		@ESXMemPagePerSec,
		@ESXAvailableMemBytes	)

end