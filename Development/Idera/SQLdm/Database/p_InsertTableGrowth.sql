if (object_id('p_InsertTableGrowth') is not null)
begin
drop procedure p_InsertTableGrowth
end
go
create procedure p_InsertTableGrowth
	@SQLServerID int,
	@UTCCollectionDateTime datetime,
	@DatabaseName nvarchar(128) = null,
	@SystemDatabase bit,
	@DatabaseID int = -1,
	@TableName nvarchar(255),
	@SchemaName nvarchar(255),
	@SystemTable bit,
	@NumberOfRows bigint,
	@DataSize float,
	@TextSize float,
	@IndexSize float,
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


insert into [TableGrowth]
	([TableID]
	,[UTCCollectionDateTime]
	,[NumberOfRows]
	,[DataSize]
	,[TextSize]
	,[IndexSize]
	,[TimeDeltaInSeconds])
 values
	(@TableID
	,@UTCCollectionDateTime
	,@NumberOfRows
	,@DataSize
	,@TextSize
	,@IndexSize
	,isnull(@TimeDeltaInSeconds,86400))

end
