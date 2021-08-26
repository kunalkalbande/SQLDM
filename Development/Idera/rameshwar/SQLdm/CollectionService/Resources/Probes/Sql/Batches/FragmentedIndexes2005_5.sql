---------------------------------------
-- Azure SQL Database Managed Instance
---------------------------------------

----------------------------------------------------------------------------------------------
--  Test the most active indexes within the database to see if they are fragmented.
--  Currently the top 10 most active indexes based on user reads that are larger than 1000 
--  pages will be tested to see if they are more than 30% fragmented.  Any found matching this 
--  will be returned.
----------------------------------------------------------------------------------------------

declare @indexes table (object_id int, index_id int, name sysname, partition_number int);
declare @frag table (object_id int, index_id int, name sysname, partition_number int, frag_percent float);
declare @loop int;
declare @frag_percent float;
declare @object_id int, @index_id int, @name sysname, @partition_number int;
declare @dbid int;
declare @old datetime;
declare @servername varchar(255);
declare @slashpos int;
declare @bufferPages bigint;

--START (RRG): Get internal for Instance Name when in Azure Platform
declare @sysperfinfoname sysname
Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))
		
--extract one row that contains the actual instance name and exclude others
--there should be only one instance name in this format MSSQL$XXXXXXXXXXXX where X is HEX char A-F and numbers 0-9
select @sysperfinfoname = (select top 1 object_name from sys.dm_os_performance_counters (nolock) where lower(object_name) like '%MSSQL$%')
select @sysperfinfoname = SUBSTRING(@sysperfinfoname, 1, PATINDEX('%:%',@sysperfinfoname)-1)
--END (RRG): Get internal for Instance Name when in Azure Platform

select @bufferPages = isnull(sum(convert(bigint,cntr_value)),0) from sys.dm_os_performance_counters 
	where (lower(object_name) = lower(@sysperfinfoname + ':buffer manager') and lower(counter_name) = 'total pages')

----------------------------------------------------------------------------------------------
--  Consider indexes that have not been used in over 7 days too old to include in our analysis
--
set @old = dateadd(dd, -7, getdate());

select @dbid = db_id();

begin
	insert into @indexes(object_id, index_id, name, partition_number) 
		select top 100 i.object_id, i.index_id, i.name, ps.partition_number from sys.indexes i  
				left outer join sys.dm_db_partition_stats ps on i.object_id = ps.object_id and i.index_id = ps.index_id
				left outer join sys.dm_db_index_usage_stats s on i.object_id = s.object_id and i.index_id = s.index_id  and s.database_id = @dbid
				where i.index_id > 0
				and i.is_disabled = 0
				and i.is_hypothetical = 0
				and objectproperty(i.[object_id], 'IsMSShipped') = 0 -- ignore system indexes
				and (ps.used_page_count >= 500 and ps.used_page_count <= 262144) -- only indexes between 500 (4mb) and 262,144 (2GB) pages in size 
				and ((s.user_seeks + s.user_scans + s.user_lookups) > 20 or s.user_scans > 0) -- make sure that there has been activity 
				and (  
					   (s.last_user_seek is not null and s.last_user_seek >= @old) or 
					   (s.last_user_scan is not null and s.last_user_scan >= @old) or
					   (s.last_user_lookup is not null and s.last_user_lookup >= @old)
					)
			order by ((user_seeks*10) + (user_scans*used_page_count) + user_lookups) desc;
end

set @loop = 0;
while (select count(*) from @indexes) > 0
begin
    set @loop = @loop + 1;
    if @loop > 100 begin break; end -- sanity check to prevent infinite looping which should never happen
    select top 1 @object_id = object_id, @index_id = index_id, @name = name, @partition_number = partition_number from @indexes;
    select @frag_percent = avg_fragmentation_in_percent from sys.dm_db_index_physical_stats (@dbid, @object_id, @index_id, @partition_number, 'LIMITED');
    if (@frag_percent >= 20.0) -- indexes over 20% fragmented will be returned
    begin
        insert into @frag(object_id, index_id, name, partition_number, frag_percent) select @object_id, @index_id, @name, @partition_number, @frag_percent;
    end 
    delete from @indexes where object_id = @object_id and index_id = @index_id and partition_number = @partition_number;
end

-- return any fragmented indexes found
select 
	[DatabaseName]=db_name(), 
	[Schema]= (Select top 1 s.name from sys.objects o inner join sys.schemas s on o.schema_id = s.schema_id where o.object_id = object_id), 
	[TableName]=object_name(object_id), 
	[IndexName]=name, 
	[IndexId]=index_id, 
	[Partition]=partition_number, 
	[FragPercent]=frag_percent,
	[PartitionPages]=(select sum(isnull(used_page_count,0)) from sys.dm_db_partition_stats ps where ps.object_id = f.object_id and ps.index_id = f.index_id and ps.partition_number = f.partition_number),
	[TablePages]=(select sum(isnull(used_page_count,0)) from sys.dm_db_partition_stats ps where ps.object_id = f.object_id),
	[TotalServerBufferPages]=@bufferPages
	from @frag f