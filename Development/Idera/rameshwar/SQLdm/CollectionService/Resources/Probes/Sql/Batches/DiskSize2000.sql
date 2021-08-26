-- Disk Size 2000

-- Populate unused disk space stats
insert into #disk_drives(drive_letter, unused_size)  exec master..xp_fixeddrives   -- Fixed
insert into #disk_drives(drive_letter, unused_size)  exec master..xp_fixeddrives 1 -- Remote

update #disk_drives 
	set unused_size = unused_size * 1024.0 * 1024.0,
		drive_letter = upper(drive_letter)

insert into #disk_drives 
	select distinct upper(left(filename,1)), 
		null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null
	from master..sysaltfiles
	where upper(left(filename,1)) {0} not in (select upper(drive_letter) {0} from #disk_drives)


