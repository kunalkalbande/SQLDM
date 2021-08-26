if (object_id('Grooming.p_VerifyAGNode') is not null)
begin
drop procedure Grooming.p_VerifyAGNode
end
go

CREATE PROCEDURE Grooming.p_VerifyAGNode
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


Declare       @DatabasesToCheck Table (
       name Sysname Not Null
)

IF OBJECT_ID('sys.availability_databases_cluster', 'V') IS NULL 
begin
	-- the AlwaysOn view does not exists
	print 'Skipping AlwaysOn validation. This is a regular SQL Server (Non AG) or it does not has enough permissions'
	return 
end

Insert @DatabasesToCheck
SELECT db_name(db_ID())

If  (
    Exists (
        -- Database in an AG
        Select  Top 1 0
        From    sys.availability_databases_cluster adc
        Join    sys.availability_groups ag
        On      adc.group_id = ag.group_id 
        Join    sys.dm_hadr_availability_group_states dhags
        On      ag.group_id  = dhags.group_id 
        Where   adc.database_name COLLATE DATABASE_DEFAULT In (Select dtc.name COLLATE DATABASE_DEFAULT From @DatabasesToCheck dtc)
        )  
    And     Not Exists (
        -- Database in an AG which is Primary on this instance
        Select  Top 1 0
        From    sys.availability_databases_cluster adc
        Join    sys.availability_groups ag
        On      adc.group_id = ag.group_id
        Join    sys.dm_hadr_availability_group_states dhags
        On      ag.group_id = dhags.group_id
        Where   adc.database_name COLLATE DATABASE_DEFAULT In (Select dtc.name COLLATE DATABASE_DEFAULT From @DatabasesToCheck dtc)
        And     Upper(dhags.primary_replica) = Upper(@@Servername)
		And  dhags.primary_replica
      = @@Servername
        )
    )  
Begin
    Raiserror('This is not the primary replica.', 16, 1) With Nowait
    Return
End
 
-- Check if the database isn't accessible
If  Not Exists (
    Select  Top 1 0
    From    sys.databases
    Where   databases.name COLLATE DATABASE_DEFAULT In (Select dtc.name COLLATE DATABASE_DEFAULT From @DatabasesToCheck dtc)
    And     state_desc = 'ONLINE'
    )
Begin
    Raiserror('The database doesn''t exist or isn''t ONLINE on this node.', 16, 1) With Nowait
    Return
End

-- Readonly detection
declare @accessMode nvarchar(4000)
set @accessMode = (SELECT CONVERT(nvarchar(4000),DATABASEPROPERTYEX(DB_NAME(), 'Updateability')))
if @accessMode collate DATABASE_DEFAULT = 'READ_ONLY'
begin
    Raiserror('The database is in READ_ONLY mode.', 16, 1) With Nowait
    Return
end
else
begin
	print 'accessMode: ' +  @accessMode
end

END
GO
