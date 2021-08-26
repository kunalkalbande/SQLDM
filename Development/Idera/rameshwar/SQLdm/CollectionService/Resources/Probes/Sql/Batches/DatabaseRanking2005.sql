
select top 100 
	[Database]=db_name(database_id), 
	[IO]=sum(num_of_bytes_read + num_of_bytes_written) 
	from sys.dm_io_virtual_file_stats(null, null) 
	where database_id > 4
	group by database_id
	order by 2 desc
