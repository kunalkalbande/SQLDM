if (object_id('p_InsertTableReorganization') is not null)
begin
drop procedure p_InsertTableReorganization
end
go
create procedure p_InsertTableReorganization
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@DatabaseName nvarchar(128) = null,
	@SystemDatabase bit,
	@DatabaseID int = -1,
	@TableName nvarchar(255),
	@SchemaName nvarchar(255),
	@SystemTable bit,
	@ScanDensity float,
	@LogicalFragmentation float,
	@TimeDeltaInSeconds float,
	@ReturnMessage nvarchar(128) output
as
begin

declare @InnerReturnMessage nvarchar(128), @TableID int

exec [p_InsertTableName] 
   @SQLServerID
  ,@DatabaseName
  ,@SystemDatabase
  ,@TableName
  ,@SchemaName
  ,@SystemTable
  ,@DatabaseID output
  ,@TableID output
  ,@InnerReturnMessage output

insert into [TableReorganization]
	([TableID]
	,[UTCCollectionDateTime]
	,[ScanDensity]
	,[LogicalFragmentation]
	,[TimeDeltaInSeconds])
 values
	(@TableID
	,@UTCCollectionDateTime
	,@ScanDensity
	,@LogicalFragmentation
	,isnull(@TimeDeltaInSeconds,86400))

-- was this, but error_message is only valid on SQL Server 2005
-- set @ReturnMessage = isnull(error_message(),'')
set @ReturnMessage = ''

return @@error


end

