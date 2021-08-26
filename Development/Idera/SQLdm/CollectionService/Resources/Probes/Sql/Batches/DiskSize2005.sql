-- Disk Size 2005

-- Populate unused disk space stats
begin try
insert into #disk_drives(drive_letter, unused_size)  exec master..xp_fixeddrives   -- Fixed
insert into #disk_drives(drive_letter, unused_size)  exec master..xp_fixeddrives 1 -- Remote

update #disk_drives 
	set unused_size = unused_size * 1024.0 * 1024.0,
		drive_letter = upper(drive_letter)

insert into #disk_drives(drive_letter)
	select distinct upper(left(physical_name,1))
	from master.sys.master_files
	where upper(left(physical_name,1)) {0} not in (select upper(drive_letter) {0} from #disk_drives)
end try
begin catch
end catch