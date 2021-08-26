--------------------------------------------------------------------------------
--  Batch: Disk Drives header
--------------------------------------------------------------------------------
use master

declare 
	@command nvarchar(MAX), 
	@dbname nvarchar(255), 
	@cmptlevel smallint,
	@curconfig int,
	@disableole int
	
if (select isnull(object_id('tempdb..#disk_drives'), 0)) = 0 
begin 
	create table #disk_drives (
		drive_letter nvarchar(256), 
		unused_size dec(38,0),
		total_size dec(38,0),
		disk_idle nvarchar(30),
		disk_idle_base nvarchar(30),
		disk_queue nvarchar(30),
		timestamp_sys100ns nvarchar(30),
		AvgDisksecPerRead  nvarchar(30),
		AvgDisksecPerRead_Base nvarchar(30),
		AvgDisksecPerTransfer nvarchar(30),
		AvgDisksecPerTransfer_Base nvarchar(30),
		AvgDisksecPerWrite nvarchar(30),
		AvgDisksecPerWrite_Base nvarchar(30),
		Frequency_Perftime nvarchar(30),
		DiskReadsPerSec nvarchar(30),
		DiskTransfersPerSec nvarchar(30),
		DiskWritesPerSec nvarchar(30),
		TimeStamp_PerfTime nvarchar(30)
) 
end	
else
	truncate table #disk_drives

{0}