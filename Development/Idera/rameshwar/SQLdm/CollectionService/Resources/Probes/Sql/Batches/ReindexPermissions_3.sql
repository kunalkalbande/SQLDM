--------------------------------------------------------------------------------
--  Batch: Reindex Permissions
--  Tables: master.sys.dm_db_index_physical_stats
--  Variables: 
--		[0] - Target Database
--		[1] - Table ID
--		[2] - user_name or schema_name
--------------------------------------------------------------------------------
--  SQLdm 10.3 (Varun Chopra)-- Created batch to get Reindex Permissions Information for Current User
---------------------------------------------------------------------------------------------------


BEGIN TRY

-- Table: Permissions Table to store the permissions
-- Permission Name : Name of the permissions
-- Permission Value : Value of the permissions
DECLARE @ReindexPermissionsTable TABLE
(
	PermissionName nvarchar(256),
	PermissionValue int
)

-- Permission Value : To store value of the permission
DECLARE @ReindexPermissionValue INT

--------------------------------------------------------------------------------
--  Batch: Reindex
--  Tables: sysobjects
--  Variables: 
--		[0] - Target Database
--		[1] - Table ID
--		[2] - Index Name
--------------------------------------------------------------------------------
use [{0}]

declare @qualified_table_name nvarchar(500), @command nvarchar(max), @id bigint, @type int, @indexid bigint, @cmptlevel int
set @id = {1}

select 
	@qualified_table_name = quotename(schema_name(uid)) + '.' + quotename(name) 
from
	sysobjects
where 
	id = @id

select @cmptlevel = compatibility_level from master.sys.databases where database_id = db_id()


if (len(@qualified_table_name) > 0)
begin
    
	-- Check Alter Permissions
	SELECT @ReindexPermissionValue = COUNT(*) FROM sys.fn_my_permissions(@qualified_table_name, 'OBJECT') WHERE permission_name = 'ALTER'
	-- INSERT ReindexPermissionsTable
	INSERT INTO @ReindexPermissionsTable VALUES('ReindexAlterTable', @ReindexPermissionValue)

	if (@cmptlevel > 80)
	begin
		if (@indexid is null)
		begin
			select @indexid = index_id from sys.indexes where index_id = 1 and object_id = @id
		end
		
		declare @idIN bigint, @indexidIN bigint
		SET @idIN = @id
		SET @indexidIN = @indexid 
		
		-- Select 1 if Failed Control Permissions are not found
		select @ReindexPermissionValue = CASE WHEN COUNT(*) = 0 THEN 1 ELSE 0 END 
		from master.sys.dm_db_index_physical_stats(db_id(), @idIN, @indexidIN, null, 'limited') 
		WHERE -- Select Failed Control Permissions Entries
		NOT EXISTS
		(SELECT 1 FROM sys.fn_my_permissions(OBJECT_NAME(object_id),'OBJECT') where permission_name = 'CONTROL')
		-- INSERT ReindexPermissionsTable
		INSERT INTO @ReindexPermissionsTable VALUES('ReindexControlPermissions', @ReindexPermissionValue)
		-- INSERT ReindexPermissionsTable
		INSERT INTO @ReindexPermissionsTable VALUES('ReindexShowContig', 1)
	end
	else
	begin
		SELECT @ReindexPermissionValue = COUNT(*) FROM sys.fn_my_permissions(@qualified_table_name, 'OBJECT') WHERE permission_name = 'TAKE OWNERSHIP'
		-- INSERT ReindexPermissionsTable
		INSERT INTO @ReindexPermissionsTable VALUES('ReindexShowContig', @ReindexPermissionValue)
		-- INSERT ReindexPermissionsTable
		INSERT INTO @ReindexPermissionsTable VALUES('ReindexControlPermissions', 1)
	end
end
ELSE
BEGIN
	-- INSERT ReindexPermissionsTable
	INSERT INTO @ReindexPermissionsTable VALUES('ReindexAlterTable', 1)
	-- INSERT ReindexPermissionsTable
	INSERT INTO @ReindexPermissionsTable VALUES('ReindexControlPermissions', 1)
	-- INSERT ReindexPermissionsTable
	INSERT INTO @ReindexPermissionsTable VALUES('ReindexShowContig', 1)
END

-- SELECT Values FROM @PermissionsTable
SELECT PermissionName, PermissionValue FROM @ReindexPermissionsTable

END TRY

BEGIN CATCH
    declare @ReindexErrorMessage nvarchar(max), @ReindexErrorSeverity int, @ReindexErrorState int;
    select @ReindexErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ReindexErrorSeverity = ERROR_SEVERITY(), @ReindexErrorState = ERROR_STATE();
    raiserror (@ReindexErrorMessage, @ReindexErrorSeverity, @ReindexErrorState); -- raising the error and logging it as warning 
END CATCH
