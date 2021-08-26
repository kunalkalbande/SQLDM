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
--  Batch: Update statistics Permissions
--  Tables: sysobjects
--  Variables: 
--		[0] - Target Database
--		[1] - Table ID
--		[2] - user_name or schema_name
--------------------------------------------------------------------------------
--  SQLdm 10.3 (Varun Chopra)-- Created batch to get Update Statistics Permissions Information for Current User
---------------------------------------------------------------------------------------------------


BEGIN TRY

-- Table: Permissions Table to store the permissions
-- Permission Name : Name of the permissions
-- Permission Value : Value of the permissions
DECLARE @UpdateStatisticsPermissionsTable TABLE
(
	PermissionName nvarchar(256),
	PermissionValue int
)

-- Permission Value : To store value of the permission
DECLARE @UpdateStatisticsPermissionValue INT

use [{0}]

declare @update_statistics_qualified_table_name nvarchar(500)
select 
	@update_statistics_qualified_table_name = quotename(schema_name(uid)) + '.' + quotename(name) 
from
	sysobjects (nolock)
where 
	id = {1}

SELECT @UpdateStatisticsPermissionValue = 0

if (len(@update_statistics_qualified_table_name) > 0)
begin
	-- Check Alter Permissions
	SELECT @UpdateStatisticsPermissionValue = COUNT(*) FROM sys.fn_my_permissions(@update_statistics_qualified_table_name, 'OBJECT') WHERE permission_name = 'ALTER'
end

-- INSERT UpdateStatisticsPermissionsTable
INSERT INTO @UpdateStatisticsPermissionsTable VALUES('UPDATESTATISTICS', @UpdateStatisticsPermissionValue)

-- SELECT Values FROM @PermissionsTable
SELECT PermissionName, PermissionValue FROM @UpdateStatisticsPermissionsTable

END TRY

BEGIN CATCH
    declare @UpdateStatisticsErrorMessage nvarchar(max), @UpdateStatisticsErrorSeverity int, @UpdateStatisticsErrorState int;
    select @UpdateStatisticsErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @UpdateStatisticsErrorSeverity = ERROR_SEVERITY(), @UpdateStatisticsErrorState = ERROR_STATE();
    raiserror (@UpdateStatisticsErrorMessage, @UpdateStatisticsErrorSeverity, @UpdateStatisticsErrorState); -- raising the error and logging it as warning 
END CATCH
