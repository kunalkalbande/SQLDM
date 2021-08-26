-- SQL Diagnostic Manager v10.3.0.0
-- Copyright © IDERA, Inc. 2003-2017 

set transaction isolation level read uncommitted 
 set lock_timeout 20000 
 set implicit_transactions off 
 if @@trancount > 0 commit transaction 
 set language us_english 
 set cursor_close_on_commit off 
 set query_governor_cost_limit 0 
 set numeric_roundabort off
--------------------------------------------------------------------------------
--  Batch: Fragmentation Permissions
--  Tables: master.sys.dm_db_index_physical_stats
--  Variables: 
--		[0] - rowcount limit
--		[1] - timeout
--		[2] - order
--------------------------------------------------------------------------------
--  SQLdm 10.3 (Pranay Sharma)-- Created batch to get Fragmentation Permissions Information for Current User
---------------------------------------------------------------------------------------------------
set nocount on


BEGIN TRY

-- Table: Permissions Table to store the permissions
-- Permission Name : Name of the permissions
-- Permission Value : Value of the permissions
DECLARE @FragmentationPermissionsTable TABLE
(
	PermissionName nvarchar(256),
	PermissionValue int
)

-- Permission Value : To store value of the permission
DECLARE @FragmentationPermissionValue INT
SET @FragmentationPermissionValue = 1



declare 
@dbid int,
@objectid int,
@timeoutseconds int,
@rowcount int,
@rowcountlimit int,
@rowid int,
@TimeoutTime datetime,
@objectname nvarchar(128),
@schemaname nvarchar(128),
@order tinyint,
@retry int,
@innerretry int,
@fragmentationvalue float,
@hostName nvarchar(max)

set @rowcountlimit = 500	
set @timeoutseconds = 1740	
set @order = 3

select @TimeoutTime = dateadd(ss,@timeoutseconds, getutcdate())

set
 @rowid = null

SET @hostName = host_name()
 
select @retry = isnull(min(retry),0)
from
	##dmfragmentationworktable
	where servicename =  @hostName
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
	where servicename = @hostName
	and isnull(objectid,-1) > 0
	and retry <= @retry
end
else
begin
	select 
	@rowid = max(rowid)
	from
	##dmfragmentationworktable
	where servicename = @hostName
	and isnull(objectid,-1) > 0
	and retry <= @retry
end	
WHILE (isnull(@rowid,-1) >= 0 and GetUTCDate() <= @TimeoutTime and @rowcount <= @rowcountlimit)
begin
 
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
	
	if (@fragmentationvalue is null)
	begin
	    --  Check Control Permissions present on Object with Object Id
		IF NOT EXISTS (SELECT 1 FROM sys.fn_my_permissions(OBJECT_NAME(@objectid),'OBJECT') where permission_name = 'CONTROL')
		BEGIN
			SELECT @FragmentationPermissionValue = 0;
			BREAK;	
		END
	end
	
	select @rowcount = @rowcount + @@rowcount
	
	if (@order % 2 = 1)
	begin
		select 
		@rowid = min(rowid)
		from
		##dmfragmentationworktable
		where servicename = @hostName
		and isnull(objectid,-1) > 0
		and retry <= @retry
	end
	else
	begin
		select 
		@rowid = max(rowid)
		from
		##dmfragmentationworktable
		where servicename = @hostName
		and isnull(objectid,-1) > 0
		and retry <= @retry
	end	
end


-- INSERT FragmentationPermissionsTable
INSERT INTO @FragmentationPermissionsTable VALUES('FRAGMENTATION', @FragmentationPermissionValue)

-- SELECT Values FROM @PermissionsTable
SELECT PermissionName, PermissionValue FROM @FragmentationPermissionsTable

END TRY

BEGIN CATCH
    declare @FragmentationErrorMessage nvarchar(max), @FragmentationErrorSeverity int, @FragmentationErrorState int;
    select @FragmentationErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @FragmentationErrorSeverity = ERROR_SEVERITY(), @FragmentationErrorState = ERROR_STATE();
    raiserror (@FragmentationErrorMessage, @FragmentationErrorSeverity, @FragmentationErrorState); -- raising the error and logging it as warning 
END CATCH
