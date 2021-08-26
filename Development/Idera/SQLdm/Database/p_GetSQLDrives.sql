if (object_id('p_GetSQLDrives') is not null)
begin
drop procedure [p_GetSQLDrives]
end
go
create procedure [dbo].[p_GetSQLDrives]
as
begin
	if (select isnull(object_id('tempdb..#disk_drives'), 0)) = 0 
	begin 
		create table #disk_drives (
			drive_letter nvarchar(256), 
			unused_size dec(38,0)
		)
	end
	else
	begin
		truncate table #disk_drives
	end
	
	-- Populate unused disk space stats
	insert into #disk_drives(drive_letter, unused_size)  exec master..xp_fixeddrives   -- Fixed
	insert into #disk_drives(drive_letter, unused_size)  exec master..xp_fixeddrives 1 -- Remote
	
	select drive_letter from #disk_drives
end


