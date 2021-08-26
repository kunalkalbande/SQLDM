if (object_id('p_InsertOSStatistics') is not null)
begin
drop procedure p_InsertOSStatistics
end
go
create procedure p_InsertOSStatistics
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@OSTotalPhysicalMemoryInKilobytes bigint,
	@OSAvailableMemoryInKilobytes bigint,
	@PagesPerSecond float,
	@ProcessorTimePercent float,
	@PrivilegedTimePercent float,
	@UserTimePercent float,
	@ProcessorQueueLength float,
	@DiskTimePercent float,
	@DiskQueueLength float,
	@ReturnMessage nvarchar(128) output
as
begin

insert into [OSStatistics]
	([SQLServerID]
	,[UTCCollectionDateTime]
	,[OSTotalPhysicalMemoryInKilobytes]
	,[OSAvailableMemoryInKilobytes]
	,[PagesPerSecond]
	,[ProcessorTimePercent]
	,[PrivilegedTimePercent]
	,[UserTimePercent]
	,[ProcessorQueueLength]
	,[DiskTimePercent]
	,[DiskQueueLength])
values
	(@SQLServerID,
	@UTCCollectionDateTime,
	@OSTotalPhysicalMemoryInKilobytes,
	@OSAvailableMemoryInKilobytes,
	@PagesPerSecond,
	@ProcessorTimePercent,
	@PrivilegedTimePercent,
	@UserTimePercent,
	@ProcessorQueueLength,
	@DiskTimePercent,
	@DiskQueueLength
	)

end


