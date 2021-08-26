if (object_id('p_GetDiskDrives') is not null)
begin
drop procedure [p_GetDiskDrives]
end
go
create procedure [dbo].[p_GetDiskDrives]
				@ServerID int
as
begin
	select DriveName 
	from DiskDrives (NOLOCK)
	where 
		SQLServerID = @ServerID and
		DriveName != 'No Drives Configured'
	group by DriveName 
end