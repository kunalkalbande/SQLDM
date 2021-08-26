if (object_id('p_InsertESXConfigData') is not null)
begin
drop procedure p_InsertESXConfigData
end
go
create procedure p_InsertESXConfigData 
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@UUID nvarchar(128),
	@HostName nvarchar(256),
	@DomainName nvarchar(256),
	@ESXStatus int,
	@ESXBootTime datetime,
	@ESXCPUMhz int,
	@ESXNumCPUCores smallint,
	@ESXNumCPUPkgs smallint,
	@ESXNumCPUThreads smallint, 
	@ESXNumNics int,
	@ESXMemSize bigint,
	@ReturnMessage nvarchar(128) output
as
begin

	insert into [ESXConfigData] (
		[SQLServerID],
		[UTCCollectionDateTime],
		[UUID] , 
		[HostName] ,
		[DomainName] , 
		[Status] , 
		[BootTime] , 
		[CPUMHz] ,
		[NumCPUCores] ,
		[NumCPUPkgs] ,
		[NumCPUThreads] ,
		[NumNICs] ,
		[MemorySize] )
	values (
		@SQLServerID,
		@UTCCollectionDateTime ,
		@UUID , 
		@HostName ,
		@DomainName ,
		@ESXStatus ,
		@ESXBootTime ,
		@ESXCPUMhz ,
		@ESXNumCPUCores ,
		@ESXNumCPUPkgs ,
		@ESXNumCPUThreads ,
		@ESXNumNics ,
		@ESXMemSize )

end