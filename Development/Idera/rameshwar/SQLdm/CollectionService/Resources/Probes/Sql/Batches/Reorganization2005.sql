--------------------------------------------------------------------------------
--  Batch: Table Reorganization
--------------------------------------------------------------------------------
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
@objectname nvarchar(128)

begin try

set @rowcountlimit = 500 --{1}	
set @timeoutseconds = 1 --{2}	

select @TimeoutTime = dateadd(ss,@timeoutseconds, getutcdate())

set
 @rowid = null
 
select 
@rowid = min(rowid)
from
tempdb..dmfragmentationworktable
where servicename = host_name()

while (isnull(@rowid,-1) >= 0)
begin
    if (GetUTCDate() > @TimeoutTime)           
	raiserror (N'Timeout', 11, 1);
	
	if (@rowcount > @rowcountlimit)
	raiserror (N'Rowcount Limit', 11, 1);

	select 
		@dbid = dbid,
		@objectid = objectid,
		@objectname = objectname
	from
		tempdb..dmfragmentationworktable
	where 
		rowid = @rowid
	
	select
		@objectname,
		@dbid,
		@objectid,
		sum(avg_fragmentation_in_percent * page_count) / nullif(sum(page_count),0)
	from 
		master.sys.dm_db_index_physical_stats(@dbid, @objectid, 1, null, 'limited') d
	where  alloc_unit_type_desc = 'IN_ROW_DATA' 

	select @rowcount = @rowcount + @@rowcount

	delete from tempdb..dmfragmentationworktable 
	where rowid = @rowid
	
	select 
	@rowid = min(rowid)
	from
	tempdb..dmfragmentationworktable
	where servicename = host_name()


end 
end try
begin catch
select error_message()
end catch		