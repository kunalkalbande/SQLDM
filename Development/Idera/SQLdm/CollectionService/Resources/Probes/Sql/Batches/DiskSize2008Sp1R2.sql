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

INSERT INTO #disk_drives(drive_letter, unused_size)  
  SELECT DISTINCT UPPER(LEFT(volume_mount_point, 1)) AS drive, available_bytes/1024/1024 AS 'MB free'
    FROM sys.master_files AS f CROSS APPLY sys.dm_os_volume_stats(f.database_id, f.file_id)
  WHERE UPPER(LEFT(volume_mount_point,1))  {0} NOT IN (SELECT UPPER(drive_letter)  {0} FROM #disk_drives)