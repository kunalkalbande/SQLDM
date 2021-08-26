-- SQLdm 9.1 (Abhishek Joshi)

-- Filegroup and Mountpoint Monitoring Improvements
-- retrieves all the disks corresponding to a SQL Server

-- exec p_GetAllDisks  @SQLServerID = 14

if (object_id('p_GetAllDisks') is not null)
begin
	drop procedure [p_GetAllDisks]
end
go

create procedure [dbo].[p_GetAllDisks]
	@SQLServerID int = null
as
begin
	
	select 
		distinct SQLServerID,
		DriveName
	from 
		DiskDriveStatistics
	where
		SQLServerID = (case when @SQLServerID is null then SQLServerID else @SQLServerID end)

end
go
