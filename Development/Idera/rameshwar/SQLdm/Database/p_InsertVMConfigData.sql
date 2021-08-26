if (object_id('p_InsertVMConfigData') is not null)
begin
drop procedure p_InsertVMConfigData
end
go
create procedure p_InsertVMConfigData
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@UUID nvarchar(128),
	@Name nvarchar(256),
	@HeartBeat int,
	@DomainName nvarchar(256),
	@BootTime datetime,
	@NumCPUs int,
	@CPULimit bigint,
	@CPUReserve bigint,
	@MemSize bigint,
	@MemLimit bigint,
	@MemReserve bigint,
	@ReturnMessage nvarchar(128) output
as
begin

	insert into [VMConfigData] (
		[SQLServerID],
		[UTCCollectionDateTime],
		[UUID] ,
		[VMName] ,
		[VMHeartBeat] ,
		[DomainName] ,
		[BootTime] ,
		[NumCPUs] ,
		[CPULimit] ,
		[CPUReserve] ,
		[MemSize] ,
		[MemLimit] ,
		[MemReserve] )
	values (
		@SQLServerID ,
		@UTCCollectionDateTime , 
		@UUID ,
		@Name ,
		@HeartBeat ,
		@DomainName ,
		@BootTime ,
		@NumCPUs ,
		@CPULimit ,
		@CPUReserve ,
		@MemSize ,
		@MemLimit ,
		@MemReserve )
	
end
	