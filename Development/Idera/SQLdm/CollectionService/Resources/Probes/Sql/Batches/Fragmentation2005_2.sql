--------------------------------------------------------------------------------
--  Batch: Table Fragmentation Collector 2005
--  Variables: 
--		[0] - rowcount limit
--		[1] - timeout
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
@order tinyint,
@retry int,
@innerretry int,
@fragmentationvalue float

begin try

set @rowcountlimit = {0}	
set @timeoutseconds = {1}	
set @order = {2}

select @TimeoutTime = dateadd(ss,@timeoutseconds, getutcdate())

set
 @rowid = null
 
select @retry = isnull(min(retry),0)
from
	##dmfragmentationworktable
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
	##dmfragmentationworktable
	where servicename = host_name()
	and isnull(objectid,-1) > 0
	and retry <= @retry
end
else
begin
	select 
	@rowid = max(rowid)
	from
	##dmfragmentationworktable
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
		##dmfragmentationworktable
	set
		retry = retry + 1
	where 
		rowid = @rowid 
				
	select 
		@dbid = dbid,
		@objectid = objectid,
		@objectname = objectname,
		@schemaname = schemaname,
		@innerretry = retry,
		@fragmentationvalue = fragmentation
	from
		##dmfragmentationworktable
	where 
		rowid = @rowid
	
	-- Only retry 5 times for objects that seem to be causing problems
	if @innerretry > 5
	begin
		delete from ##dmfragmentationworktable 
		where rowid = @rowid
		
		select 'Skipping object after 5 retries: DB ' + quotename(db_name(@dbid)) + ' (dbid: ' + cast(@dbid as nvarchar(5)) + ') OBJ ' + quotename(@objectname) + ' (objid: ' + cast(@objectid as nvarchar(10)) + ')'
	end
	
	if (@fragmentationvalue is null)
	begin
		select
			@fragmentationvalue = sum(avg_fragmentation_in_percent * page_count) / nullif(sum(page_count),0)
		from 
			master.sys.dm_db_index_physical_stats(@dbid, @objectid, 1, null, 'limited') d
		where  alloc_unit_type_desc = 'IN_ROW_DATA' 
	end
	
	select
		db_name(@dbid),
		@objectname,
		@schemaname,
		@fragmentationvalue

	select @rowcount = @rowcount + @@rowcount

	if (GetUTCDate() > @TimeoutTime)   
	begin
		-- Save fragmentation data for later pickup in case we timed out during this loop
		update ##dmfragmentationworktable
		set fragmentation = @fragmentationvalue
		where rowid = @rowid
	end
	else
	begin
		delete from ##dmfragmentationworktable
		where rowid = @rowid
	end
	
	if (@order % 2 = 1)
	begin
		select 
		@rowid = min(rowid)
		from
		##dmfragmentationworktable
		where servicename = host_name()
		and isnull(objectid,-1) > 0
		and retry <= @retry
	end
	else
	begin
		select 
		@rowid = max(rowid)
		from
		##dmfragmentationworktable
		where servicename = host_name()
		and isnull(objectid,-1) > 0
		and retry <= @retry
	end	

end 
end try
begin catch
	select error_message()
end catch		

if (select isnull(count(*),0) from ##dmfragmentationworktable where servicename = host_name() and objectid is not null) = 0
begin
	select 'Complete'
end