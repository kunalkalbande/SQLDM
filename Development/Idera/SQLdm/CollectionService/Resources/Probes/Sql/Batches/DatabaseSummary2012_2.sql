
select DB_NAME();
declare  
	@dbname nvarchar(255), 
	@dbid smallint, 
	@mode smallint,
	@status int,
	@status2 int,
	@category int,
	@crdate datetime,
	@lastbackup datetime,
	@compatibility int,
	@databasestatus int = 0,
	@collectsize bit,
	@recovery nvarchar(120)

declare @MasterDatabaseStatePermission int
SELECT @MasterDatabaseStatePermission = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')

IF 1 = HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'VIEW DATABASE STATE')
BEGIN
select @dbname = d.name, @status = d.status,@status2 = d.status2,@category = d.category, @crdate = d.crdate,
@lastbackup = logStats.log_backup_time, @compatibility = cmptlevel from sysdatabases d
OUTER APPLY sys.dm_db_log_stats(d.dbid) logStats
where d.name = DB_NAME()
END
ELSE
BEGIN
select @dbname = d.name, @status = d.status,@status2 = d.status2,@category = d.category, @crdate = d.crdate,
@lastbackup = NULL, @compatibility = cmptlevel from sysdatabases d
where d.name = DB_NAME()
END

IF 1 = HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'VIEW DATABASE STATE')
with SizeStats(Data_Size,TextImage_Size,Used_Pages) as (select 
			Data_Size = sum(case when it.internal_type in (202,204) then 0 when p.index_id < 2 then convert(dec(38,0),au.data_pages) else 0 end) * 8,   
			TextImage_Size = sum(case when it.internal_type in (202,204) then 0 when au.type != 1 then convert(dec(38,0),au.used_pages) else 0 end) * 8, 
			Used_Pages = sum(convert(dec(38,0),au.used_pages)) * 8
			from sys.partitions p 
			left join sys.allocation_units au on p.partition_id = au.container_id   
			 left join sys.internal_tables it on p.object_id = it.object_id )
			select 'Database', @dbname, 
			 dbstatus = @databasestatus, 			
			 status = @status ,  
			 status2 =  @status2  ,  
			 category =  @category ,  
			 crdate = @crdate, 
			 lastbackup =  case when @lastbackup is null then  NULL else @lastbackup end ,
			 compatibility =  @compatibility  ,
			 systemTables = (select count(*) from sysobjects where xtype = 'S'), 
			 userTables = (select count(*) from sysobjects where xtype = 'U'), 
			 filecount = (select count(*) from sysfiles), 
			 filegroupcount = (select count(*) from sysfilegroups), 
			 Allocated_DBSize = (select isnull(sum(convert(dec(38,0),size) * 8), 0) from 
			 sysfiles (nolock) where status&1000000 = 0), 
			Data_Size,TextImage_Size,(Used_Pages - Data_Size - TextImage_Size) as Index_Size,
			Processes = (select isnull(count(*),0) from sysprocesses where dbid =  @dbid   and spid <>  @@spid ), 
			autogrow = (select sum(distinct(case 
				when growth <> 0 and status&1000000 = 0 then 1 
				when growth = 0 and status&1000000 = 0 then 4
				when growth <> 0 and status&1000000 <> 0 then 8 
				when growth = 0 and status&1000000 <> 0 then 16
				else 0 end)) from sysfiles), 
			recovery =  + (select case @recovery when 'SIMPLE' then 3  when 'BULK_LOGGED' then 2  else 1 end)			 
			,[Total_Used] = case when @compatibility >= 80 then (select sum(convert(dec(38,0), fileproperty(name,'SpaceUsed') ) * 8) from  
				sysfiles where status&1000000 = 0) else  NULL  end
			,cast((select avg(xtp_storage_percent) 
				from sys.dm_db_resource_stats) as decimal) as [InMemoryStoragePct]
			from SizeStats
ELSE
with SizeStats(Data_Size,TextImage_Size,Used_Pages) as (select 
			Data_Size = sum(case when it.internal_type in (202,204) then 0 when p.index_id < 2 then convert(dec(38,0),au.data_pages) else 0 end) * 8,   
			TextImage_Size = sum(case when it.internal_type in (202,204) then 0 when au.type != 1 then convert(dec(38,0),au.used_pages) else 0 end) * 8, 
			Used_Pages = sum(convert(dec(38,0),au.used_pages)) * 8
			from sys.partitions p 
			left join sys.allocation_units au on p.partition_id = au.container_id   
			 left join sys.internal_tables it on p.object_id = it.object_id )
			select 'Database', @dbname, 
			 dbstatus = @databasestatus, 			
			 status = @status ,  
			 status2 =  @status2  ,  
			 category =  @category ,  
			 crdate = @crdate, 
			 lastbackup =  case when @lastbackup is null then  NULL else @lastbackup end ,
			 compatibility =  @compatibility  ,
			 systemTables = (select count(*) from sysobjects where xtype = 'S'), 
			 userTables = (select count(*) from sysobjects where xtype = 'U'), 
			 filecount = (select count(*) from sysfiles), 
			 filegroupcount = (select count(*) from sysfilegroups), 
			 Allocated_DBSize = (select isnull(sum(convert(dec(38,0),size) * 8), 0) from 
			 sysfiles (nolock) where status&1000000 = 0), 
			Data_Size,TextImage_Size,(Used_Pages - Data_Size - TextImage_Size) as Index_Size,
			Processes = (select isnull(count(*),0) from sysprocesses where dbid =  @dbid   and spid <>  @@spid ), 
			autogrow = (select sum(distinct(case 
				when growth <> 0 and status&1000000 = 0 then 1 
				when growth = 0 and status&1000000 = 0 then 4
				when growth <> 0 and status&1000000 <> 0 then 8 
				when growth = 0 and status&1000000 <> 0 then 16
				else 0 end)) from sysfiles), 
			recovery =  + (select case @recovery when 'SIMPLE' then 3  when 'BULK_LOGGED' then 2  else 1 end)			 
			,[Total_Used] = case when @compatibility >= 80 then (select sum(convert(dec(38,0), fileproperty(name,'SpaceUsed') ) * 8) from  
				sysfiles where status&1000000 = 0) else  NULL  end
			,NULL as [InMemoryStoragePct]
			from SizeStats

select 'DB Expansion', NULL
	union select 'Log Expansion', NULL

if DB_NAME() = 'master'
begin
	declare @command nvarchar(MAX)
	if (select isnull(object_id('tempdb..#totaldbsizes'), 0)) = 0 
		create table #totaldbsizes (usedsize dec(38,0)) 
	else 
		truncate table #totaldbsizes 

	select @command = 'insert into #totaldbsizes select sum(convert(dec(38,0),used_pages))*8 from ' 		
			+ 'sys.allocation_units'
			
	execute(@command)

	select
			'summary'
			,(select
			count(*)
			from
			sysdatabases)
			,0
			,0
			,Convert(dec(38,0), 0)
			,Convert(dec(38,0), 0)
			,(select sum(usedsize) from #totaldbsizes)
	drop table #totaldbsizes
			
end

IF 1 = HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'VIEW DATABASE STATE')
BEGIN
BEGIN TRY
SELECT DB_NAME() AS [Database Name]
	,convert(float, (total_log_size_in_bytes*1.0/1024/1024)) AS [Log Size (MB)]
	,convert(float, used_log_space_in_percent) AS [Log Space Used (%)]
	,0 AS [Status]
	
FROM sys.dm_db_log_space_usage
END TRY
BEGIN CATCH
	SELECT DB_NAME() AS [Database Name]
	,NULL AS [Log Size (MB)]
	,NULL AS [Log Space Used (%)]
	,0 AS [Status]
END CATCH
END
ELSE
BEGIN
	SELECT DB_NAME() AS [Database Name]
	,NULL AS [Log Size (MB)]
	,NULL AS [Log Space Used (%)]
	,0 AS [Status]
END

if DB_NAME() = 'master'
begin
	
	-- COUNTERS START
	declare @sysperfinfoname sysname
	Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))
		
	--extract one row that contains the actual instance name and exclude others
	--there should be only one instance name in this format MSSQL$XXXXXXXXXXXX where X is HEX char A-F and numbers 0-9
	IF 1 = HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'VIEW DATABASE STATE')
		select @sysperfinfoname = (select top 1 object_name from sys.dm_os_performance_counters (nolock) where lower(object_name) like '%MSSQL$%')
	select @sysperfinfoname = SUBSTRING(@sysperfinfoname, 1, PATINDEX('%:%',@sysperfinfoname)-1)
	
	IF 1 = HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'VIEW DATABASE STATE')
	select 
			counter_name, 
			convert(bigint,isnull(cntr_value,0)), 
			--instance_name,
			DBName = d.name
		from 
			sys.dm_os_performance_counters pc
			inner join sys.databases d on d.physical_database_name = pc.instance_name
		where 			
			lower(object_name) = lower(@sysperfinfoname + ':databases') 
			and lower(counter_name) in ('transactions/sec', 'log flushes/sec', 'log bytes flushed/sec', 'log flush waits/sec', 'log cache reads/sec', 'log cache hit ratio', 'log cache hit ratio base') 
			and lower(instance_name) <> '_total' 
			and lower(instance_name) <> 'mssqlsystemresource'
		order by 
			d.name 
	ELSE
		SELECT NULL as [counter_name], NULL, NULL as [DBName]
end
