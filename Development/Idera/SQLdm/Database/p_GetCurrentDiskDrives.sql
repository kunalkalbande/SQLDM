if (object_id('p_GetCurrentDiskDrives') is not null)
begin
drop procedure [p_GetCurrentDiskDrives]
end
go
create procedure [dbo].[p_GetCurrentDiskDrives]
				@SQLServerID int
as
begin
	declare @err int

	select DriveName from DiskDrives
		where SQLServerID = @SQLServerID and
			UTCCollectionDateTime = 
				(Select max(UTCCollectionDateTime) 
					from DiskDrives
					where SQLServerID = @SQLServerID
				)

	set @err = @@ERROR
	return @err
end