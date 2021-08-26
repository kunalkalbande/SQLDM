if (object_id('p_InsertTableName') is not null)
begin
drop procedure p_InsertTableName
end
go
create procedure p_InsertTableName
	@SQLServerID int,
	@DatabaseName nvarchar(128) = null,
	@SystemDatabase bit,
	@TableName nvarchar(255),
	@SchemaName nvarchar(255),
    @SystemTable bit,
	@DatabaseID int output,
	@TableID int output,
	@ReturnMessage nvarchar(128) output
as
begin

declare @InnerReturnMessage nvarchar(128)

if isnull(@DatabaseID,0) < 1
	execute [p_InsertDatabaseName] 
	   @SQLServerID
	  ,@DatabaseName
	  ,@SystemDatabase
	  ,null
	  ,@DatabaseID output
	  ,@InnerReturnMessage output

declare @LookupTableID int

select 
	@LookupTableID = TableID 
from 
	SQLServerTableNames (nolock)
where 
	DatabaseID = @DatabaseID
	and TableName = @TableName
	and SchemaName = @SchemaName

if (@LookupTableID is null)
begin
	insert into [SQLServerTableNames]
	   ([DatabaseID]
	   ,[TableName]
	   ,[SchemaName]
	   ,[SystemTable])
	Values
	   (@DatabaseID
	   ,@TableName
	   ,@SchemaName
	   ,@SystemTable)

	select @LookupTableID = SCOPE_IDENTITY()
end

set @TableID = @LookupTableID
set @ReturnMessage = @InnerReturnMessage

end
