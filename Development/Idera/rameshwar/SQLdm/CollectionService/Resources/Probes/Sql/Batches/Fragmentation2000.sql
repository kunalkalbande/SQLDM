--------------------------------------------------------------------------------
--  Batch: Table Fragmentation Collector 2000
--  Variables: 
--		[0] - rowcount limit
--		[1] - timeout
--		[2] - order
-------------------------------------------------------------------------------
set nocount on

declare 
@dbid int,
@objectid int,
@timeoutseconds int,
@parms nvarchar(100),
@rowcount int,
@rowcountlimit int,
@rowid int,
@TimeoutTime datetime,
@objectname nvarchar(128),
@schemaname nvarchar(128),
@dbname nvarchar(128),
@command nvarchar(1000),
@order tinyint,
@retry int,
@innerretry int

set @rowcountlimit = {0}	
set @timeoutseconds = {1}	
set @order = {2}

select @TimeoutTime = dateadd(ss,@timeoutseconds, getutcdate())

set
 @rowid = null

select @retry = isnull(min(retry),0)
from
	tempdb..dmfragmentationworktable
	where servicename =  host_name()
	and isnull(objectid,-1) > 0
	
-- After 2 retries start going one table at a time	
if @retry > 2
	set @rowcountlimit = 1	

if (@order % 2 = 1)
begin
	select 
	@rowid = min(rowid)
	from
	tempdb..dmfragmentationworktable
	where servicename = host_name()
	and isnull(objectid,-1) > 0
	and retry <= @retry
end
else
begin
	select 
	@rowid = max(rowid)
	from
	tempdb..dmfragmentationworktable
	where servicename = host_name()
	and isnull(objectid,-1) > 0
	and retry <= @retry
end	

while (isnull(@rowid,-1) >= 0)
begin
    if (GetUTCDate() > @TimeoutTime)           
	raiserror (N'Timeout', 11, 1);
	
	if (@rowcount > @rowcountlimit)
	raiserror (N'Rowcount Limit', 11, 1);

	update 
		tempdb..dmfragmentationworktable
	set
		retry = retry + 1
	where 
		rowid = @rowid 
				
	select 
		@dbid = dbid,
		@dbname = db_name(dbid),
		@objectid = objectid,
		@objectname = objectname,
		@schemaname = schemaname,
		@innerretry = retry
	from
		tempdb..dmfragmentationworktable
	where 
		rowid = @rowid
	
	-- Only retry 5 times for objects that seem to be causing problems
	if @innerretry > 5
	begin
		delete from tempdb..dmfragmentationworktable 
		where rowid = @rowid
		
		select 'Skipping object after 5 retries: DB ' + quotename(db_name(@dbid)) + ' (dbid: ' + cast(@dbid as nvarchar(5)) + ') OBJ ' + quotename(@objectname) + ' (objid: ' + cast(@objectid as nvarchar(10)) + ')'
	end
		
		if 	len(@dbname) > 0
		and HAS_DBACCESS (@dbname) = 1 
		and isnull(databaseproperty(@dbname, 'IsInLoad'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsSuspect'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsInRecovery'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsNotRecovered'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsOffline'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsShutDown'),0) = 0 
		and (
			isnull(databaseproperty(@dbname, 'IsSingleUser'),0) = 0 
			or ( 
				isnull(databaseproperty(@dbname, 'IsSingleUser'),0) = 1 
				and not exists 
				(select * from master..sysprocesses p where dbid = db_id(@dbname) and p.spid <> @@spid)
				)
			)
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
					or (rsc_type > 4 and rsc_objid = @objectid))
				) = 0 
			begin
				select @dbname, @schemaname
				-- Gather fragmentation
				select @command = 
				'use ' + quotename(@dbname) + '
				dbcc showcontig(' + convert(varchar(20), @objectid) + ') with fast, tableresults'
				execute (@command) 
			end	
		end
			select @rowcount = @rowcount + @@rowcount

	delete from tempdb..dmfragmentationworktable 
	where rowid = @rowid
	
	if (@order % 2 = 1)
	begin
		select 
		@rowid = min(rowid)
		from
		tempdb..dmfragmentationworktable
		where servicename = host_name()
		and isnull(objectid,-1) > 0
		and retry <= @retry
	end
	else
	begin
		select 
		@rowid = max(rowid)
		from
		tempdb..dmfragmentationworktable
		where servicename = host_name()
		and isnull(objectid,-1) > 0
		and retry <= @retry
	end	


end 
	
	
if (select isnull(count(*),0) from tempdb..dmfragmentationworktable where servicename = host_name() and objectid is not null) = 0
begin
	select 'Complete'
end
	