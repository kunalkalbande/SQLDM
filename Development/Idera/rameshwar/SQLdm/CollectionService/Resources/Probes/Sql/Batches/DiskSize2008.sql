-- Disk Size 2005
IF ((IS_SRVROLEMEMBER('sysadmin') = 1))
BEGIN
    -- Populate unused disk space stats
    insert into #disk_drives(drive_letter, unused_size)  exec master..xp_fixeddrives   -- Fixed
    insert into #disk_drives(drive_letter, unused_size)  exec master..xp_fixeddrives 1 -- Remote
    
    update #disk_drives 
    	set unused_size = unused_size * 1024.0 * 1024.0,
    		drive_letter = upper(drive_letter)
END

IF OBJECT_ID('master.sys.master_files', 'V') IS NOT NULL
BEGIN
	insert into #disk_drives(drive_letter)
		select distinct upper(left(physical_name,1))
		from master.sys.master_files
		where upper(left(physical_name,1)) {0} not in (select upper(drive_letter) {0} from #disk_drives)
END
