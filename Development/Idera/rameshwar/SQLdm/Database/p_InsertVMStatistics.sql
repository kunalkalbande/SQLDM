if (object_id('p_InsertVMStatistics') is not null)
begin
drop procedure p_InsertVMStatistics
end
go
create procedure p_InsertVMStatistics 
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@CPUUsage float,
	@CPUUsageMHz int,
	@CPUReady bigint,
	@CPUSwapWait bigint,
	@MemSwapInRate bigint,
	@MemSwapOutRate bigint,
	@MemSwapped bigint,
	@MemActive bigint,
	@MemConsumed bigint,
	@MemGranted bigint,
	@MemBallooned bigint,
	@MemUsage float,
	@DiskRead bigint,
	@DiskWrite bigint,
	@DiskUsage bigint,
	@NetUsage bigint,
	@NetReceived bigint,
	@NetTransmitted bigint,
	@PagePerSecVM bigint,
	@AvailableByteVm bigint,
	@ReturnMessage nvarchar(128) output
as
begin

	insert into [VMStatistics]  (
		[SQLServerID],
		[UTCCollectionDateTime],
		[CPUUsage] ,
		[CPUUsageMHz] ,
		[CPUReady] ,
		[CPUSwapWait] ,
		[MemSwapInRate] ,
		[MemSwapOutRate] ,
		[MemSwapped] ,
		[MemActive] ,
		[MemConsumed] ,
		[MemGranted] ,
		[MemBalooned] ,
		[MemUsage] ,
		[DiskRead] ,
		[DiskWrite] ,
		[DiskUsage] ,
		[NetUsage] ,
		[NetReceived] ,
		[NetTransmitted],
		[PagePerSecVM],
		[AvailableByteVm]	)
	values (
		@SQLServerID,
		@UTCCollectionDateTime ,
		@CPUUsage ,
		@CPUUsageMHz ,
		@CPUReady ,
		@CPUSwapWait ,
		@MemSwapInRate ,
		@MemSwapOutRate ,
		@MemSwapped , 
		@MemActive ,
		@MemConsumed , 
		@MemGranted ,
		@MemBallooned ,
		@MemUsage ,
		@DiskRead ,
		@DiskWrite ,
		@DiskUsage ,
		@NetUsage ,
		@NetReceived ,
		@NetTransmitted,
		@PagePerSecVM,
		@AvailableByteVm	)

end