if (object_id('p_InsertFileActivity') is not null)
begin
drop procedure p_InsertFileActivity
end
go
create procedure p_InsertFileActivity
	@DatabaseID int,
	@FileName nvarchar(255),
	@FileType bit,
	@FilePath nvarchar(1000),
	@FileID int,
	@UTCCollectionDateTime datetime,
	@TimeDeltaInSeconds float,
	@NumberReads decimal,
	@NumberWrites decimal,
	@DriveName nvarchar(256)
as
begin

if (@FileID is null)
begin
	execute [p_InsertDatabaseFile]
		@DatabaseID,
		@FileName,
		@FileType,
		@FilePath,
		@DriveName,
		@FileID output
end

if (@FileID is null)
	return


insert into
	[DatabaseFileActivity] 
	(
	[FileID],
	[UTCCollectionDateTime],
	[Reads],
	[Writes],
	[TimeDeltaInSeconds]
	)
values
	(
	@FileID,
	@UTCCollectionDateTime,
	@NumberReads,
	@NumberWrites,
	@TimeDeltaInSeconds
	)

end

