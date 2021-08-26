--------------------------------------------------------------------------------
--  Batch: Table Reorganization
--  Variables: 
--		[0] - Database exclude string
--		[1] - Include system tables
--		[2] - Minimum table size in pages
--		[3] - List of databases to retry -- delete from @databases where dbid not in ()
--		[4] - List of tables to retry -- select 0, 0, 0, ''
--		[5] - Reverse order (0 or 1)
--------------------------------------------------------------------------------
use master
declare 
@command nvarchar(2048), 
@dbname nvarchar(255), 
@mode smallint, 
@dbid int, 
@tableid int,
@maxdb int,
@maxtable int,
@issystemtable bit,
@databasestatus int,
@proceed bit,
@reverseorder bit

set @reverseorder = {5}

declare @databases table(dbid int)
declare @retrytables table(dbid int, tableid int, issystemtable int, username nvarchar(50))
 
if (select isnull(object_id('tempdb..#reorgtables'), 0)) = 0 
	create table #reorgtables(tableid int, issystemtable int, username nvarchar(50)) 
else
	truncate table #reorgtables

insert into @databases
select
	dbid
from 
	master..sysdatabases db
where
	lower(name) not in ('mssqlsystemresource'{0})

{3}

insert into @retrytables
	select 0, 0, 0, ''
	{4}
	

select * from @databases

if @reverseorder = 1
	update @databases set dbid = dbid * -1
		
select 
	@dbid = min(dbid),
	@maxdb = max(dbid)
from
	@databases

while @dbid <= @maxdb
begin

	if @reverseorder = 1
		set @dbid = @dbid * -1
	
	select @dbname = db_name(@dbid), @proceed = 0
	
	select @databasestatus = 
		case when isnull(databasepropertyex(@dbname,'IsInStandby'),0) = 1 then 16 else 0 end 									--Standby
		+ case when isnull(databaseproperty(@dbname, 'IsInLoad'),0) = 1 or db.status & 32 = 32 then 32 else 0 end				--Restoring (non-mirror)
		+ case when db.status & 64 = 64 then 64 else 0 end																			--Pre-Recovery
		+ case when isnull(databaseproperty(@dbname, 'IsInRecovery'),0) = 1 or db.status & 128 = 128 then 128 else 0 end		--Recovering
		+ case when isnull(databaseproperty(@dbname, 'IsNotRecovered'),0) = 1 or isnull(databaseproperty(@dbname, 'IsSuspect'),0) = 1 
			or isnull(databaseproperty(@dbname, 'IsShutDown'),0) = 1 or db.status & 256 = 256 then 256 else 0 end				--Suspect
		+ case when isnull(databaseproperty(@dbname, 'IsOffline'),0) = 1 or db.status & 512 = 512 then 512 else 0 end			--Offline
		+ case when isnull(databaseproperty(@dbname, 'IsReadOnly'),0) = 1 or db.status & 1024 = 1024 then 1024 else 0 end		--Read Only
		+ case when isnull(databaseproperty(@dbname, 'IsDboOnly'),0) = 1 or db.status & 2048 = 2048 then 2048 else 0 end		--DBO Use
		+ case when isnull(databaseproperty(@dbname, 'IsSingleUser'),0) = 1 or db.status & 4096 = 4096 then 4096 else 0 end		--Single User
		+ case when isnull(databaseproperty(@dbname, 'IsEmergencyMode'),0) = 1 or db.status & 32768 = 32768 then 32768 else 0 end	--Emergency Mode	
		+ case when db.status & 1073741824 = 1073741824 then 1073741824 else 0 end	,
		@mode = mode
	from 
		sysdatabases db
	where dbid = @dbid

	
	-- Decide whether to collect size information
	if
		(@databasestatus & 8 <> 8 --Restoring Mirror
		and @databasestatus & 32 <> 32 --Restoring
		and @databasestatus & 64 <> 64 --Pre-Recovery
		and @databasestatus & 128 <> 128 --Recovering
		and @databasestatus & 256 <> 256 --Suspect
		and @databasestatus & 512 <> 512 --Offline
		and (@databasestatus & 4096 <> 4096
			or (@databasestatus & 4096 = 4096 
				and not exists 
				(select * from master..sysprocesses p where dbid = @dbid and p.spid <> @@spid)
				and not exists
				(select * from master..syslockinfo l where rsc_dbid = @dbid and l.req_spid <> @@spid)
			))  --Single User
		)
			select @proceed = 1

	-- If we cannot access database, but there is no known reason, set database to Inacessible
	if	@proceed = 1 and (has_dbaccess (@dbname) <> 1 or @mode <> 0)
		select @databasestatus = @databasestatus + 8192, @proceed = 0


	if @proceed = 0
	begin
		-- Try again later
		select 'Retry database', @dbname, @dbid
	end
	else
	begin
		if (select count(*) from @retrytables where dbid = @dbid) > 0
		begin
			insert into #reorgtables
				select tableid, issystemtable, username from @retrytables where dbid = @dbid

			set @command = 'use [' + replace(@dbname,char(93),char(93)+char(93)) + ']' +
				N'delete from #reorgtables
				where tableid not in 
				(select 
				id
				from 
				sysobjects o
				where o.id in (select id from sysindexes where indid = 1 and rowcnt > 0 and used >= {2}) 
				and upper(o.type) in (''{1}'',''U''))'

			exec(@command)
		end
		else
		begin
			-- Make a list of tables
			set @command =
				N'select 
				id,
				issystemtable = case when category & 2 = 2 then cast(1 as bit) else cast(0 as bit) end,
				username = u.name
				from 
				[' + replace(@dbname,char(93),char(93)+char(93)) + ']..sysobjects o
				left join [' + replace(@dbname,char(93),char(93)+char(93)) + ']..sysusers u on u.uid = o.uid
				where o.id in (select id from [' + replace(@dbname,char(93),char(93)+char(93)) + ']..sysindexes where indid = 1 and rowcnt > 0 and used >= {2}) 
				and upper(o.type) in (''{1}'',''U'')'

			insert into #reorgtables
				exec sp_executesql @command
		end

		select 
			'TableList',
			@dbname, 
			@dbid,
			tableid,
			issystemtable,
			username
		from #reorgtables

		if @@rowcount = 0
			select 
				'Skip database',
				@dbname,
				@dbid

		select 
			@tableid = min(tableid),
			@maxtable = max(tableid)
		from
			#reorgtables

		while @tableid <= @maxtable
		begin
			-- Check for locked tables		
			if (select 
					count(*) 
				from 
					master..syslockinfo (nolock) 
				where 
					rsc_dbid = @dbid 
					and req_mode not in (3,8) 
					and (rsc_type = 2 
					or (rsc_type > 4 and rsc_objid = @tableid))
				) = 0 
			begin
				-- Gather fragmentation
				select @command = 
				'use [' + replace(@dbname,char(93),char(93)+char(93)) + '] 
				dbcc showcontig(' + convert(varchar(20), @tableid) + ') with fast, tableresults'
				execute (@command) 
			end	
			else
			begin 
				-- Retry later
				select 
					'Retry Table',
					@dbid, 
					@tableid,
					issystemtable, 
					username 
				from
					#reorgtables
				where tableid = @tableid
			end
				
		
			delete from #reorgtables where tableid = @tableid

			select 
				@tableid = min(tableid)
			from #reorgtables

		end
	end

	delete from #reorgtables

	if @reverseorder = 1
		delete from @databases where dbid = @dbid * -1
	else
		delete from @databases where dbid = @dbid


	select 
		@dbid = min(dbid)
	from @databases
end

delete from @databases
drop table #reorgtables