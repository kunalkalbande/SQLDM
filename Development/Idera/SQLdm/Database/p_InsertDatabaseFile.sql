if (object_id('p_InsertDatabaseFile') is not null)
begin
drop procedure p_InsertDatabaseFile
end
go
create procedure p_InsertDatabaseFile
	@DatabaseID int,
	@FileName nvarchar(255),
	@FileType bit,
	@FilePath nvarchar(1000),
	@DriveName nvarchar(256),
	@FileID int output
as
begin

declare @LookupFileID int, @TempDriveName nvarchar(256)


-- Do not match on drive letter because this may change with config changes
select 
	@LookupFileID = FileID,
	@TempDriveName = DriveName
from 
	DatabaseFiles (nolock)
where 
	DatabaseID = @DatabaseID
	and FileName = @FileName
	and FilePath = @FilePath

if (@LookupFileID is null)
begin
	insert into [DatabaseFiles]
	   (
		[DatabaseID]
	   ,[FileName]
  	   ,[FileType]
	   ,[FilePath]
	   ,[DriveName])
	Values
	   (@DatabaseID
	   ,@FileName
	   ,@FileType
	   ,@FilePath
	   ,@DriveName)

	select @LookupFileID = SCOPE_IDENTITY()
end
else
begin
	if (@TempDriveName <> @DriveName)
		update [DatabaseFiles] set DriveName = @DriveName where FileID = @LookupFileID
end

set @FileID = @LookupFileID

end
