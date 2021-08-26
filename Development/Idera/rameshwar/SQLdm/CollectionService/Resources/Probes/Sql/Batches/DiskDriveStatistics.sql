-- SQLdm9.1 (Ankit Srivastava) New Batch for Disk Drive Statistics
select drive_letter,unused_size,total_size,DiskReadsPerSec,DiskWritesPerSec, getutcdate() from #disk_drives 